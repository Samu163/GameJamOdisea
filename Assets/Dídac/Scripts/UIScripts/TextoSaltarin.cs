using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TextoSaltarin : MonoBehaviour
{
    [Header("Configuración")]
    public float alturaSalto = 20f;
    public float duracionSalto = 0.5f;
    public float delayEntreLetras = 0.1f;
    public float demoraLoop = 1f;
    public Ease tipoDeMovimiento = Ease.OutQuad;

    private TMP_Text textComponent;
    private float[] saltosY; // Aquí guardamos cuánto sube cada letra

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();

        // Forzamos una actualización para asegurar que el texto existe en la malla
        textComponent.ForceMeshUpdate();

        // Preparamos el array de saltos
        int totalCaracteres = textComponent.textInfo.characterCount;
        saltosY = new float[totalCaracteres];

        StartCoroutine(StartAnimarTexto());
    }

    void Update()
    {
        ActualizarVertices();
    }

    private IEnumerator StartAnimarTexto()
    {
        yield return new WaitForSeconds(demoraLoop); // Pequeña espera para asegurar que todo está listo
        AnimarTexto();
    }

    void AnimarTexto()
    {
        // Creamos una secuencia de DOTween
        Sequence secuencia = DOTween.Sequence();

        for (int i = 0; i < saltosY.Length; i++)
        {
            int index = i; // Capturamos el índice para usarlo dentro del tween

            // Verificamos si la letra es visible (para no animar espacios en blanco)
            if (!textComponent.textInfo.characterInfo[index].isVisible) continue;

            // Animamos el valor en el array 'saltosY'
            // 1. Sube
            secuencia.Insert(index * delayEntreLetras,
                DOTween.To(() => saltosY[index], x => saltosY[index] = x, alturaSalto, duracionSalto / 2)
                .SetEase(tipoDeMovimiento));

            // 2. Baja (vuelve a 0)
            secuencia.Insert((index * delayEntreLetras) + (duracionSalto / 2),
                DOTween.To(() => saltosY[index], x => saltosY[index] = x, 0f, duracionSalto / 2)
                .SetEase(Ease.InQuad));
        }

        secuencia.AppendInterval(demoraLoop);

        secuencia.SetLoops(-1); 
    }

    void ActualizarVertices()
    {
        textComponent.ForceMeshUpdate(); // Importante para refrescar estado
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            // Obtenemos los índices de los 4 vértices que forman una letra
            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            // Obtenemos los vértices originales
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Aplicamos el salto Y a los 4 vértices de la letra
            float saltoActual = saltosY[i];

            vertices[vertexIndex + 0].y += saltoActual; // Abajo-Izquierda
            vertices[vertexIndex + 1].y += saltoActual; // Arriba-Izquierda
            vertices[vertexIndex + 2].y += saltoActual; // Arriba-Derecha
            vertices[vertexIndex + 3].y += saltoActual; // Abajo-Derecha
        }

        // Enviamos los cambios a la malla de Unity
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
