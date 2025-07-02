using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerDoorEntranceHandler : MonoBehaviour
{
    PlayerMovement movController;
    bool playerInRange = false;
    GameObject obj;
    public bool StartWithKey = true;
    bool running = false;

    public UnityEvent onCutsceneEnd;
    [SerializeField] bool invokeCutsceneAction = true;

    void Awake()
    {
        if(SceneManager.GetActiveScene().name != "World")
        {
            StateSavingController.RuntimeState.IsInsideBuilding = true; // handle edge case if we directly load into the House scene
        }
    }

    void Start()
    {
        movController = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Door"))
        {
            playerInRange = true;
            obj = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            playerInRange = false;
            obj = null;
        }
    }

    void Update()
    {
        if (running) return;
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame && StartWithKey)
        {
            movController.SetAllowedToMove(false);
            StartCoroutine(Animation());
            Debug.Log("Door transition");
        }
    }

    public IEnumerator Animation(bool playAnimationOnly = false, AnimatedDoor door = null) // async method to play cutscene
    {
        running = true;
        if(Camera.main.gameObject.TryGetComponent<CameraFollow>(out var cam)) cam.enabled = false; // do not follow player
        AnimatedDoor d = door == null ? obj.GetComponent<AnimatedDoor>() : door;
        yield return d.SetDoorState(true); // open door
        yield return new WaitForSeconds(0.5f);
        Direction dir = d.GetDirection();
        yield return movController.ArtificialMove(2f,dir,3f); // move a bit to "enter" house
        yield return new WaitForSeconds(0.5f);
        yield return d.SetDoorState(false); // close door
        running = false;
        if (cam != null) cam.enabled = true; // follow player (return to default)
        if (playAnimationOnly) {
            if(invokeCutsceneAction)
            {
                onCutsceneEnd.Invoke();
                invokeCutsceneAction = false;
            }
            yield break;
        }
        string destination = d.SceneDestination;
        StateSavingController.RuntimeState.IsInsideBuilding = destination != "World";
        SceneTransition.Instance.LoadScene(destination);
    }
}
