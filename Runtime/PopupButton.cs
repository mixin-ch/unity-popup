using System;
using UnityEngine;

namespace Mixin.Popup
{
    /// <summary>
    /// 
    /// </summary>
    public struct PopupButton
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

        /// <summary>
        /// 
        /// </summary>
        private AudioClip _onClickSound;

        public string Text { get => _text; set => _text = value; }
        public Action Call { get => _call; set => _call = value; }
        public Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        /// <inheritdoc cref="PopupButton.PopupButton(string, Action, Color)"/>
        public PopupButton(string text, Action call)
        {
            _text = text;
            _call = call;
            _backgroundColor = Color.grey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="call"></param>
        /// <param name="backgroundColor"></param>
        public PopupButton(string text, Action call, Color backgroundColor)
        {
            _text = text;
            _call = call;
            _backgroundColor = backgroundColor;
        }
    }
}