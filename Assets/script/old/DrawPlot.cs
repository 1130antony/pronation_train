using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPlot : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3 position;
    int index = 0;
    int lenthOfLineRender = 0;
    List<Vector3> drawPoint = new List<Vector3>();
    
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.SetWidth(0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        while (index < lenthOfLineRender)
        {
            lineRenderer.SetPosition(index, drawPoint[index]);
            index++;
        }
    }

    public void AddPoint(float x, float y)
    {
        Vector3 point = new Vector3(x, 0f, y);

        drawPoint.Add(point);
        lenthOfLineRender++;
        lineRenderer.SetVertexCount(lenthOfLineRender);
    }

    public void ResetPlot()
    {
        drawPoint.Clear();
        lineRenderer.positionCount = 0;
        lenthOfLineRender = 0;
        index = 0;
    }
}
