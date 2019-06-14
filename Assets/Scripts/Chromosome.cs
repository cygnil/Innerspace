using UnityEngine;
using System;

public class Chromosome
{
    private static readonly int basesPerUnit = 100000000;
    private static readonly int radialResolution = 21;
    private static readonly float r = 1;
    private int[] pCentromere;
    private int[] qCentromere;
    private Cytoband[] Cytobands;
    private int Length;
    private Mesh mesh;
    private Texture2D pTexture;
    public Vector3 Scale = new Vector3(1, 1, 1);
    public Vector3 Translation = new Vector3(0, 0, 0);

    public Chromosome(Cytoband[] cytobands)
    {
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
                    pCentromere = new int[2] {cytobands[i].start, cytobands[i].end};
                } else
                {
                    qCentromere = new int[2] {cytobands[i].start, cytobands[i].end};
                }
            }
        }
    }

    public Texture2D BuildTexture()
    {
        pTexture = new Texture2D(2, 1);
        pTexture.SetPixels(0, 0, 2, 1, new Color[2] {new Color(0.75f, 0.75f, 0.75f), new Color(0.25f, 0.25f, 0.25f) });
        pTexture.Apply();
        return pTexture;
    }

    public void BuildMesh(GameObject gameObject)
    {
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

        // Build the points to use as triangle vertices
        float zScale = this.Scale.z * this.Length / basesPerUnit;
        Vector3[] vertices = new Vector3[radialResolution * 2];
        for (int i = 0; i < radialResolution; i++)
        {
            float x = Convert.ToSingle(Math.Cos(Convert.ToDouble(i) / radialResolution * Math.PI * 2) * r) * this.Scale.x + this.Translation.x;
            float y = Convert.ToSingle(Math.Sin(Convert.ToDouble(i) / radialResolution * Math.PI * 2) * r) * this.Scale.y + this.Translation.y;
            vertices[i] = new Vector3(x, y, -0.5f * zScale + this.Translation.z);
            vertices[radialResolution + i] = new Vector3(x, y, 0.5f * zScale + this.Translation.z);
        }

        // Build triangles to use as faces--faces are visible in only one direction, the one which is formed by running counterclockwise
        int[] triangles = new int[radialResolution * 6];
        for (int i = 0; i < radialResolution; i++)
        {
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + radialResolution;
            triangles[i * 6 + 2] = (i + 1) % radialResolution;
            triangles[i * 6 + 3] = triangles[i * 6 + 2];
            triangles[i * 6 + 4] = triangles[i * 6 + 1];
            triangles[i * 6 + 5] = (i + 1) % radialResolution + radialResolution;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
