using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindObjectOfType<GameLauncher>() != null)
            return;

        var root = new GameObject("GameLauncher");
        root.AddComponent<GameLauncher>();
    }
}
