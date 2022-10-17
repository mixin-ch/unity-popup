using System;
using UnityEngine;

namespace Mixin.Popup
{
    /// <summary>
    /// 
    /// </summary>
    public class PopupButton
    {
        /// <summary>
        /// 
        /// </summary>
        private string _text;

        /// <summary>
        /// 
        /// </summary>
        private Action _call;

        /// <summary>
        /// 
        /// </summary>
        private Color _backgroundColor;

        public string Text { get => _text; set => _text = value; }
        public Action Call { get => _call; set => _call = value; }
        public Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        /// <inheritdoc cref="PopupButton.PopupButton(string, Action, Color)"/>
        public PopupButton(string text, Action call)
        {
            Text = text;
            Call = call;
            BackgroundColor = Color.grey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="call"></param>
        /// <param name="backgroundColor"></param>
        public PopupButton(string text, Action call, Color backgroundColor)
        {
            Text = text;
            Call = call;
            BackgroundColor = backgroundColor;
        }


        public PopupButton GetSubmitButton(Action call)
        {
            return new PopupButton("Cancel", call);
        }
        public PopupButton GetCancelButton()
        {
            return new PopupButton("Cancel", null);
        }
    }
}