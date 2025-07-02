using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroHelper : MonoBehaviour
{
    [SerializeField] NPCDialogController dialog;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame && !dialog.Active)
        {
            dialog.Activate(0);
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        dialog.EndDialog();
        yield return new WaitForSeconds(1.5f);
        SceneTransition.Instance.LoadScene("World");
    }
}
