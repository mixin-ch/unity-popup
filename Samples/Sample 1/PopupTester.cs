using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mixin.Popup;

public class PopupTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        new PopupObject(PopupType.Default, "Title", "Message")
            .AddButton(new PopupButton("Submit", null))
            .AddButton(new PopupButton("Submit", null))
            .AddButton(new PopupButton("Submit", null))
            .AutoOpen();
    }
}
