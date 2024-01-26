using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircle : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 10)]
    public float xradius = 1;
    [Range(0, 10)]
    public float yradius = 1;

    [Range(0, 1)]
    public float lineWidth = 0.1f;

    LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        CreatePoints();
        this.transform.Rotate(90.0f, 0, 0);    
    }

    void Update()
    {
        CreatePoints();
        //var rot = this.transform.rotation;
        //rot.x = 90.0f;
        //this.transform.rotation = rot;

    }

    void CreatePoints()
    {
        float x;
        float y;
        float z;

        float angle = 120f;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments + 1);
        }
    }
}
