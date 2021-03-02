using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Dropdown DResolution;
    public void QuitGame()
    {
        Debug.Log("Vous avez quitter le jeu");
        Application.Quit();
    }

    public void SetResolution()
    {
        switch (DResolution.value)
        {
            case 0:
                Screen.SetResolution(640, 360, Screen.fullScreen);
                break;

            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;

            case 2:
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;

            case 3:
                Screen.SetResolution(7680, 4320, Screen.fullScreen);
                break;
        }
    }

    public void SetQuality(int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
