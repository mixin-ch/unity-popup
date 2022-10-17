using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mixin.Utils;

namespace Mixin.Popup
{
    [ExecuteAlways]
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField]
        private bool _showPopup = true;

        [Header("Base Setup")]
        ///<summary>
        /// When enabled the setup will be called on awake.
        /// </summary>
        [SerializeField]
        [Tooltip("When enabled the setup will be called on awake.")]
        private bool _autoSetup = false;
        [SerializeField] GameObject _popupComposition;

        /// <summary>
        /// If not null the Popup automatically closes when pressed on this background.
        /// </summary>
        [Tooltip("If not null the Popup automatically closes when pressed on this background.")]
        [SerializeField]
        Button _backgroundButton;
        [Obsolete]
        GameObject _contentContainer;
        [SerializeField] Animator _animator;

        [Header("Message Box")]
        [SerializeField] Image _messageBoxBackgroundImage;
        [SerializeField] Image _messageBoxOverlay;
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _message;

        [Space]
        [SerializeField]
        GameObject _imageContainer;

        /// <summary>
        /// If true it will automatically refrence the ImageObjects based of the ImageContainer.
        /// </summary>
        [SerializeField]
        [Tooltip("If true it will automatically refrence the ImageObjects based of the ImageContainer.")]
        bool _autoIndexImageChildren = true;
        [ConditionalField("_autoIndexImageChildren", false)]
        [SerializeField]
        Image _imageBackground;
        [ConditionalField("_autoIndexImageChildren", false)]
        [SerializeField]
        Image _imageMiddleground;
        [ConditionalField("_autoIndexImageChildren", false)]
        [SerializeField]
        Image _imageForeground;

        [Header("Buttons")]
        [SerializeField] GameObject _buttonPrefabContainer;
        [SerializeField] GameObject _buttonPrefab;
        [SerializeField] MixinButton _closeXButton;

        [Obsolete] TMP_Text SubmitButtonText;
        [Obsolete] Button SubmitButton;
        [Obsolete] TMP_Text CancelButtonText;
        [Obsolete] Button CancelButton;

        [Header("Sounds")]
        [SerializeField] AudioClip _soundDefault;
        [SerializeField] AudioClip _soundConfirm;
        [SerializeField] AudioClip _soundError;
        [SerializeField] AudioClip _soundReward;
        [SerializeField] AudioClip _soundClose;

        [Header("Colors")]
        [SerializeField] Color _defaultColor = Color.white;
        [SerializeField] Color _successColor = Color.green;
        [SerializeField] Color _warningColor = Color.yellow;
        [SerializeField] Color _errorColor = Color.red;

        [Obsolete] private const float _SATURATION = 0.1f;
        private PopupObject _lastOpenedPopupObject = null;

        private AudioManager _audioManager;
        private List<PopupObject> _popupObjectList = new List<PopupObject>();

        private bool _hasOpenPopup;

        public static event Action OnPopupOpened;
        public static event Action OnPopupClosed;


        public bool SetupFinished { get; private set; } = false;

        private void Awake()
        {
            if (_autoSetup)
                Setup();
        }

        public void Setup()
        {
            _audioManager = AudioManager.Instance;

            if (_showPopup)
                _popupComposition.SetActive(true);
            else
                _popupComposition.SetActive(false);

            if (_autoIndexImageChildren)
            {
                if (_imageContainer.transform.childCount < 3)
                    $"Please set 3 ImageObjects in the ImageContainer.".LogError();

                _imageBackground = _imageContainer.transform.GetChild(0)?.gameObject.GetComponent<Image>();
                _imageMiddleground = _imageContainer.transform.GetChild(1)?.gameObject.GetComponent<Image>();
                _imageForeground = _imageContainer.transform.GetChild(2)?.gameObject.GetComponent<Image>();
            }

            SetupFinished = true;
        }
        private void Clear()
        {
            //clear button listeners
            _backgroundButton.onClick.RemoveAllListeners();

            //set inactive
            _title.gameObject.SetActive(false);
            _message.gameObject.SetActive(false);
            _imageContainer.SetActive(false);
            _imageForeground.enabled = false;

            _buttonPrefabContainer.DestroyChildren();
        }

        // only open with message
        // duration 0 waits until player clicked on button
        // open with message and image
        public void TryOpenNext()
        {
            if (_popupObjectList.Count > 0 && !_hasOpenPopup)
                Open(_popupObjectList[0]);
        }
        public void Open(PopupObject popupObject)
        {
            //first clear everything
            Clear();

            // Set the last opened Popup Object
            _lastOpenedPopupObject = popupObject;
            _hasOpenPopup = true;

            // warn if cancel button and custom button are true
            if (popupObject.CancelButton && popupObject.CustomButton)
                Debug.LogWarning(
                    $"{MethodBase.GetCurrentMethod()} Conflict: Cancel Button and Custom Button are true. Overwriting Cancel Button...");

            SetText(_title, popupObject.Title);
            SetText(_message, popupObject.Message);
            SetImage(_imageBackground, popupObject.ImageBackeground, popupObject.ImageBackgroundColor);
            SetImage(_imageMiddleground, popupObject.ImageMiddleground, popupObject.ImageMiddlegroundColor);
            SetImage(_imageForeground, popupObject.ImageForeground, popupObject.ImageForegroundColor);

            // Generate all buttons.
            foreach (PopupButton button in popupObject.ButtonList)
            {
                // Get the button component.
                MixinButton buttonObj = _buttonPrefab.GeneratePrefab(_buttonPrefabContainer)
                    .GetComponent<MixinButton>();

                // Set the button text.
                buttonObj.ButtonText.text = button.Text;

                // Add the listeners.
                if (button.Call != null)
                    buttonObj.onClick.AddListener(() => button.Call());
                buttonObj.onClick.AddListener(Close);

                buttonObj.gameObject.SetActive(true);
            }

            //close popup when clicked somewhere on background
            if (_backgroundButton != null)
                _backgroundButton.onClick.AddListener(OnCloseButtonClick);

            /* at this point it gets visible, config should be done before */

            //now show everything
            _popupComposition.SetActive(true);

            //variables based on type
            AudioClip audioClip = null;
            string animationName = null;

            // Fit the Rect Transform of the Overlay
            RectTransform rect = _messageBoxOverlay.gameObject.GetComponent<RectTransform>();

            rect.offsetMin = new Vector2(0, 0); // Left, Bottom
            rect.offsetMax = new Vector2(0, 0); // Right, Top

            rect.anchorMin = new Vector2(0, 0); // Left, Bottom
            rect.anchorMax = new Vector2(1f, 1f); // Right, Top

            ExecuteEffects(popupObject, out audioClip, out animationName);

            //if PopupObject has custom audio clip, then overwrite
            if (popupObject.CustomAudioClip != null)
                audioClip = popupObject.CustomAudioClip;

            //execute PopupType stuff
            // Play Sound
            //if (_audioManager != null)
            //audioManager.Play(audioManager.CreateNewSound(audioClip), false);

            // Trigger Animation
            _animator.SetTrigger(animationName);

            //remove object from list if it exists
            if (_popupObjectList.Contains(popupObject))
                _popupObjectList.Remove(popupObject);
        }

        private void ExecuteEffects(PopupObject popupObject, out AudioClip audioClip, out string animationName)
        {
            //execute animations etc. based on type
            switch (popupObject.PopupType)
            {
                case PopupType.Confirm:
                    //if (popupObject.sprite == null)
                    //    popupObject.sprite = SpriteCollection.Instance.GetSprite(SpriteType.Checkmark);
                    audioClip = _soundConfirm;
                    animationName = "Open";
                    //_messageBoxBackgroundImage.color = _warningColor.WithSaturation(_SATURATION);
                    _messageBoxOverlay.color = _warningColor;
                    break;
                case PopupType.Error:
                    //if (popupObject.sprite == null)
                    //    popupObject.sprite = SpriteCollection.Instance.GetSprite(SpriteType.Error);
                    audioClip = _soundError;
                    animationName = "Open";
                    //_messageBoxBackgroundImage.color = _errorColor.WithSaturation(_SATURATION);
                    _messageBoxOverlay.color = _errorColor;
                    break;
                case PopupType.Reward:
                    //if (popupObject.sprite == null)
                    //    popupObject.sprite = SpriteCollection.Instance.GetSprite(SpriteType.Reward);
                    audioClip = _soundReward;
                    animationName = "Open";
                    //_messageBoxBackgroundImage.color = _successColor.WithSaturation(_SATURATION);
                    _messageBoxOverlay.color = _successColor;
                    break;
                default:
                    audioClip = _soundDefault;
                    animationName = "Open";
                    _messageBoxBackgroundImage.color = _defaultColor;
                    _messageBoxOverlay.color = _defaultColor;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textObj"></param>
        /// <param name="text"></param>
        private void SetText(TMP_Text textObj, string? text)
        {
            if (text != null)
            {
                textObj.text = text;
                textObj.gameObject.SetActive(true);
            }
            else
                textObj.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageObj"></param>
        /// <param name="sprite"></param>
        /// <param name="color"></param>
        private void SetImage(Image imageObj, Sprite? sprite, Color color)
        {
            if (sprite != null)
            {
                imageObj.sprite = sprite;
                imageObj.color = color;
                imageObj.gameObject.SetActive(true);
            }
            else
                imageObj.gameObject.SetActive(false);
        }

        /// <summary>
        /// Adds a PopupObject to the queue.
        /// </summary>
        /// <param name="popupObject"></param>
        public void AddPopupObjectToList(PopupObject popupObject)
        {
            _popupObjectList.Add(popupObject);
        }

        /// <summary>
        /// Adds a list of PopupObjects to the queue.
        /// </summary>
        /// <param name="popupObjectList"></param>
        public void AddPopupObjectsToList(List<PopupObject> popupObjectList)
        {
            this._popupObjectList.AddRange(popupObjectList);
        }

        // closes the popup
        public void Close()
        {
            // Try Play Sound
            if (_audioManager != null)
                //audioManager.Play(audioManager.CreateNewSound(_soundClose), false);

                //the trigger automatically calls OnPopupClosed() and closes the popup
                _animator.SetTrigger("Close");
        }

        //this method gets called by the animator
        public void HandleOnPopupClosed()
        {
            print("popup closed");
            _popupComposition.SetActive(false);

            // Fire Event on Popup Closed 
            _lastOpenedPopupObject.FireOnPopupClosedEvent();
            _hasOpenPopup = false;
            TryOpenNext();
        }

        private void OnSubmitButtonClick()
        {
            //TODO remove all listenesers
            //play sound
            //SubmitButton.onClick.RemoveAllListeners();
            Close();
        }
        private void OnCloseButtonClick()
        {
            //TODO remove all listenesers
            //play sound
            //SubmitButton.onClick.RemoveAllListeners();
            Close();
        }

        private void OnValidate()
        {
            Setup();
        }
    }
    public enum ButtonTextType
    {
        NONE,
        Yes,
        Confirm,
        Ok,
        Send,
        Submit,
        Play,
        Cancel,
        No,
        Exit,
        Cool,
        Wow,
        Save,
        Nice,
    }
    public enum PopupType
    {
        Default,
        Confirm,
        Error,
        Reward,
        Success,
    }
}