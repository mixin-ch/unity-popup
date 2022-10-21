using UnityEngine;

namespace Mixin.Popup
{
    /// <summary>
    /// 
    /// </summary>
    public struct PopupImage
    {
        /// <summary>
        /// 
        /// </summary>
        private Sprite _sprite;

        /// <summary>
        /// 
        /// </summary>
        private Color _color;

        /// <summary>
        /// 
        /// </summary>
        private PopupImagePosition _position;

        /// <inheritdoc cref="_sprite"/>
        public Sprite Sprite { get => _sprite; set => _sprite = value; }

        /// <inheritdoc cref="_color"/>
        public Color Color { get => _color; set => _color = value; }

        /// <inheritdoc cref="_position"/>
        public PopupImagePosition Position { get => _position; set => _position = value; }

        /// <inheritdoc cref="PopupImage(Sprite, Color)"/>
        public PopupImage(Sprite spriteMiddleground)
        {
            _sprite = spriteMiddleground;
            _color = Color.white;
            _position = PopupImagePosition.Middleground;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="color"></param>
        public PopupImage(Sprite sprite, Color color, PopupImagePosition position)
        {
            _sprite = sprite;
            _color = color;
            _position = position;
        }
    }

}