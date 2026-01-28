using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public List<Transform> waypoints;
    public int currentWaypointIndex = 0;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float targetSize;

    private bool isChangingPosition = false;

    [SerializeField] private float transitionTime = 2f;
    [SerializeField] private float currentLerpTime = 0f;

    private Camera mainCamera;

    private void Start()
    {

        mainCamera = GetComponent<Camera>();

        if (waypoints.Count > 0)
        {
            Transform initialWaypoint = waypoints[0];
            transform.position = initialWaypoint.position;
            transform.rotation = initialWaypoint.rotation;
        }

        currentLerpTime = 0f;
    }

    public void NextCameraPosition()
    {
        currentWaypointIndex++;
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) return;

        if (currentWaypointIndex % 2 != 0)
        {
            targetSize = 3;
        }
        else if (currentWaypointIndex % 2 == 0)
        {
            targetSize = 10;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        targetPosition = targetWaypoint.position;
        targetRotation = targetWaypoint.rotation;
        isChangingPosition = true;
        currentLerpTime = 0f;
    }

    public void ResetCameraLevel()
    {
        currentWaypointIndex = 2 * (LevelManager.instance.currentLevel - 1);
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) return;
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = targetWaypoint.position;
        transform.rotation = targetWaypoint.rotation;
    }

    public void SkipNextLevel()
    {
        currentWaypointIndex += 2;
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) return;
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        targetPosition = targetWaypoint.position;
        targetRotation = targetWaypoint.rotation;
        targetSize = 10f;
        isChangingPosition = true;
        currentLerpTime = 0f;
    }

    private void Update()
    {
        if (isChangingPosition)
        {
            // normalizar t en [0,1]
            currentLerpTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentLerpTime / transitionTime);

            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, t);

            if (t >= 1f)
            {
                isChangingPosition = false;
                currentLerpTime = 0f;
            }
        }

    }
}
