using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    public void FinishThrowing()
    {
        Animator.SetBool("IsThrowing", false);
    }
}
