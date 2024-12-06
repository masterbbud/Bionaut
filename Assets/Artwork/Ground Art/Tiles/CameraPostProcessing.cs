using UnityEngine;

[ExecuteInEditMode]
public class CameraPostProcessing : MonoBehaviour
{
    public Material postProcessingMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessingMaterial != null)
        {
            Graphics.Blit(source, destination, postProcessingMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
