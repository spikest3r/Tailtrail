using UnityEngine;

public class SetBuildingTag : MonoBehaviour
{
    [SerializeField] StateSavingController.TransitionType transition;

    void Awake()
    {
        StateSavingController.RuntimeState.transitionType = transition;
    }
}
