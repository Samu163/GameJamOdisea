using System.ComponentModel;
using UnityEngine;
using System.Collections;
using UnityEngine.Animations;
using DG.Tweening;

public class BoxInteractable : InputInteractable
{
    GridObject gridComponent;
    ObjectConstraint objectConstraint;

    [Description("Object with interactor collider enabling this interactable to acts as interactor for other interactables")]
    public Collider my_interactor;

    [Description("Collider used to calculate the box bounds for carrying purposes")]
    public Collider meshCollider;

    [Header("Drop Animation")]
    [Description("How long the toss takes")]
    public float dropDuration = 0.5f;
    [Description("How high the box jumps when dropped")]
    public float arcHeight = 1.0f;
    [Description("The curve that defines the shape of the jump arc")]
    public AnimationCurve arcCurve;
    [Description("The curve that defines the squash/stretch effect on landing")]
    public AnimationCurve scaleCurve;

    protected override void Awake()
    {
        base.Awake();
        gridComponent = GetComponent<GridObject>();
        objectConstraint = GetComponent<ObjectConstraint>();
    }

    public override void Interact(Interactor interactor)
    { // Received the interactor from the player who interacted with self


        // Change player's state to carrying state passing this box
        interactor.playerStateMachine.ChangeState(interactor.playerStateMachine.CurrentState,
            "CarryingState", new System.Collections.Generic.Dictionary<string, object> { { "box", this } });

        base.Interact(interactor);
        Deactivate(); // Interactability is disabled because the player is carrying it
        // TODO: should also disable the interactor so that you can't interact with other things while carrying
    }

    // Called by the CarryingState when the box is grabbed
    public void Grab(Transform grabPoint)
    {
        StartCoroutine(GrabRoutine(grabPoint));
        my_interactor.enabled = false; // Disable the interactor collider while being carried
    }


    public void FakeParent(Transform grabPoint) {
        objectConstraint.FakeParent(this.gameObject, grabPoint.gameObject);
        
    }

    float grabTime = 0.4f;

    private IEnumerator GrabRoutine(Transform grabPoint)
    {
        float elapsed = 0f;

        while (elapsed < grabTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / grabTime; // 0 to 1 value

            Vector3 lpos = Vector3.Lerp(transform.position, grabPoint.position, t);
            Vector3 lrot = Vector3.Lerp(transform.rotation.eulerAngles, grabPoint.rotation.eulerAngles, t);

            transform.position = lpos;
            transform.rotation = Quaternion.Euler(lrot);

            yield return null;
        }

        FakeParent(grabPoint);

    }

    public void Drop(Interactor interactor)
    {
        objectConstraint.StopFollowing(this.gameObject);

        // Calculate Start and End points
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 targetPos = gridComponent.GetSnappedPosition();

        // Calculate target rotation
        float snappedY = Mathf.Round(transform.eulerAngles.y / 90.0f) * 90.0f;
        Quaternion targetRot = Quaternion.Euler(0, snappedY, 0);

        // Start the Animation Routine
        StartCoroutine(AnimateToss(startPos, targetPos, startRot, targetRot));

        my_interactor.enabled = true; // Re-enable the interactor collider

        //Re-enable interaction
        Activate();
    }

    private IEnumerator AnimateToss(Vector3 start, Vector3 end, Quaternion startRot, Quaternion endRot)
    {
        float elapsed = 0f;

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropDuration; // 0 to 1 value

            // Lerp linearly from A to B
            Vector3 linearPos = Vector3.Lerp(start, end, t);

            // height is obtained through the curve * arcHeight
            float height = arcCurve.Evaluate(t) * arcHeight;

            transform.position = new Vector3(linearPos.x, linearPos.y + height, linearPos.z);

            // --- ROTATION ---
            // Smoothly rotate to the nice 90-degree angle
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null; // Wait for next frame
        }

        // --- THE LANDING (Bounce/Squash) ---
        // Force exact position/rotation at the end to prevent drift
        transform.position = end;
        transform.rotation = endRot;

        // Play a quick "Wobble" effect
        yield return StartCoroutine(AnimateSquash());
    }

    private System.Collections.IEnumerator AnimateSquash()
    {
        float elapsed = 0f;
        float squashDuration = 0.3f;

        while (elapsed < squashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / squashDuration;

            // Evaluate the curve to get the current height scale
            float height = scaleCurve.Evaluate(t);

            // Squash and strethc : Width = 1 / Height
            float width = 1f / Mathf.Clamp(height, 0.1f, 10f); // Clamp prevents divide-by-zero errors

            transform.localScale = new Vector3(width, height, width);

            yield return null;
        }

        // Reset to perfect one at the end
        transform.localScale = Vector3.one;
    }
}