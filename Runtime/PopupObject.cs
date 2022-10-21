using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mixin.Popup
{
    public class PopupObject
    {
        public PopupType PopupType;
        public string Title;
        public string Message;
        public AudioClip SoundOpen = null;
        public AudioClip SoundClose = null;

        public event Action OnPopupClosed;

        public List<PopupButton> ButtonList = new List<PopupButton>();
        public List<PopupImage> ImageList = new List<PopupImage>();

        public PopupObject(
            PopupType popupType,
            string title,
            string message
            )
        {
            PopupType = popupType;
            Title = title;
            Message = message;
        }

        public PopupObject AddButton(PopupButton button)
        {
            ButtonList.Add(button);
            return this;
        }

        public PopupObject AddSprite(PopupImage image)
        {
            ImageList.Add(image);
            return this;
        }

        public PopupObject AddOpenSound(AudioClip audioClip)
        {
            SoundOpen = audioClip;
            return this;
        }

        public PopupObject AddCloseSound(AudioClip audioClip)
        {
            SoundClose = audioClip;
            return this;
        }



        /*--------------*/
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

        /*--------------*/

        public void FireOnPopupClosedEvent()
        {
            OnPopupClosed?.Invoke();
        }
    }

}