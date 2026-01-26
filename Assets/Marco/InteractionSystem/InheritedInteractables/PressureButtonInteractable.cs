using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class PressureButtonInteractable : TriggerInteractable
{
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private UnityEvent onDeactivate;
    private MeshRenderer buttonMesh;
    float defaultScaleY;

    protected void Awake()
    {
        buttonMesh = GetComponentInChildren<MeshRenderer>();
        defaultScaleY = buttonMesh.transform.localScale.y;
    }

    public override void Activate()
    {
        //if (active_state == ACTIVE_STATE.ON) return;

        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(0.2f, 0.1f).SetEase(Ease.OutExpo);

        //if (active_state == ACTIVE_STATE.ON) return;
        onActivate?.Invoke();
    }

    public override void Deactivate()
    {
        //if (active_state == ACTIVE_STATE.OFF) return;

        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(defaultScaleY, 0.8f).SetEase(Ease.OutElastic);

        //if (active_state == ACTIVE_STATE.OFF) return;
        onDeactivate?.Invoke();
    }
}