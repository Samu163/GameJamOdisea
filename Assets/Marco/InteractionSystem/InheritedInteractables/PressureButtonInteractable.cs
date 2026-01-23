using UnityEngine;
using DG.Tweening;

public class PressureButtonInteractable : TriggerInteractable
{
    [SerializeField] private GameObject door;
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
        door.gameObject.SetActive(false);
    }

    public override void Deactivate()
    {
        buttonMesh.transform.DOKill();
        buttonMesh.transform.DOScaleY(defaultScaleY, 0.8f).SetEase(Ease.OutElastic);
        door.gameObject.SetActive(true);
    }
}
