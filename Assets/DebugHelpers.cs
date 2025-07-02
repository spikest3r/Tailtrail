using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugHelpers : MonoBehaviour
{
    [SerializeField] bool AlwaysActive = false;

    void Awake()
    {
        if (!Application.isEditor && !AlwaysActive) Destroy(this);
    }

    private void Start()
    {
        Debug.LogWarning("Debug helper active");
    }

    void Update()
    {
        if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            StateSavingController.saveState.SaveState();
        }
        if(Keyboard.current.oKey.wasPressedThisFrame)
        {
            Debug.LogError("File deleted!");
            File.Delete(StateSavingController.SaveFilePath);
        }
    }
}
