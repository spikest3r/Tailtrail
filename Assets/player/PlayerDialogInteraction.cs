using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDialogInteraction : MonoBehaviour
{
    NPCDialogController controller;

    void Update()
    {
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            if(controller != null && !controller.Active)
            {
                controller.Activate();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("NPC"))
        {
            controller = collision.gameObject.GetComponent<NPCDialogController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("NPC"))
        {
            controller = null;
        }
    }
}
