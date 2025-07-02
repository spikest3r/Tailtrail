using System.Collections;
using UnityEngine;

// semi automatic call helper
public class PlayerHouseAnimation : MonoBehaviour
{
    public PlayerMovement player;
    public AnimatedDoor door;
    public bool StartRightAway = false;

    void Start()
    {
        if (StartRightAway)
            StartCoroutine(WaitForFieldsThenStartAnim());
    }

    IEnumerator WaitForFieldsThenStartAnim()
    {
        while (player == null || door == null)
            yield return null;

        player.SetAllowedToMove(false);
        yield return new WaitForSeconds(1f);
        PlayerDoorEntranceHandler phe = player.GetComponent<PlayerDoorEntranceHandler>();
        phe.StartWithKey = false;
        yield return phe.Animation(true, door);
        phe.StartWithKey = true;
        player.SetAllowedToMove(true);
        Destroy(this);
    }
}