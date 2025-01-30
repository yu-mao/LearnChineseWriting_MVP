using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageComparison : MonoBehaviour
{
    public static ImageComparison instance;

    [Tooltip("Base chinese word image")] public Image image;
    [Tooltip("Drawing to compare")] public RawImage rawImage;

    [SerializeField] private int fixedWidth = 256;
    [SerializeField] private int fixedHeight = 256;

    //[SerializeField] private Califitation califitation;

    [SerializeField, Range(0, 100)] public float thresholdVeryGood;
    [SerializeField, Range(0, 100)] public float thresholdGood;
    [SerializeField, Range(0, 1)] public float grayscaleTolerance = 0.05f;
    [SerializeField, Range(0, 1)] public float ignoreBlankSpacesThreshold = 0.95f;

    private void Awake()
    {
        instance = this;
    }

        /*private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }*/

        /*private void StartFunction()
        {
            califitation = CompareImages();
            calificationGUI.text = califitation.ToString();
            Debug.Log($"Calificación obtenida: {califitation}");
        }*/

    public void SetImageToCompare(Image imagen)
    {
        image = imagen;
    }

    public void SetRawImageToCompare(RawImage rawImagen)
    {
        rawImage = rawImagen;
    }

    public Califitation CompareImages()
    {
        if (image == null || rawImage == null)
        {
            return Califitation.None;
        }

        Texture2D texture1;

        if (rawImage.texture is RenderTexture renderTexture)
        {
            texture1 = ConvertRenderTextureToTexture2D(renderTexture);
        }
        else if (rawImage.texture is Texture2D tex2D)
        {
            texture1 = tex2D;
        }
        else
        {
            return Califitation.None;
        }

        Texture2D texture2 = image.sprite.texture;

        if (!texture1.isReadable || !texture2.isReadable)
        {
            return Califitation.None;
        }

        Texture2D resizedTexture1 = ResizeTexture(texture1, fixedWidth, fixedHeight);
        Texture2D resizedTexture2 = ResizeTexture(texture2, fixedWidth, fixedHeight);

        //SaveGrayscaleData(resizedTexture1, "/debug_texture1_grayscale.txt");
        //SaveGrayscaleData(resizedTexture2, "/debug_texture2_grayscale.txt");

        Color[] pixels1 = resizedTexture1.GetPixels();
        Color[] pixels2 = resizedTexture2.GetPixels();

        int width = fixedWidth;
        int height = fixedHeight;
        int correctPixels = 0;
        int incorrectPixels = 0;
        int totalRelevantPixels = 0;

        for (int i = 0; i < pixels1.Length; i++)
        {
            float gray1 = pixels1[i].grayscale;
            float gray2 = pixels2[i].grayscale;

            if (gray1 > ignoreBlankSpacesThreshold || gray2 > ignoreBlankSpacesThreshold)
            {
                continue;
            }

            totalRelevantPixels++;

            if (gray2 < grayscaleTolerance && gray1 < grayscaleTolerance)
            {
                correctPixels++;
            }
            else if (gray2 > grayscaleTolerance && gray1 < grayscaleTolerance)
            {
                incorrectPixels++;
            }
        }

        if (totalRelevantPixels == 0)
        {
            return Califitation.BAD;
        }

        float percentageCorrect = (correctPixels / (float)totalRelevantPixels) * 100;
        float percentageIncorrect = (incorrectPixels / (float)totalRelevantPixels) * 100;

        float percentageSimilarity = percentageCorrect - percentageIncorrect;

        Debug.Log($"Percentage of similarity: {percentageSimilarity}%");

        image = null;
        rawImage.texture = null;
        rawImage = null;

        if (percentageSimilarity >= thresholdVeryGood)
            return Califitation.VERY_GOOD;
        else if (percentageSimilarity >= thresholdGood)
            return Califitation.GOOD;
        else
            return Califitation.BAD;
    }

    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        return texture;
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

    private void SaveGrayscaleData(Texture2D texture, string filename) //this function is only for grayscale comparison and verification in pc
    {
        string filePath = Application.persistentDataPath + filename;
        System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);

        Color[] pixels = texture.GetPixels();

        foreach (var pixel in pixels)
        {
            float grayScale = pixel.grayscale;
            writer.WriteLine(grayScale);
        }

        writer.Close();
        Debug.Log("Escalas de grises guardadas en: " + filePath);
    }
}

[Serializable]
public enum Califitation
{
    None,
    BAD,
    GOOD,
    VERY_GOOD
}
