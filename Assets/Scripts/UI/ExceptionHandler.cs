using UnityEngine;

public class ExceptionHandler : MonoBehaviour
{
    public GameObject fove;
    public ViewManager viewManager;

    void Awake()
    {
        Application.logMessageReceived += HandleException;
    }

    void HandleException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception && stackTrace.Contains("FoveInterface"))
        {
            // Turn off Fove
            fove.SetActive(false);
            // Switch to UI view
            viewManager.ResetView();
        }
    }
}