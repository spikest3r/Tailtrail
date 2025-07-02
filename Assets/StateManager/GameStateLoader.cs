using UnityEngine;

public class GameStateLoader : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        if (!StateSavingController.RuntimeState.DidLoadSave)
        {
            Debug.Log("First time save");
            StateSavingController.RuntimeState.DidLoadSave = true;
            StateSavingController.saveState.LoadState();
        }
        Destroy(this); // destroy component
    }
}