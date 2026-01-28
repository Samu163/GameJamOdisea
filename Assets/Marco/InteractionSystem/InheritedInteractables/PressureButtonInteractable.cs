using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class PressureButtonInteractable : TriggerInteractable
{
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private UnityEvent onDeactivate;
    [SerializeField] public  MeshRenderer buttonMesh;
    float defaultScaleY;

    protected void Awake()
    {
        defaultScaleY = buttonMesh.transform.localScale.y;
    }

    public override void Activate()
    {
        if (active_state == ACTIVE_STATE.ON) return;

        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(0.2f, 0.1f).SetEase(Ease.OutExpo);

        base.Activate();
        onActivate?.Invoke();
    }

    public override void Deactivate()
    {
        if (active_state == ACTIVE_STATE.OFF) return;

        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(defaultScaleY, 0.8f).SetEase(Ease.OutElastic);

        base.Deactivate();
        onDeactivate?.Invoke();
    }

    public override bool IsActive()
    {
        return active_state == ACTIVE_STATE.ON;
    }
}