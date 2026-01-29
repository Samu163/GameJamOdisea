using UnityEngine;
using TMPro;

public class SquiggleText : MonoBehaviour
{
    public float scale = 10f;
    public float strength = 2f;
    public float fps = 6f; // Low FPS for that "hand-drawn" feel

    private TMP_Text textComponent;
    private float timer;
    private float stepTime;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // 1. Manage the "Stop Motion" FPS
        timer += Time.deltaTime;
        if (timer < 1f / fps) return; // Wait for next frame
        timer = 0;

        // Update the "stepped" time for the noise
        stepTime += 0.1f;

        // 2. Force the mesh to update so we have fresh vertices
        textComponent.ForceMeshUpdate();

        var textInfo = textComponent.textInfo;

        // 3. Loop through every character
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            // Skip invisible characters (spaces)
            if (!charInfo.isVisible) continue;

            // Get the index of the vertices for this specific letter
            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            // Get the vertices array
            Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

            // 4. Calculate a unique offset for this character
            // We use the character's position so every letter wiggles differently
            Vector3 center = (sourceVertices[vertexIndex] + sourceVertices[vertexIndex + 2]) / 2;

            float noiseX = Mathf.PerlinNoise(center.x * 0.1f + stepTime, center.y * 0.1f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(center.x * 0.1f, center.y * 0.1f + stepTime) - 0.5f;

            Vector3 jitter = new Vector3(noiseX, noiseY, 0) * strength;

            // 5. Apply the jitter to all 4 corners of the letter
            sourceVertices[vertexIndex + 0] += jitter;
            sourceVertices[vertexIndex + 1] += jitter;
            sourceVertices[vertexIndex + 2] += jitter;
            sourceVertices[vertexIndex + 3] += jitter;
        }

        // 6. Push changes back to the mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}