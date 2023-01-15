using Mixin.Utils.Audio;
using Mixin.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mixin.Popup
{
    /// <summary>
    /// 
    /// </summary>
    public class PopupObject
    {
        /// <summary>
        /// 
        /// </summary>
        private string _title;

        /// <summary>
        /// 
        /// </summary>
        private string _message;

        /// <summary>
        /// 
        /// </summary>
        private AudioTrackSetup _soundOpen = null;
        
        /// <summary>
        /// 
        /// </summary>
        private AudioTrackSetup _soundClose = null;

        /// <summary>
        /// 
        /// </summary>
        private  List<PopupButton> _buttonList = new List<PopupButton>();

        /// <summary>
        /// 
        /// </summary>
        private List<PopupImage> _imageList = new List<PopupImage>();

        private MixinDictionary<PopupImagePosition, Color> _pos = 
            new MixinDictionary<PopupImagePosition, Color>();

        /***********************************/
        /********************************/
        /// <inheritdoc cref="_title"/>
        public string Title { get => _title; }

        /// <inheritdoc cref="_message"/>
        public string Message { get => _message; }

        /// <inheritdoc cref="_soundOpen"/>
        public AudioTrackSetup SoundOpen { get => _soundOpen; }

        /// <inheritdoc cref="_soundClose"/>
        public AudioTrackSetup SoundClose { get => _soundClose; }

        /// <inheritdoc cref="_buttonList"/>
        public List<PopupButton> ButtonList { get => _buttonList; }

        /// <inheritdoc cref="_imageList"/>
        public List<PopupImage> ImageList { get => _imageList; }

        /// <summary>
        /// 
        /// </summary>
        public event Action OnPopupClosed;

        /***********************************/
        /****************Constructor*******************/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public PopupObject(
            string title,
            string message
            )
        {
            _title = title;
            _message = message;
        }

        /***********************************/
        /****************Popup Functionality*******************/
        /// <summary>
        /// Adds a Button from the Prefab.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public PopupObject AddButton(PopupButton button)
        {
            _buttonList.Add(button);
            return this;
        }

        /// <summary>
        /// Show an Image in the Popup.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public PopupObject AddSprite(PopupImage image)
        {
            _imageList.Add(image);
            return this;
        }

        /// <summary>
        /// Add a Sound for when the Popup gets opened.
        /// </summary>
        public PopupObject AddPopupOpenSound(AudioTrackSetup audioClip)
        {
            _soundOpen = audioClip;
            return this;
        }

        /// <summary>
        /// Add a Sound for when the Popup gets closed.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        public PopupObject AddPopupCloseSound(AudioTrackSetup audioClip)
        {
            _soundClose = audioClip;
            return this;
        }

        public PopupObject SetMessageBoxColor(PopupImagePosition messageBox, Color color)
        {
            _pos.Add(messageBox, color);
            return this;
        }

        /***********************************/
        /***********************************/
        /// <summary>
        /// Adds the PopupObject to the Queue.
        /// </summary>
        /// <returns></returns>
        public PopupObject AddToList()
        {
            PopupManager.Instance.AddPopupObjectToList(this);
            return this;
        }

        /// <summary>
        /// This opens all Popups from the List one after another.
        /// </summary>
        /// <returns></returns>
        public PopupObject OpenAll()
        {
            PopupManager.Instance.TryOpenNext();
            return this;
        }

        /// <summary>
        /// Adds this PopupObject to the List and tries to open it.
        /// </summary>
        /// <returns></returns>
        public PopupObject AutoOpen()
        {
            AddToList();
            PopupManager.Instance.TryOpenNext();
            return this;
        }

        /***********************************/
        /****************Events*******************/
        public void FireOnPopupClosedEvent()
        {
            OnPopupClosed?.Invoke();
        }
    }

}