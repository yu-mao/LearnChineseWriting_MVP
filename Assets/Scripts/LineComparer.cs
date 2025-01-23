using System.Collections.Generic;
using UnityEngine;

public enum Rating //Maybe this enum will be in another script to make it globally accesible and not only from this class... by the moment it's here hehe
{
    BAD,
    GOOD,
    VERY_GOOD
}

public class LineComparer : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask sceneLayer;
    public float tolerance = 0.1f;

    public float similarityWeight = 1f;
    public float penaltyWeight = 1f;

    [Range(0, 100)]
    public float badThreshold = 33f;
    [Range(0, 100)]
    public float goodThreshold = 66f;

    public Rating EvaluateSimilarity() // We want to return an Enum element
    {
        List<Transform> playerSpheres = GetSpheresByLayer(playerLayer);
        List<Transform> sceneSpheres = GetSpheresByLayer(sceneLayer);
        HashSet<Transform> usedSceneSpheres = new HashSet<Transform>();

        if (sceneSpheres.Count == 0) return Rating.BAD;

        int matches = 0;
        int penalties = 0;

        foreach (Transform playerSphere in playerSpheres)
        {
            bool matchFound = false;

            foreach (Transform sceneSphere in sceneSpheres)
            {
                if (usedSceneSpheres.Contains(sceneSphere))
                    continue;

                if (Vector3.Distance(sceneSphere.position, playerSphere.position) <= tolerance)
                {
                    matches++;
                    matchFound = true;
                    usedSceneSpheres.Add(sceneSphere);
                    break;
                }
            }

            if (!matchFound)
            {
                penalties++;
            }
        }

        float similarityPercentage = (float)matches / sceneSpheres.Count * 100f;
        float penaltyPercentage = penalties > 0 ? ((float)penalties / playerSpheres.Count * 100f) : 0f;

        float weightedSimilarity = similarityPercentage * similarityWeight;
        float weightedPenalty = penaltyPercentage * penaltyWeight;

        float finalScore = Mathf.Max(0f, weightedSimilarity - weightedPenalty);

        if (finalScore < badThreshold)
        {
            return Rating.BAD;
        }
        else if (finalScore < goodThreshold)
        {
            return Rating.GOOD;
        }
        else
        {
            return Rating.VERY_GOOD;
        }
    }

    private List<Transform> GetSpheresByLayer(LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(Vector3.zero, Mathf.Infinity, layerMask);
        List<Transform> spheres = new List<Transform>();

        foreach (Collider collider in colliders)
        {
            spheres.Add(collider.transform);
        }

        return spheres;
    }
}