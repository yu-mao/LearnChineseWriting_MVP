using UnityEngine;
using UnityEngine.UI;

public class ImageComparison : MonoBehaviour
{
    public RawImage rawImage;
    public Image image;

    private void Start()
    {
        CompareImages();
    }

    public void CompareImages()
    {
        Texture2D texture1 = (Texture2D)rawImage.texture;
        Texture2D texture2 = image.sprite.texture;

        if (!texture1.isReadable || !texture2.isReadable)
        {
            Debug.LogError("Error in readable Texture");
            return;
        }

        //Texture2D resizedTexture2 = ResizeTexture(texture2, texture1.width, texture1.height);

        Color[] pixels1 = texture1.GetPixels();
        Color[] pixels2 = texture2.GetPixels();

        int width = texture2.width;
        int height = texture2.height;
        float[,] img1 = new float[height, width];
        float[,] img2 = new float[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                img1[y, x] = pixels1[y * width + x].grayscale;
                img2[y, x] = pixels2[y * width + x].grayscale;
            }
        }

        int diffPixels = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Mathf.Abs(img1[y, x] - img2[y, x]) > 0)
                {
                    diffPixels++;
                }
            }
        }

        float totalPixels = width * height;
        float percentageDifference = (float)diffPixels / totalPixels * 100;
        float percentageSimilarity = 100 - percentageDifference;

        Debug.Log($"Porcentaje de similitud: {percentageSimilarity}%");
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        RenderTexture.active = rt;

        Graphics.Blit(originalTexture, rt);

        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight);
        resizedTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        resizedTexture.Apply();

        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.active = null;

        return resizedTexture;
    }
}
