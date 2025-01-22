using System.Collections.Generic;
using UnityEngine;

public class LineComparer : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask sceneLayer;
    public float tolerance = 0.1f;

    public float similarityWeight = 1f;
    public float penaltyWeight = 1f;

    public void EvaluateSimilarity()
    {
        List<Transform> playerSpheres = GetSpheresByLayer(playerLayer);
        List<Transform> sceneSpheres = GetSpheresByLayer(sceneLayer);
        HashSet<Transform> usedSceneSpheres = new HashSet<Transform>();

        if (sceneSpheres.Count == 0) return;

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

        Debug.Log($"Similitud: {weightedSimilarity}% (Penalizacion: {weightedPenalty}%) - Puntaje final: {finalScore}%");
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
