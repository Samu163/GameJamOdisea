using UnityEngine;

/// <summary>
/// Controla las animaciones del jugador basándose en su estado de movimiento y alargar
/// SOPORTA 3 ANIMATORS SIMULTÁNEOS - Todos se sincronizan automáticamente
/// NUEVO: Fuerza animación a Idle durante el estiramiento
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Multiple Animators (Assign 1-3)")]
    [SerializeField] private Animator animator1;
    [Tooltip("Primer personaje/modelo (requerido)")]
    
    [SerializeField] private Animator animator2;
    [Tooltip("Segundo personaje/modelo (opcional)")]
    
    [SerializeField] private Animator animator3;
    [Tooltip("Tercer personaje/modelo (opcional)")]
    
    [Header("Components")]
    private PlayerMovement playerMovement;
    private PlayerAlargar playerAlargar;
    
    [Header("Animation Settings")]
    [SerializeField] private float runSpeedThreshold = 0.5f;
    [Tooltip("Velocidad mínima para considerar que está corriendo")]
    
    [SerializeField] private float speedTransitionSpeed = 5f;
    [Tooltip("Velocidad de transición del parámetro Speed")]
    
    [Header("Stretch Animation Reset")]
    [SerializeField] private bool forceIdleDuringStretch = true;
    [Tooltip("Si está activo, fuerza la animación a Idle durante el estiramiento")]
    
    [Header("Sync Settings")]
    [SerializeField] private bool syncAllAnimators = true;
    [Tooltip("Si está activo, todos los animators se sincronizan. Si no, solo se usa animator1")]
    
    [Header("Debug (Editor Only)")]
    [SerializeField] private bool showDebugInfo = false;
    [Tooltip("Activa para ver info de debug en pantalla (solo en Editor)")]
    
    // Animation Parameter Names (hashes para mejor performance)
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsAlargarHeld = Animator.StringToHash("IsAlargarHeld");
    private static readonly int IniciarAlargar = Animator.StringToHash("IniciarAlargar");
    private static readonly int TerminarAlargar = Animator.StringToHash("TerminarAlargar");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int Bark = Animator.StringToHash("Bark");
    
    // Estado interno
    private bool wasAlargaring = false;
    private float currentSpeed = 0f;
    
    // Array de animators activos para fácil iteración
    private Animator[] activeAnimators;
    
    // NUEVO: Control de pausa de animaciones
    private bool areAnimationsFrozen = false;
    
    private void Awake()
    {
        // Obtener referencias
        CollectAnimators();
        
        playerMovement = GetComponent<PlayerMovement>();
        playerAlargar = GetComponent<PlayerAlargar>();
        
        ValidateComponents();
    }
    
    /// <summary>
    /// Recolecta todos los animators asignados en un array
    /// </summary>
    private void CollectAnimators()
    {
        // Si no hay animator1 asignado, intentar encontrar uno
        if (animator1 == null)
        {
            animator1 = GetComponentInChildren<Animator>();
            
            if (animator1 == null)
            {
                Debug.LogError($"No se encontró ningún Animator en {gameObject.name}. Por favor asigna al menos animator1 en el Inspector.");
            }
        }
        
        // Crear array solo con los animators que están asignados
        int count = 0;
        if (animator1 != null) count++;
        if (animator2 != null && syncAllAnimators) count++;
        if (animator3 != null && syncAllAnimators) count++;
        
        activeAnimators = new Animator[count];
        int index = 0;
        
        if (animator1 != null) activeAnimators[index++] = animator1;
        if (animator2 != null && syncAllAnimators) activeAnimators[index++] = animator2;
        if (animator3 != null && syncAllAnimators) activeAnimators[index++] = animator3;
        
        Debug.Log($"✅ {gameObject.name}: {activeAnimators.Length} animator(s) configurado(s)");
    }
    
    private void ValidateComponents()
    {
        if (playerMovement == null)
        {
            Debug.LogError($"PlayerMovement no encontrado en {gameObject.name}");
        }
        
        if (playerAlargar == null)
        {
            Debug.LogError($"PlayerAlargar no encontrado en {gameObject.name}");
        }
        
        if (activeAnimators == null || activeAnimators.Length == 0)
        {
            Debug.LogError($"No hay animators activos en {gameObject.name}");
        }
    }
    
    private void Update()
    {
        if (activeAnimators == null || activeAnimators.Length == 0) return;
        
        // NUEVO: Controlar reset a Idle durante estiramiento
        if (forceIdleDuringStretch)
        {
            UpdateIdleForce();
        }
        
        UpdateSpeedParameter();
        UpdateAlargarState();
        UpdateJumpState();
    }
    
    #region Idle Force Control
    
    /// <summary>
    /// NUEVO: Fuerza la animación a Idle durante el estiramiento
    /// </summary>
    private void UpdateIdleForce()
    {
        if (playerAlargar == null) return;
        
        bool shouldForceIdle = playerAlargar.isAlargarHeld;
        
        // Solo actualizar si hay cambio de estado
        if (shouldForceIdle != areAnimationsFrozen)
        {
            if (shouldForceIdle)
            {
                ForceIdleAnimation();
            }
            else
            {
                ReleaseIdleForce();
            }
            
            areAnimationsFrozen = shouldForceIdle;
        }
    }
    
    /// <summary>
    /// NUEVO: Fuerza todas las animaciones a volver a Idle (Speed = 0)
    /// </summary>
    private void ForceIdleAnimation()
    {
        Debug.Log("⏸️ Forzando animación a Idle durante estiramiento");
        
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                // Forzar Speed a 0 para que vuelva a Idle
                anim.SetFloat(Speed, 0f);
            }
        }
    }
    
    /// <summary>
    /// NUEVO: Libera el control de Idle y permite animaciones normales
    /// </summary>
    private void ReleaseIdleForce()
    {
        Debug.Log("▶️ Liberando control de Idle - animaciones normales");
        // No hace nada aquí porque UpdateSpeedParameter se encargará de actualizar
    }
    
    #endregion
    
    #region Speed & Movement
    
    /// <summary>
    /// Actualiza el parámetro de velocidad en TODOS los animators basándose en el movimiento real
    /// </summary>
    private void UpdateSpeedParameter()
    {
        if (playerMovement == null) return;
        
        // NUEVO: Si está forzando Idle, no actualizar Speed
        if (forceIdleDuringStretch && playerAlargar != null && playerAlargar.isAlargarHeld)
        {
            return;
        }
        
        float targetSpeed = playerMovement.GetCurrentSpeed();
        
        // Suavizar la transición de velocidad
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedTransitionSpeed);
        
        // Actualizar TODOS los animators simultáneamente
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                anim.SetFloat(Speed, currentSpeed);
            }
        }
    }
    
    /// <summary>
    /// Verifica si el jugador está corriendo
    /// </summary>
    public bool IsRunning()
    {
        return currentSpeed >= runSpeedThreshold;
    }
    
    #endregion
    
    #region Alargar System
    
    /// <summary>
    /// Actualiza los estados de alargar en TODOS los animators
    /// </summary>
    private void UpdateAlargarState()
    {
        if (playerAlargar == null) return;
        
        bool isCurrentlyAlargarHeld = playerAlargar.isAlargarHeld;
        
        // Detectar inicio de alargar
        if (isCurrentlyAlargarHeld && !wasAlargaring)
        {
            OnStartAlargar();
        }
        // Detectar fin de alargar
        else if (!isCurrentlyAlargarHeld && wasAlargaring)
        {
            OnEndAlargar();
        }
        
        // Actualizar estado continuo en TODOS los animators
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                anim.SetBool(IsAlargarHeld, isCurrentlyAlargarHeld);
            }
        }
        
        wasAlargaring = isCurrentlyAlargarHeld;
    }
    
    /// <summary>
    /// Llamado cuando empieza a alargar - sincroniza TODOS los animators
    /// </summary>
    private void OnStartAlargar()
    {
        Debug.Log("🎬 Animación: Iniciar Alargar (x" + activeAnimators.Length + " personajes)");
        
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                anim.SetTrigger(IniciarAlargar);
            }
        }
    }
    
    /// <summary>
    /// Llamado cuando termina de alargar - sincroniza TODOS los animators
    /// </summary>
    private void OnEndAlargar()
    {
        Debug.Log("🎬 Animación: Terminar Alargar (x" + activeAnimators.Length + " personajes)");
        
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                anim.SetTrigger(TerminarAlargar);
            }
        }
    }
    
    #endregion
    
    #region Jump System
    
    /// <summary>
    /// Actualiza el estado de salto en TODOS los animators
    /// </summary>
    private void UpdateJumpState()
    {
        if (playerAlargar == null) return;
        
        bool isJumping = playerAlargar.isJumping;
        
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                anim.SetBool(IsJumping, isJumping);
            }
        }
    }
    
    #endregion
    
    #region Public API - Bark
    
    /// <summary>
    /// Reproduce la animación de Bark en TODOS los personajes
    /// Llama esto desde un script de Input o desde otro sistema
    /// </summary>
    public void PlayBark()
    {
        if (activeAnimators == null || activeAnimators.Length == 0) return;
        
        Debug.Log("🐕 Animación: Bark! (x" + activeAnimators.Length + " personajes)");
        
        foreach (Animator anim in activeAnimators)
        {
            if (anim != null)
            {
                anim.SetTrigger(Bark);
            }
        }
    }
    
    #endregion
    
    #region Debug Utilities
    
    /// <summary>
    /// Información de debug en el Inspector
    /// </summary>
    private void OnGUI()
    {
        if (!Application.isPlaying || activeAnimators == null || activeAnimators.Length == 0) return;
        if (!showDebugInfo) return; // Solo si está activado en el Inspector
        
        #if UNITY_EDITOR
        GUILayout.BeginArea(new Rect(10, 10, 350, 300));
        GUILayout.Box("=== Player Animation Debug ===");
        GUILayout.Label($"Active Animators: {activeAnimators.Length}");
        GUILayout.Label($"Speed: {currentSpeed:F2}");
        GUILayout.Label($"IsRunning: {IsRunning()}");
        GUILayout.Label($"IsAlargarHeld: {playerAlargar?.isAlargarHeld}");
        GUILayout.Label($"IsJumping: {playerAlargar?.isJumping}");
        
        GUILayout.Space(5);
        GUILayout.Label($"⏸️ Forcing Idle: {areAnimationsFrozen}");
        
        GUILayout.Space(10);
        GUILayout.Label("--- Animators Status ---");
        for (int i = 0; i < activeAnimators.Length; i++)
        {
            if (activeAnimators[i] != null)
            {
                string state = activeAnimators[i].GetCurrentAnimatorStateInfo(0).IsName("Idle") ? "Idle" :
                               activeAnimators[i].GetCurrentAnimatorStateInfo(0).IsName("Run") ? "Run" :
                               activeAnimators[i].GetCurrentAnimatorStateInfo(0).IsName("Alargar") ? "Alargar" : "Other";
                float speedParam = activeAnimators[i].GetFloat(Speed);
                GUILayout.Label($"Animator {i + 1}: {state} (Speed param: {speedParam:F2})");
            }
        }
        
        if (GUILayout.Button("Test Bark (All)"))
        {
            PlayBark();
        }
        
        GUILayout.EndArea();
        #endif
    }
    
    #endregion
}
