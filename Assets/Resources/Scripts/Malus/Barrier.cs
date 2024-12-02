using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Animator barrierAnim;
    public BoxCollider2D barrierCollider;

    private void Update()
    {
        ColliderState();
    }

    private void ColliderState()
    {
        if (barrierAnim.GetCurrentAnimatorStateInfo(0).IsName("No Active"))
        {
            barrierCollider.enabled = false;
        }
        else if (barrierAnim.GetCurrentAnimatorStateInfo(0).IsName("Active"))
        {
            barrierCollider.enabled = true;
        }
        return;
    }
}
