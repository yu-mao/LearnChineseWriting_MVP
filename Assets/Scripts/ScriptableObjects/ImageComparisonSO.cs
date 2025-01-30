using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;
using static ImageComparison;

[CreateAssetMenu(fileName = "ImageComparisonSO", menuName = "ScriptableObjects/ImageComparisonSO")]
public class ImageComparisonSO : ScriptableObject
{
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private Califitation califitation;

    public void ComparImagesIC()
    {
        califitation = ImageComparison.instance.CompareImages();
    }

    public void SetImageIC(Image img)
    {
        ImageComparison.instance.image = img;
    }

    public void SetRawImageIC(Transform location)
    {
        GameObject go = Instantiate(canvasPrefab, location.position, Quaternion.identity);
        ImageComparison.instance.rawImage = go.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>();
    }

    public Califitation GetCalification()
    {
        return califitation;
    }

}
