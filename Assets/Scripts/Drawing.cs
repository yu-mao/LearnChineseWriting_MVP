using UnityEngine;
using System.Collections;

public class Drawing : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.05f;
    public GameObject pointPrefab;
    public float delayTime = 0.2f;
    public LayerMask drawLayer;
    public LayerMask SphereLayer;
    public LayerMask LineLayer;
    public float eraseRadius = 0.1f;

    private LineRenderer currentLine;
    private Vector3 lastPoint;
    private bool isDrawing = false;
    private bool isErasing = false;
    private bool isWaiting = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.D) && !isErasing)
        {
            if (!isDrawing && !isWaiting)
                StartCoroutine(StartDrawingWithDelay());
            else if (isDrawing)
                Draw();
        }
        else if (Input.GetKeyUp(KeyCode.D) && isDrawing)
        {
            StopDrawing();
        }

        if (Input.GetKey(KeyCode.E) && !isDrawing)
        {
            isErasing = true;
            Erase();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            isErasing = false;
        }
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
                GameObject lineObject = new GameObject("LineDraw");
                currentLine = lineObject.AddComponent<LineRenderer>();
                currentLine.material = lineMaterial;
                currentLine.startWidth = lineWidth;
                currentLine.endWidth = lineWidth;
                currentLine.positionCount = 1;
                currentLine.useWorldSpace = false;

                lineObject.layer = Mathf.RoundToInt(Mathf.Log(LineLayer.value, 2));

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

    void Erase()
    {
        if (!isErasing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if ((LineLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                LineRenderer line = hit.collider.GetComponent<LineRenderer>();
                if (line != null)
                {
                    EraseLineSegment(line, hit.point);
                }
            }

            if ((SphereLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void EraseLineSegment(LineRenderer line, Vector3 hitPoint)
    {
        int positionCount = line.positionCount;
        if (positionCount <= 1) return;

        Vector3[] positions = new Vector3[positionCount];
        line.GetPositions(positions);

        for (int i = 0; i < positionCount; i++)
        {
            if (Vector3.Distance(positions[i], hitPoint) < eraseRadius)
            {
                positions[i] = new Vector3(9999, 9999, 9999);
            }
        }

        line.SetPositions(positions);
    }

    void CreatePointPrefab(Vector3 position, Transform parent)
    {
        if (pointPrefab != null)
        {
            GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
            point.transform.SetParent(parent);
            point.layer = Mathf.RoundToInt(Mathf.Log(SphereLayer.value, 2));
        }
    }
}
