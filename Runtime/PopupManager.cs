using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mixin.Utils;
using UnityEditor;
using Mixin.Utils.Audio;

namespace Mixin.Popup
{
    /// <summary>
    /// Manages all Popups.
    /// </summary>
    [ExecuteAlways]
    public class PopupManager : Singleton<PopupManager>
    {
        /// <summary>
        /// When enabled it will output console logs.
        /// </summary>
        [Tooltip("When enabled it will output console logs.")]
        [SerializeField]
        private bool _debugMode = false;

        /// <summary>
        /// If the Popup should be visible in Editor Mode.
        /// </summary>
        [Tooltip("If the Popup should be visible in Editor Mode.")]
        [SerializeField]
        private bool _showPopupInEditor = true;

        /****************************************/
        /****************Base Setup***************/
        ///<summary>
        /// When enabled the setup will be called on awake.
        /// </summary>
        [Header("Base Setup")]
        [SerializeField]
        [Tooltip("When enabled the setup will be called on awake.")]
        private bool _autoSetup = false;

        /// <summary>
        /// The object that will be hidden, when Popup closes.
        /// </summary>
        [Tooltip("The object that will be hidden, when Popup closes.")]
        [SerializeField]
        private GameObject _popupComposition;

        /****************************************/
        /****************Animation***************/
        /// <summary>
        /// The Animator with all Animations.
        /// </summary>
        [Header("Animation")]
        [SerializeField]
        private Animator _animator;

        /// <summary>
        /// This variable will trigger in the Animator. 
        /// You can use it for Transition Conditions.<br></br>
        /// This variable gets called when the Popup opens.
        /// </summary>
        [Tooltip("This variable will trigger in the Animator. " +
            "You can use it for Transition Conditions." +
            "This variable gets called when the Popup opens.")]
        [SerializeField]
        private string _triggerVariableOpen = "open";

        /// <summary>
        /// This variable will trigger in the Animator. 
        /// You can use it for Transition Conditions.<br></br>
        /// This variable gets called when the Popup closes.
        /// </summary>
        [Tooltip("This variable will trigger in the Animator. " +
            "You can use it for Transition Conditions." +
            "This variable gets called when the Popup closes.")]
        [SerializeField]
        private string _triggerVariableClose = "close";

        /****************************************/
        /****************Message Box***************/
        [Header("Message Box")]
        [SerializeField]
        private Image _messageBoxBackgroundImage;
        [SerializeField]
        private Image _messageBoxOverlay;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TMP_Text _title;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TMP_Text _message;

        /// <summary>
        /// The Container that holds all Images.
        /// </summary>
        [SerializeField]
        private GameObject _imageContainer;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private MixinDictionary<PopupImagePosition, Image> _imageList =
            new MixinDictionary<PopupImagePosition, Image>();

        /****************************************/
        /****************Buttons***************/
        /// <summary>
        /// 
        /// </summary>
        [Header("Buttons")]
        [SerializeField]
        private GameObject _buttonPrefabContainer;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _buttonPrefab;

        /// <summary>
        /// A list of buttons that will close the popup onClick. <br></br>
        /// You can use this e.g. for clicking on the background and the [X]-Button.
        /// </summary>
        [Tooltip("A list of buttons that will close the popup onClick." +
            "You can use this e.g. for clicking on the background and the [X]-Button.")]
        [SerializeField]
        private List<Button> _escapeButtonList = new List<Button>();

        /****************************************/
        /*******************************/
        /// <summary>
        /// The Popup Queue.
        /// </summary>
        private List<PopupObject> _popupObjectList = new List<PopupObject>();

        /// <summary>
        /// The Popup that is currently active or was previous opened.
        /// </summary>
        private PopupObject _lastOpenedPopupObject = null;

        /// <summary>
        /// This bool tells if there is currently a Popup open.
        /// </summary>
        private bool _hasOpenPopup = false;

        /****************************************/
        /****************Events***************/
        public static event Action<PopupObject> OnPopupOpened;
        public static event Action<PopupObject> OnPopupClosed;


        protected override void Awake()
        {
            base.Awake();
            if (_autoSetup)
                Setup();
        }

        private void OnEnable()
        {
            // Close Popup when clicked on the background.
            // Close Popup when clicked on the [X]-Button.
            foreach (Button button in _escapeButtonList)
                button.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            // Clear button listeners
            foreach (Button button in _escapeButtonList)
                button.onClick.RemoveAllListeners();
        }

        public void Setup()
        {
            if (_debugMode)
                $"Setting up Popup".LogProgress();

            _popupComposition.SetActive(false);

            if (_debugMode)
                $"Popup set up".LogSuccess();
        }
        private void Clear()
        {
            // Clear Color of Message Boxes.
            _messageBoxBackgroundImage.color = Color.white;
            _messageBoxOverlay.color = Color.white;

            // Set inactive
            _title.gameObject.SetActive(false);
            _message.gameObject.SetActive(false);
            _imageContainer.SetActive(false);

            // Disable each Image.
            foreach (KeyValuePair<PopupImagePosition, Image> image in _imageList)
                image.Value.gameObject.SetActive(false);

            // Destroy all buttons.
            _buttonPrefabContainer.DestroyChildren();

            if (_debugMode)
                $"Popup cleared".Log();
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
            if (_debugMode)
                $"Opening Popup".LogProgress();

            // First clear everything
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

            /* at this point it gets visible, config should be done before */

            // Now show everything
            _popupComposition.SetActive(true);

            PlaySound(popupObject.SoundOpen);

            // Trigger Animation
            _animator.SetTrigger(_triggerVariableOpen);

            // Remove object from list if it exists
            if (_popupObjectList.Contains(popupObject))
                _popupObjectList.Remove(popupObject);

            OnPopupOpened?.Invoke(popupObject);

            if (_debugMode)
                $"Popup opened".LogSuccess();
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
                Button buttonObj = _buttonPrefab.GeneratePrefab(_buttonPrefabContainer)
                    .GetComponent<Button>();
                Image imageObj = buttonObj.GetComponent<Image>();

                // Set the button text.
                // TODO: Set Button Text
                //buttonObj.Text.text = button.Text;
                imageObj.color = button.BackgroundColor;

                // Add the listeners.
                if (button.Call != null)
                    buttonObj.onClick.AddListener(() => button.Call());
                buttonObj.onClick.AddListener(Close);
                buttonObj.onClick.AddListener(() => PlaySound(button.OnClickSound));

                buttonObj.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Play a Sound.
        /// </summary>
        /// <param name="audioTrackSetup"></param>
        private void PlaySound(AudioTrackSetup audioTrackSetup)
        {
            AudioManager.Instance.PlayTrack(audioTrackSetup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textObj"></param>
        /// <param name="text"></param>
        private void SetText(TMP_Text textObj, string text)
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

        // Closes the popup
        public void Close()
        {
            if (_debugMode)
                $"Closing Popup".LogProgress();

            //the trigger automatically calls HandleOnPopupClosed() and closes the popup
            PlaySound(_lastOpenedPopupObject.SoundClose);

            _animator.SetTrigger(_triggerVariableClose);
        }

        // This method gets called by the animator
        public void HandleOnPopupClosed()
        {
            _popupComposition.SetActive(false);

            _hasOpenPopup = false;
            _lastOpenedPopupObject.FireOnPopupClosedEvent();
            OnPopupClosed?.Invoke(_lastOpenedPopupObject);

            TryOpenNext();

            if (_debugMode)
                $"Popup closed".LogSuccess();
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
}