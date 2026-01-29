using UnityEngine;

public class InspectorAnimationTrigger : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public string triggerName = "Play";

    [Header("Inspector Control")]
    public bool playAnimation;

    private void OnValidate()
    {
        // Only run in Play Mode
        if (!Application.isPlaying) return;

        if (playAnimation)
        {
            animator.SetTrigger(triggerName);
            playAnimation = false; // reset so it can be clicked again
        }
    }
}
