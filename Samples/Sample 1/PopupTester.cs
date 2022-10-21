using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mixin.Popup;
using UnityEngine.UI;

public class PopupTester : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Sprite _sprite1;
    [SerializeField]
    private Sprite _sprite2;

    // Start is called before the first frame update
    private void OnEnable()
    {
        _button.onClick.AddListener(CreatePopup);
    }

    private void CreatePopup()
    {
        new PopupObject(PopupType.Default, "Title", "Message")
                    .AddButton(new PopupButton("Submit1", null, Color.yellow))
                    .AddButton(new PopupButton("Submit2", null, Color.red))
                    .AddButton(new PopupButton("Submit3", null))
                    .AddSprite(new PopupImage(_sprite1))
                    .AddSprite(new PopupImage(_sprite2, Color.white, PopupImagePosition.Foreground))
                    .AutoOpen();
    }
}
