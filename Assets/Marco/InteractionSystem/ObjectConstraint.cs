using UnityEngine;
using UnityEngine.Animations;

public class ObjectConstraint : MonoBehaviour
{
    public void FakeParent(GameObject child, GameObject targetParent)
    {
        // Add or Get the ParentConstraint component
        ParentConstraint constraint = child.GetComponent<ParentConstraint>();
        if (constraint == null)
            constraint = child.AddComponent<ParentConstraint>();

        // Create a Constraint Source
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = targetParent.transform;
        source.weight = 1.0f; // 100% influence
        constraint.SetSources(new System.Collections.Generic.List<ConstraintSource>() { source });

        //Configure constraint
        constraint.translationAtRest = Vector3.zero;
        constraint.rotationAtRest = Vector3.zero;

        // Set the offset 
        constraint.SetTranslationOffset(0, child.transform.position - targetParent.transform.position);
        constraint.SetRotationOffset(0, (Quaternion.Inverse(targetParent.transform.rotation) * child.transform.rotation).eulerAngles);

        constraint.constraintActive = true;
    }

    public void StopFollowing(GameObject child)
    {
        ParentConstraint constraint = child.GetComponent<ParentConstraint>();
        if (constraint != null)
        {
            // Disable constraint
            constraint.constraintActive = false;
        }
    }
}