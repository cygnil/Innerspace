using UnityEngine;
using System;

public class Chromosome : UnityEngine.Object
{
    private static readonly int basesPerUnit = 100000000;
    private static readonly int radialResolution = 21;
    private static readonly float r = 0.2f;
    private Cytoband pCentromere;
    private Cytoband qCentromere;
    private Cytoband[] Cytobands;
    private GameObject gameObject;
    private int Length;
    private Mesh mesh;
    private Texture2D pTexture;

    public Chromosome(Cytoband[] cytobands)
    {
        SetCytobands(cytobands);
    }

    public void SetCytobands(Cytoband[] cytobands)
    {
        if (cytobands.Length == 0)
        {
            return;
        }

        gameObject = new GameObject(cytobands[0].chr + " Cytoband");

        this.Cytobands = cytobands;

        // Calculate length and centromere coordinates
        for (int i = 0; i < cytobands.Length; i++)
        {
            if (cytobands[i].end > this.Length)
            {
                this.Length = cytobands[i].end;
            }
            if (cytobands[i].pos == "acen")
            {
                if (cytobands[i].arm == 'p')
                {
                    pCentromere = cytobands[i];
                }
                else
                {
                    qCentromere = cytobands[i];
                }
            }
        }
    }

    public void BuildTexture()
    {
        pTexture = new Texture2D(2, 2);
        pTexture.SetPixels(0, 0, 2, 2, new Color[4] {Color.red, new Color(0f, 0f, 0.75f), Color.green, Color.blue});
        pTexture.Apply();
        gameObject.GetComponent<Renderer>().material.mainTexture = pTexture;
    }

    public void BuildMesh()
    {
        gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        mesh = new Mesh();
        UpdateMeshPosition();
        meshFilter.mesh = mesh;
    }

    public void UpdateMeshPosition()
    {
        if (mesh == null)
        {
            return;
        }

        mesh.Clear();

        // Build the points to use as triangle vertices
        float zScale = this.Length / basesPerUnit;
        Vector3[] vertices = new Vector3[radialResolution * 2 + 2];
        for (int i = 0; i < radialResolution; i++)
        {
            float x = Convert.ToSingle(Math.Cos(Convert.ToDouble(i) / radialResolution * Math.PI * 2) * r);
            float y = Convert.ToSingle(Math.Sin(Convert.ToDouble(i) / radialResolution * Math.PI * 2) * r);
            vertices[i] = new Vector3(x, y, -0.5f * zScale);
            vertices[radialResolution + i] = new Vector3(x, y, 0.5f * zScale);
        }
        vertices[vertices.Length - 2] = new Vector3(0, 0, -0.5f * zScale);
        vertices[vertices.Length - 1] = new Vector3(0, 0, 0.5f * zScale);

        // Build a very basic UV map for each vertex--this will almost certainly need to be adjusted later
        // Dubiously necessary. I think Unity calculates a default map that's similar to this, but this is a good placeholder for later.
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        // Build triangles to use as faces--faces are visible in only one direction, the one which is formed by running counterclockwise
        int[] triangles = new int[radialResolution * 12];
        for (int i = 0; i < radialResolution; i++)
        {
            // Side triangles
            triangles[i * 6] = (i + 1) % radialResolution;
            triangles[i * 6 + 1] = i + radialResolution;
            triangles[i * 6 + 2] = i;
            triangles[i * 6 + 3] = (i + 1) % radialResolution + radialResolution;
            triangles[i * 6 + 4] = triangles[i * 6 + 1];
            triangles[i * 6 + 5] = triangles[i * 6];

            // Endcap triangles
            triangles[(radialResolution * 6) + (i * 6)] = i;
            triangles[(radialResolution * 6) + (i * 6 + 1)] = vertices.Length - 2;
            triangles[(radialResolution * 6) + (i * 6 + 2)] = (i + 1) % radialResolution;
            triangles[(radialResolution * 6) + (i * 6 + 3)] = (i + 1) % radialResolution + radialResolution;
            triangles[(radialResolution * 6) + (i * 6 + 4)] = vertices.Length - 1;
            triangles[(radialResolution * 6) + (i * 6 + 5)] = i + radialResolution;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void SetPosition(Vector3 translation)
    {
        this.gameObject.transform.localPosition = translation;
    }

    public void SetScale(Vector3 scale)
    {
        this.gameObject.transform.localScale = scale;
    }

    public void SetRotation(Vector3 rotation)
    {
        this.gameObject.transform.rotation = Quaternion.Euler(rotation);
    }

    public void SetRotation(Quaternion rotation)
    {
        this.gameObject.transform.rotation = rotation;
    }
}
