using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JusticeUser : MonoBehaviour
{
    public float m_DistanceThreshold = 2;

    public int m_RingCount = 2;
    public int m_RayDensity = 10;
    public float m_Radius = 1.0f;

    [ColorUsage(true, true)]
    public Color m_LitColor = Color.yellow;

    private JusticeGuard[] m_JusticeGuards;
    private Color[] m_MeshColors;

    private List<int> m_ColoredTriangles;

    // Start is called before the first frame update
    void Awake()
    {
        m_JusticeGuards = FindObjectsOfType<JusticeGuard>();
    }

    private void Start()
    {
        m_ColoredTriangles = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {  
        for (int i = 0; i < m_JusticeGuards.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, m_JusticeGuards[i].transform.position);

            if (distance <= m_DistanceThreshold)
            {
                DrawRays();

                float intensity = DeanUtils.Map(distance, 0, m_DistanceThreshold, 0.0f, 1.5f);
                PaintMesh(1 - intensity);
            }
            else
            {
                PaintMesh(0);
            }
        }


    }

    private void DrawRays()
    {
        for (int i = 0; i < m_RingCount; i++)
        {
            float ringStep = m_Radius / m_RingCount;
            float ringRadius = ringStep * (i + 1);

            //make ring of raycasts to get all points
            for (int j = 0; j < m_RayDensity; j++)
            {
                float step = 2 * Mathf.PI / m_RayDensity;

                //use adding for this step * i 
                float offsetX = Mathf.Cos(step * j) * ringRadius;
                float offsetY = Mathf.Sin(step * j) * ringRadius;

                Vector3 origin = transform.position + new Vector3(offsetX, 0, offsetY);
                Debug.DrawLine(origin, origin + new Vector3(0, 10f, 0));

                Ray ray = new Ray(origin, Vector3.down);
                int triIndex = RayCastForTriangle(ray);

                if (triIndex >= 0)
                {
                    m_ColoredTriangles.Add(triIndex);
                }
            }
        }
    }

    //returns int of tri hit, -1 if nothing
    private int RayCastForTriangle(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.triangleIndex;
        }
        else
        {
            return -1;
        }
    }


    //https://stackoverflow.com/questions/57405631/unity-how-to-use-mesh-triangleindex-to-recolor-a-triangle-when-clicked for triangle math
    private void PaintMesh(float intensity)
    {
        Mesh mesh = m_JusticeGuards[0].GetComponent<MeshFilter>().mesh;

        var triangles = mesh.triangles;
        m_MeshColors = new Color[mesh.vertexCount];

        for (int i = 0; i < m_MeshColors.Length; i++)
        {
            //m_MeshColors[i] = Color.blue;
        }

        for (int i = 0; i < m_ColoredTriangles.Count; i++)
        {
            m_MeshColors[triangles[m_ColoredTriangles[i] * 3]] = Color.red;
        }

        mesh.colors = m_MeshColors;

        //clear list
        m_ColoredTriangles.Clear();

        Material m = m_JusticeGuards[0].GetComponent<MeshRenderer>().material;

        Color color = m_LitColor * Mathf.Pow(2, intensity);
        color.a = intensity;
        m.SetColor("_EmissionColor", color);
    }

}
