using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAlargar : MonoBehaviour
{

    private PlayerInput playerInput;

    [Header("Alargar Settings")]
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject bottom;
    [SerializeField] private float alargarAmount = 0.05f;
    [SerializeField] private float maxRange = 9f;
    public bool isAlargarHeld = false;
    [SerializeField] private float jumpPlayer2Force = 10f;

    private bool isRetracting = false;

    private float alargarTimer = 0f;
    private float alargarInterval = 0.1f;

    private float totalAlargar = 0f;

    private Vector3 initialBodyScale;
    private Vector3 targetBodyScale;

    private Vector3 initialBodyLocalPos;
    private Vector3 targetBodyPosition;

    private Vector3 initialHeadLocalPos;
    private Vector3 targetHeadPosition;

    private Vector3 initialBottomLocalPos;
    private Vector3 targetBottomPosition;

    private Rigidbody rb;

    // Posición mundial objetivo de la cabeza cuando se suelta el botón
    private Vector3 releaseHeadWorldPos;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        initialBodyScale = body.transform.localScale;
        targetBodyScale = body.transform.localScale;

        initialBodyLocalPos = body.transform.localPosition;
        targetBodyPosition = body.transform.localPosition;

        initialHeadLocalPos = head.transform.localPosition;
        targetHeadPosition = head.transform.localPosition;

        initialBottomLocalPos = bottom.transform.localPosition;
        targetBottomPosition = bottom.transform.localPosition;

        if (playerInput.playerIndex == 1)
        {
            LevelManager.instance.player1 = this.gameObject;
            LevelManager.instance.SpawnPlayersOnInitialPosition(1);
        }
        else if (playerInput.playerIndex == 2)
        {
            LevelManager.instance.player2 = this.gameObject;
            LevelManager.instance.SpawnPlayersOnInitialPosition(2);
        }
    }

    public void Update()
    {
        // Extender mientras se mantiene
        if (isAlargarHeld && totalAlargar < maxRange)
        {
            alargarTimer += Time.deltaTime;
            if (alargarTimer >= alargarInterval)
            {
                Alargar();
                alargarTimer = 0f;
            }

            body.transform.localScale = Vector3.Lerp(body.transform.localScale, targetBodyScale, 0.1f);
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, targetBodyPosition, 0.1f);
            head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, targetHeadPosition, 0.1f);
        }

        // Retracción: interpolar hacia valores iniciales y mantener la cabeza fija en el mundo
        if (isRetracting)
        {
            // Interpolar locales hacia los iniciales
            body.transform.localScale = Vector3.Lerp(body.transform.localScale, initialBodyScale, 0.1f);
            body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, initialBodyLocalPos, 0.1f);
            head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, initialHeadLocalPos, 0.1f);
            bottom.transform.localPosition = Vector3.Lerp(bottom.transform.localPosition, initialBottomLocalPos, 0.1f);

            // Después de aplicar Lerp, calcular desplazamiento necesario para mantener la cabeza en su posición mundial registrada
            Vector3 currentHeadWorld = head.transform.position;
            Vector3 delta = releaseHeadWorldPos - currentHeadWorld;
            // Desplazar la raíz para compensar movimiento de la cabeza
            if (delta.sqrMagnitude > 0f)
            {
                transform.position += delta;
            }

                // Si ya llegamos cerca de la configuración inicial, finalizar retracción
                const float eps = 0.01f;
            if (Vector3.Distance(body.transform.localScale, initialBodyScale) < eps &&
                Vector3.Distance(body.transform.localPosition, initialBodyLocalPos) < eps &&
                Vector3.Distance(head.transform.localPosition, initialHeadLocalPos) < eps)
            {
                body.transform.localScale = initialBodyScale;
                body.transform.localPosition = initialBodyLocalPos;
                head.transform.localPosition = initialHeadLocalPos;
                bottom.transform.localPosition = initialBottomLocalPos;

                targetBodyScale = initialBodyScale;
                targetBodyPosition = initialBodyLocalPos;
                targetHeadPosition = initialHeadLocalPos;
                targetBottomPosition = initialBottomLocalPos;

                isRetracting = false;
                totalAlargar = 0f;

                if (playerInput.playerIndex == 2)
                {
                    Player2JumpEndOfAlargar();
                }
            }
        }
    }

    public void Alargar()
    {

        if (playerInput.playerIndex == 1)
        {
            targetBodyScale += new Vector3(0, alargarAmount / 2, 0);
            targetBodyPosition += new Vector3(0, 0, alargarAmount / 2);
            targetHeadPosition += new Vector3(0, 0, alargarAmount);
            targetBottomPosition += new Vector3(0, 0, alargarAmount);
        }
        else if (playerInput.playerIndex == 2)
        {
            targetBodyScale += new Vector3(0, alargarAmount / 2, 0);
            targetBodyPosition += new Vector3(0, alargarAmount / 2, 0);
            targetHeadPosition += new Vector3(0, alargarAmount, 0);
            targetBottomPosition += new Vector3(0, alargarAmount, 0);
        }

        totalAlargar += alargarAmount;
    }

    public void CancelAlargar()
    {
        isAlargarHeld = false;
        isRetracting = true;
        totalAlargar = 0f;

        // Registrar la posición mundial de la cabeza en el momento de soltar
        releaseHeadWorldPos = head.transform.position;

        // Asegurar que las metas vuelvan a la posición/local inicial (empezamos la Lerp hacia ellas)
        targetBodyScale = initialBodyScale;
        targetBodyPosition = initialBodyLocalPos;
        targetHeadPosition = initialHeadLocalPos;
        targetBottomPosition = initialBottomLocalPos;
    }

    public void Player2JumpEndOfAlargar()
    {
        rb.AddForce(Vector3.up * jumpPlayer2Force, ForceMode.Impulse);
    }

}
