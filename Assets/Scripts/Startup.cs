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
    private IEnumerator Start()
    {
        TransitionSystem transitionSystem = TransitionSystem.Get();
        ModSystem modSystem = ModSystem.Get();
        transitionSystem.SetSelectedTransition("OpacityFadeTransition");
        if (transitionSystem.SelectedTransition is not OpacityFadeTransition fadeTransition)
        {
            yield return transitionSystem.MakeTransitionInactive();
            yield break;
        }
        
        if (fadeTransition.TargetText)
        {
            fadeTransition.TargetText.text = "Loading mod assemblies...";
        }
        
        void StateChanged(ModSystem _, ModSystemState state, string stateDescription)
        {
            if (fadeTransition.TargetText)
            {
                fadeTransition.TargetText.text = stateDescription;
            }
        }
        
        yield return transitionSystem.MakeTransitionActive();
        //var result = modSystem.LoadModAssembliesAsync();
        modSystem.OnStateChanged += StateChanged;
        //yield return new WaitUntil(() => result.IsCompleted);
        modSystem.OnStateChanged -= StateChanged;
        
        transitionSystem.StartCoroutine(transitionSystem.LoadScene("TitleScene"));
    }
}
