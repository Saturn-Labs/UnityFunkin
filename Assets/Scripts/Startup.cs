using System.Collections;
using System.Collections.Generic;
using ModManagement;
using ModManagement.State;
using TransitionManagement;
using TransitionManagement.Default;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    private void Start()
    {
        TransitionManager.Main.SetSelectedTransition("OpacityFadeTransition");
        TransitionManager.Main.LoadScene("TitleScene");
    }
}
