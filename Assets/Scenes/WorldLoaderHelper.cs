using System.Collections.Generic;
using UnityEngine;
using RS = StateSavingController.RuntimeState;

[System.Serializable]
struct TransitionData
{
    public Vector3 pos;
    public AnimatedDoor door;
}

public class WorldLoaderHelper : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] StateSavingController.TransitionType[] dataKey;
    [SerializeField] TransitionData[] dataValue;
    Dictionary<StateSavingController.TransitionType, TransitionData> data;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (dataKey.Length != dataKey.Length) {
            Debug.LogError("key and value len are not equal!");
            return;
        }
        data = new Dictionary<StateSavingController.TransitionType, TransitionData>();
        for(int i = 0; i < dataKey.Length; i++)
        {
            data.Add(dataKey[i], dataValue[i]);
        }

        Cursor.lockState = CursorLockMode.Confined;

        if (RS.transitionType == StateSavingController.TransitionType.FIRST_TIME) return;

        Debug.Log("Starting outside World");
        TransitionData transData = data[RS.transitionType];
        player.transform.position = transData.pos;
        Vector3 camPos = transData.pos;
        camPos.z = -10;
        Camera.main.gameObject.transform.position = camPos;
        PlayerHouseAnimation pha = gameObject.AddComponent<PlayerHouseAnimation>();
        Debug.Log("Created component");
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        transData.door.opposite = false;
        pha.StartRightAway = true;
        pha.player = pm;
        pha.door = transData.door;
    }
}
