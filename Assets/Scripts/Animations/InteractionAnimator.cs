using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionAnimator : MonoBehaviour
{
    [Header("--- Configuración General ---")]
    [Tooltip("Check ON: Sube y se queda fijo. Check OFF: Si sueltas, baja.")]
    [SerializeField] private bool isPermanent = true;

    [Tooltip("Número de placas necesarias simultáneamente")]
    [SerializeField] private int numberOfInteractables = 2;

    [Header("--- Configuración de Bloques ---")]
    [SerializeField] private List<Transform> bridgeBlocks;

    [Header("--- Configuración de Animación ---")]
    [SerializeField] private float targetY = 0.5f;
    [SerializeField] private float startY = -5.0f;
    [SerializeField] private float riseSpeed = 3.0f;
    [SerializeField] private float delayBetweenPairs = 0.5f;

    private HashSet<int> activePlates = new HashSet<int>();
    private bool bridgeCompleted = false;

    private bool isAnimationCompleted = false;
    private bool isLastBlock = false;

    private void Start()
    {
        foreach (Transform block in bridgeBlocks)
        {
            if (block != null) block.position = new Vector3(block.position.x, startY, block.position.z);
        }
    }


    // ---------------------------------------------------------
    // EVENTOS
    // ---------------------------------------------------------

    public void ActivatePlate(int plateID)
    {
        if (isPermanent && bridgeCompleted) return;

        if (!activePlates.Contains(plateID))
        {
            activePlates.Add(plateID);
            CheckLogic();
        }
    }

    public void DeactivatePlate(int plateID)
    {
        if (isPermanent && bridgeCompleted) return;

        if (activePlates.Contains(plateID))
        {
            activePlates.Remove(plateID);
            CheckLogic();
        }
    }

    // ---------------------------------------------------------
    // LÓGICA
    // ---------------------------------------------------------

    private void CheckLogic()
    {
        bool conditionMet = activePlates.Count >= numberOfInteractables;

        if (isPermanent)
        {
            // MODO PERMANENTE
            if (conditionMet && !bridgeCompleted)
            {
                bridgeCompleted = true;
                StopAllCoroutines();
                StartCoroutine(BuildSequence());
                AudioManager.instance.PlayPuente();
            }
        }
        else
        {
            // MODO REVERSIBLE (SUBE / BAJA)
            StopAllCoroutines(); 

            if (conditionMet)
            {
                StartCoroutine(BuildSequence());
                AudioManager.instance.PlayPuente();
            }
            else
            {
                StartCoroutine(CollapseSequence());
                AudioManager.instance.PlayPuente();
            }
        }
    }

    // ---------------------------------------------------------
    // SECUENCIAS
    // ---------------------------------------------------------

    private IEnumerator BuildSequence()
    {
        for (int i = 0; i < bridgeBlocks.Count; i += 2)
        {

            if (i == bridgeBlocks.Count - 1)
            {
                isLastBlock = true;
            }

            MovePair(i, targetY, isLastBlock);

            

            yield return new WaitForSeconds(delayBetweenPairs);
        }

        
    }

    private IEnumerator CollapseSequence()
    {
        for (int i = bridgeBlocks.Count - 1; i >= 0; i -= 2)
        {
            bool blocksNeedReset = HasMovedFromStart(i) || HasMovedFromStart(i - 1);

            if (!blocksNeedReset)
            {
                continue;
            }

            if (i <= 0)
            {
                isLastBlock = true;
            }

            MovePair(i, startY, isLastBlock, usePrevIndex: true);

            
            yield return new WaitForSeconds(delayBetweenPairs);
        }

       
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    private void MovePair(int index, float destY, bool isLast, bool usePrevIndex = false)
    {
        int secondIndex = usePrevIndex ? index - 1 : index + 1;

        if (IsValidBlock(index))
            StartCoroutine(MoveBlock(bridgeBlocks[index], destY, isLast));

        if (IsValidBlock(secondIndex))
            StartCoroutine(MoveBlock(bridgeBlocks[secondIndex], destY, isLast));
    }
    private bool HasMovedFromStart(int index)
    {
        if (!IsValidBlock(index)) return false;

        return Mathf.Abs(bridgeBlocks[index].position.y - startY) > 0.05f;
    }
    private bool IsValidBlock(int index)
    {
        return index >= 0 && index < bridgeBlocks.Count && bridgeBlocks[index] != null;
    }

    private bool IsBlockUp(int index)
    {
        if (!IsValidBlock(index)) return false;
        return bridgeBlocks[index].position.y > (startY + 0.05f);
    }

    private IEnumerator MoveBlock(Transform block, float target, bool isLast)
    {
        while (Mathf.Abs(block.position.y - target) > 0.01f)
        {
            float step = riseSpeed * Time.deltaTime;
            float newY = Mathf.MoveTowards(block.position.y, target, step);
            block.position = new Vector3(block.position.x, newY, block.position.z);
            yield return null;
        }
        block.position = new Vector3(block.position.x, target, block.position.z);

        if (isLast)
        {
            AudioManager.instance.StopPuente();
        }
    }
}