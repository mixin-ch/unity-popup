using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mixin.Popup
{
    public class PopupObject
    {
        public PopupType PopupType;
        public string Title;
        public string Message;
        public string SubmitButtonText = ButtonTextType.Ok.ToString();
        public bool CancelButton = false;
        public AudioClip CustomAudioClip = null;

        public Sprite ImageBackeground = null;
        public Color ImageBackgroundColor = Color.white;
        public Sprite ImageMiddleground = null;
        public Color ImageMiddlegroundColor = Color.white;
        public Sprite ImageForeground = null;
        public Color ImageForegroundColor = Color.white;

        public Action Call = null;
        public bool SubmitButtonEnabled = true;

        public event Action OnPopupClosed;

        // custom button
        public bool CustomButton = false;
        public string CustomButtonText = null;
        public Action CustomButtonCall = null;

        public List<PopupButton> ButtonList = new List<PopupButton>();

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

        public PopupObject AddCancelButton()
        {
            CancelButton = true;
            return this;
        }
        public PopupObject AddSprite(Sprite sprite)
        {
            if (sprite != null)
                ImageMiddleground = sprite;
            return this;
        }
        public PopupObject AddSprite(Sprite sprite, Sprite spriteFrame)
        {
            if (sprite != null)
                ImageMiddleground = sprite;
            if (spriteFrame != null)
                ImageForeground = spriteFrame;
            return this;
        }
        public PopupObject AddSprite(Sprite sprite, Color color)
        {
            ImageMiddlegroundColor = color;
            return AddSprite(sprite);
        }
        public PopupObject AddSprite(Sprite sprite, Color color, Sprite spriteFrame, Color frameColor)
        {
            ImageMiddlegroundColor = color;
            ImageForegroundColor = frameColor;
            return AddSprite(sprite, spriteFrame);
        }
        public PopupObject AddCall(Action call)
        {
            Call = call;
            return this;
        }

        public PopupObject AddSubmitButtonText(string submitButtonText)
        {
            SubmitButtonText = submitButtonText;
            return this;
        }
        public PopupObject AddSubmitButtonText(ButtonTextType text)
        {
            SubmitButtonText = text.ToString();
            return this;
        }

        public PopupObject AddCustomSound(AudioClip audioClip)
        {
            CustomAudioClip = audioClip;
            return this;
        }

        public PopupObject AddCustomButton(string buttonText, Action buttonAction)
        {
            CustomButton = true;
            CustomButtonText = buttonText;
            CustomButtonCall = buttonAction;
            return this;
        }

        public PopupObject DisableSubmitButton()
        {
            SubmitButtonEnabled = false;
            return this;
        }



        /*--------------*/
        public PopupObject AddToList()
        {
            PopupManager.Instance.AddPopupObjectToList(this);
            return this;
        }
        public PopupObject OpenAll()
        {
            PopupManager.Instance.TryOpenNext();
            return this;
        }
        //auto adds the object to list and tries to open it
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