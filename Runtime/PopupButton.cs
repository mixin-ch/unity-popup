using Mixin.Audio;
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
        private AudioTrackSetup _onClickSound;

        public string Text { get => _text; set => _text = value; }
        public Action Call { get => _call; set => _call = value; }
        public Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }
        public AudioTrackSetup OnClickSound { get => _onClickSound; set => _onClickSound = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="call"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="onClickSound"></param>
        public PopupButton(string text, Action call, Color backgroundColor, AudioTrackSetup onClickSound)
        {
            _text = text;
            _call = call;
            _backgroundColor = backgroundColor;
            _onClickSound = onClickSound;
        }
    }
}