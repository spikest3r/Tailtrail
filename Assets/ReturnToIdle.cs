using UnityEngine;

public class ReturnToIdle : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Farm");
        animator.SetBool("Idle", true);
        animator.SetTrigger("Change");
    }
}
