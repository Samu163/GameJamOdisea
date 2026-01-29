using UnityEngine;

public class AnimationTriggerRelay : MonoBehaviour
{
    public Animator targetAnimator;
    public string triggerName;
    public bool animation = false;
    // This is called by the animation event
    public void TriggerTargetAnimation()
    {
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(triggerName);
        }
    }
    private void Update()
    {
        if (animation == true)
        {
            TriggerTargetAnimation();

        }
    }
}
