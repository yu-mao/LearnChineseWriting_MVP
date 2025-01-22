using UnityEngine;
using System.Collections;

public class Drawing : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.05f;
    public GameObject pointPrefab;
    private LineRenderer currentLine;
    private Vector3 lastPoint;
    private bool isDrawing = false;
    private bool isWaiting = false;
    public float delayTime = 0.2f;
    public LayerMask drawLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isWaiting)
            StartCoroutine(StartDrawingWithDelay());
        else if (Input.GetMouseButton(0) && isDrawing)
            Draw();
        else if (Input.GetMouseButtonUp(0) && isDrawing)
            StopDrawing();
    }

    private IEnumerator StartDrawingWithDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(delayTime);
        StartDrawing();
        isWaiting = false;
    }

    void StartDrawing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, drawLayer))
        {
            if (hit.collider != null)
            {
                if (currentLine != null)
                {
                    Destroy(currentLine.gameObject);
                }

                isDrawing = true;
                GameObject lineObject = new GameObject("Line");
                currentLine = lineObject.AddComponent<LineRenderer>();
                currentLine.material = lineMaterial;
                currentLine.startWidth = lineWidth;
                currentLine.endWidth = lineWidth;
                currentLine.positionCount = 1;
                currentLine.useWorldSpace = false;

                lastPoint = hit.point;
                currentLine.SetPosition(0, lastPoint);

                CreatePointPrefab(hit.point, lineObject.transform);
            }
        }
    }

    void Draw()
    {
        if (!isDrawing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, drawLayer))
        {
            Vector3 currentPoint = hit.point;

            if (Vector3.Distance(lastPoint, currentPoint) > 0.01f)
            {
                lastPoint = currentPoint;
                currentLine.positionCount++;
                currentLine.SetPosition(currentLine.positionCount - 1, currentPoint);

                CreatePointPrefab(currentPoint, currentLine.transform);
            }
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
        currentLine = null;
    }

    void CreatePointPrefab(Vector3 position, Transform parent)
    {
        if (pointPrefab != null)
        {
            GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
            point.transform.SetParent(parent);
        }
    }
}
