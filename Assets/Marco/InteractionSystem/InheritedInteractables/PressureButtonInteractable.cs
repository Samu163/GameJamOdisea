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
        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(0.2f, 0.1f).SetEase(Ease.OutExpo);
        onActivate?.Invoke();
    }

    public override void Deactivate()
    {
        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(defaultScaleY, 0.8f).SetEase(Ease.OutElastic);
        onDeactivate?.Invoke();
    }
}