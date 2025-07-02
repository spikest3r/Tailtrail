using UnityEngine;

public class StateInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject); // make this object our state machine
        Destroy(this); // remove this component because its used only for init
    }
}
