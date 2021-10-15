using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public Canvas howToPlay;
    public Button howToPlayOkay;
    public Canvas controllerControls;
    public Canvas keyboardControls;
    public Button controllerOkay;
    public Button keyboardOkay;

    public void SwitchToControllerControls()
    {
        controllerControls.enabled = true;
        controllerOkay.Select();
        howToPlay.enabled = false;
    }

    public void SwitchTKeyboardControls()
    {
        keyboardControls.enabled = true;
        keyboardOkay.Select();
        howToPlay.enabled = false;
    }

    public void GoBackToHTP(Canvas from)
    {
        howToPlay.enabled = true;
        howToPlayOkay.Select();
        from.enabled = false;
    }
}
