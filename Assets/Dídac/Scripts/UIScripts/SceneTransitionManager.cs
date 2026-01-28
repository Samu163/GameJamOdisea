using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;

    [Header("Configuración de la Pared")]
    public RectTransform wallRectTransform;
    public float transitionDuration = 1.0f;
    public Ease easeType = Ease.InOutQuad;
    private float initialYPosition;

    [Header("Level Transition Settings")]
    public RectTransform levelWallTransform;
    public float levelTransitionDuration = 1.0f;
    public Ease levelEaseType = Ease.InOutQuad;
    private float initialLevelWallXPosition;

    void Awake()
    {
        // Configuración del Singleton para que sobreviva entre escenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ¡La clave! Este objeto no se destruye.

            // Guardamos la posición inicial (arriba, fuera de pantalla)
            initialYPosition = wallRectTransform.anchoredPosition.y;
            initialLevelWallXPosition = levelWallTransform.anchoredPosition.x;
        }
        else
        {
            // Si ya existe uno, destruimos el duplicado
            Destroy(gameObject);
        }
    }

    // Método público para llamar desde tus botones o triggers
    public void ChangeScene(string sceneName)
    {
        GameManager.instance.currentSceneName = sceneName;
        StartCoroutine(TransitionRoutine(sceneName));
    }

    public void ResetLevelTransition(string sceneName)
    {

    }

    public void NextLevelTransition()
    {
        StartCoroutine(LevelTransitionRoutine());
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        // Bloqueamos interacciones si fuera necesario (opcional)
        // CanvasGroup.blocksRaycasts = true;

        // --- FASE 1: BAJAR LA PARED ---
        // Movemos la pared hacia la posición Y=0 (el centro de la pantalla si está anclada arriba)
        // Usamos WaitForCompletion() para que la corrutina espere a que termine la animación.
        yield return wallRectTransform.DOAnchorPosY(0, transitionDuration)
            .SetEase(easeType)
            .WaitForCompletion();

        // --- FASE 2: CARGAR LA ESCENA ---
        // Mientras la pantalla está cubierta, cargamos la escena de forma asíncrona.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Esperamos a que la escena termine de cargar completamente.
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // (Opcional) Pequeña espera extra para asegurar que todo se asentó
        yield return new WaitForSeconds(0.2f);

        // --- FASE 3: SUBIR LA PARED ---
        // Movemos la pared de vuelta a su posición inicial arriba fuera de la pantalla.
        yield return wallRectTransform.DOAnchorPosY(initialYPosition, transitionDuration)
            .SetEase(easeType)
            .WaitForCompletion();

        // Desbloqueamos interacciones
        // CanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LevelTransitionRoutine()
    {
        
        yield return levelWallTransform.DOAnchorPosX(0, levelTransitionDuration)
            .SetEase(levelEaseType)
            .WaitForCompletion();

        yield return new WaitForSeconds(0.2f);

        yield return levelWallTransform.DOAnchorPosX(initialLevelWallXPosition, levelTransitionDuration)
            .SetEase(easeType)
            .WaitForCompletion();

    }
}
