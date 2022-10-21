using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mixin.Utils;
using UnityEditor;
using Mixin.Audio;

namespace Mixin.Popup
{
    [ExecuteAlways]
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField]
        private bool _showPopupInEditor = true;

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

        [Header("Animation")]
        [SerializeField] Animator _animator;
        [SerializeField]
        private string _triggerVariableOpen = "open";

        [SerializeField]
        private string _triggerVariableClose = "close";

        [Header("Message Box")]
        [SerializeField] Image _messageBoxBackgroundImage;
        [SerializeField] Image _messageBoxOverlay;
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _message;

        [Space]
        [SerializeField]
        GameObject _imageContainer;

        /* /// <summary>
         /// If true it will automatically refrence the ImageObjects based of the ImageContainer.
         /// </summary>
         [SerializeField]
         [Tooltip("If true it will automatically refrence the ImageObjects based of the ImageContainer.")]
         bool _autoIndexImageChildren = true;*/

        [SerializeField]
        private MixinDictionary<PopupImagePosition, Image> _imageList =
            new MixinDictionary<PopupImagePosition, Image>();

        [Obsolete]
        Image _imageBackground;
        [ConditionalField("_autoIndexImageChildren", false)]
        [Obsolete]
        Image _imageMiddleground;
        [ConditionalField("_autoIndexImageChildren", false)]
        [Obsolete]
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
        [SerializeField] Color _defaultButtonColor = Color.grey;
        [SerializeField] Color _defaultColor = Color.white;
        [SerializeField] Color _successColor = Color.green;
        [SerializeField] Color _warningColor = Color.yellow;
        [SerializeField] Color _errorColor = Color.red;

        [Obsolete] private const float _SATURATION = 0.1f;
        private PopupObject _lastOpenedPopupObject = null;

        private AudioManager _audioManager;
        private List<PopupObject> _popupObjectList = new List<PopupObject>();

        /// <summary>
        /// This bool tells if there is currently a Popup open.
        /// </summary>
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

            _popupComposition.SetActive(false);

            SetupFinished = true;
        }
        private void Clear()
        {
            // Clear button listeners
            _backgroundButton.onClick.RemoveAllListeners();

            // Set inactive
            _title.gameObject.SetActive(false);
            _message.gameObject.SetActive(false);
            _imageContainer.SetActive(false);

            // Disable each Image.
            foreach (KeyValuePair<PopupImagePosition, Image> image in _imageList)
                image.Value.gameObject.SetActive(false);

            // Destroy all buttons.
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

            SetText(_title, popupObject.Title);
            SetText(_message, popupObject.Message);

            GenerateImages(popupObject);

            // Show ImageContainer when there are Images.
            if (popupObject.ImageList.Count >= 1)
                _imageContainer.SetActive(true);

            GenerateButtons(popupObject);

            // Close Popup when clicked on the background.
            if (_backgroundButton != null)
                _backgroundButton.onClick.AddListener(OnCloseButtonClick);
            // Close Popup when clicked on the [X]-Button.
            if (_closeXButton != null)
                _closeXButton.onClick.AddListener(OnCloseButtonClick);

            /* at this point it gets visible, config should be done before */

            //now show everything
            _popupComposition.SetActive(true);

            PlaySound(popupObject.SoundOpen);

            // Trigger Animation
            _animator.SetTrigger(_triggerVariableOpen);

            //remove object from list if it exists
            if (_popupObjectList.Contains(popupObject))
                _popupObjectList.Remove(popupObject);
        }

        private void GenerateImages(PopupObject popupObject)
        {
            // Show all Images from List.
            foreach (PopupImage popupImage in popupObject.ImageList)
            {
                Image image = _imageList[popupImage.Position];
                image.sprite = popupImage.Sprite;
                image.color = popupImage.Color;
                image.gameObject.SetActive(true);
            }
        }

        private void GenerateButtons(PopupObject popupObject)
        {
            // Generate all buttons.
            foreach (PopupButton button in popupObject.ButtonList)
            {
                // Get the button component.
                MixinButton buttonObj = _buttonPrefab.GeneratePrefab(_buttonPrefabContainer)
                    .GetComponent<MixinButton>();
                Image imageObj = buttonObj.GetComponent<Image>();

                // Set the button text.
                buttonObj.ButtonText.text = button.Text;
                imageObj.color = button.BackgroundColor;

                // Add the listeners.
                if (button.Call != null)
                    buttonObj.onClick.AddListener(() => button.Call());
                buttonObj.onClick.AddListener(Close);
                buttonObj.onClick.AddListener(() => PlaySound(button.OnClickSound));

                buttonObj.gameObject.SetActive(true);
            }
        }

        private void PlaySound(AudioTrackSetup audioTrackSetup)
        {
            AudioManager.Instance.Play(audioTrackSetup);
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
            _popupObjectList.AddRange(popupObjectList);
        }

        // closes the popup
        public void Close()
        {
            // Try Play Sound
            //if (_audioManager != null)
            //audioManager.Play(audioManager.CreateNewSound(_soundClose), false);

            //the trigger automatically calls OnPopupClosed() and closes the popup
            //_animator.SetTrigger("Close");

            //_animator.Play("close");
            _animator.SetTrigger(_triggerVariableClose);
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

            if (_showPopupInEditor)
                _popupComposition.SetActive(true);
            else
                _popupComposition.SetActive(false);
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