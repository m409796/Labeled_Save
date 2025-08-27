using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public LineRenderer line;
    [SerializeField]public Transform[] points;


    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    public void SetUpLine(Transform[] points)
    {
        line.positionCount = points.Length;
        this.points = points;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetUpLine(points);
    }
    
    // Update is called once per frame
    void Update()
    {
        for(int i=0; i < points.Length; i++)
        {
            line.SetPosition(i, points[i].position);
        }
    }


    void OnDrawGizmos()
    {
        if (points == null || points.Length < 2)
            return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (points[i] != null && points[i + 1] != null)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }


}
