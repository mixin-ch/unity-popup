using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mixin.Popup;
using UnityEngine.UI;
using Mixin.Utils.Audio;

public class PopupTester : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Sprite _sprite1;
    [SerializeField]
    private Sprite _sprite2;
    [SerializeField]
    private AudioTrackSetupSO _buttonClickSound;
    [SerializeField]
    private AudioTrackSetupSO _popupOpenSound;
    [SerializeField]
    private AudioTrackSetupSO _popupCloseSound;

    // Start is called before the first frame update
    private void OnEnable()
    {
        _button.onClick.AddListener(CreatePopup);
    }

    private void CreatePopup()
    {
        AudioTrackSetup buttonClickSound = _buttonClickSound.ToAudioTrackSetup();
        AudioTrackSetup popupOpenSound = _popupOpenSound.ToAudioTrackSetup();
        AudioTrackSetup popupCloseSound = _popupCloseSound.ToAudioTrackSetup();

        new PopupObject("Title", "Message")
                    .AddButton(new PopupButton("Submit", null, Color.yellow, buttonClickSound))
                    .AddButton(new PopupButton("Cancel", null, Color.grey, buttonClickSound))
                    .AddSprite(new PopupImage(_sprite1))
                    .AddSprite(new PopupImage(_sprite2, Color.white, PopupImagePosition.Foreground))
                    .AddPopupOpenSound(popupOpenSound)
                    .AddPopupCloseSound(popupCloseSound)
                    .AutoOpen();
    }
}
