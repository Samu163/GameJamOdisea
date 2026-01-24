using UnityEngine;

public class RudderMoveable : MonoBehaviour
{

    [SerializeField] Vector3 maxMove = new Vector3(0, 2, 0);
    [SerializeField] Vector3 minMove = new Vector3(0, 0, 0);

    [Tooltip("HSelect the rudder interactable on this scene that moves this platform")]
    public RudderInteractable myRudder;

    [Tooltip("How many degrees of rudder rotation does it take to move from Min to Max?")]
    [SerializeField] float rangeInDegrees = 150f;


    // 0.0 means we are at Min, 1.0 means we are at Max
    private float _currentProgress = 0f;

    private void Awake()
    {
        myRudder.onRudderTurned.AddListener(OnRudderTurned);
        transform.localPosition = minMove;
    }

    public void OnRudderTurned(float angle_difference)
    {
        // Convert the angle change into a "percentage" change
        float percentChange = angle_difference / rangeInDegrees;
        _currentProgress += percentChange;


        _currentProgress = Mathf.Clamp01(_currentProgress);
        transform.localPosition = Vector3.Lerp(minMove, maxMove, _currentProgress);
    }

}
