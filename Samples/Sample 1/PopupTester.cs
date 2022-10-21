using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mixin.Popup;
using UnityEngine.UI;
using Mixin.Audio;

public class PopupTester : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Sprite _sprite1;
    [SerializeField]
    private Sprite _sprite2;
    [SerializeField]
    private AudioTrackSetupSO _onButtonClickSound;

    // Start is called before the first frame update
    private void OnEnable()
    {
        _button.onClick.AddListener(CreatePopup);
    }

    private void CreatePopup()
    {
        AudioTrackSetup onButtonClickSound = _onButtonClickSound.ToAudioTrackSetup();

        new PopupObject("Title", "Message")
                    .AddButton(new PopupButton("Submit1", null, Color.yellow, onButtonClickSound))
                    .AddButton(new PopupButton("Submit2", null, Color.grey, onButtonClickSound))
                    .AddSprite(new PopupImage(_sprite1))
                    .AddSprite(new PopupImage(_sprite2, Color.white, PopupImagePosition.Foreground))
                    .AddOpenSound(onButtonClickSound)
                    .AddCloseSound(onButtonClickSound)
                    .AutoOpen();
    }
}
