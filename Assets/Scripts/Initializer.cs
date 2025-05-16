using TransitionManagement;
using UnityEngine;

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Initialization logic for any singletons or static classes can go here
        _ = TransitionManager.Main;
    }
}
