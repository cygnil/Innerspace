using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LoadCytobands : MonoBehaviour
{
    public static Cytoband[] Cytobands;
    public static HashSet<string> chrNames;

    // Start is called before the first frame update
    void Start()
    {
        Cytobands = LoadCytobandsFromAsset("hg19.cyto");
        chrNames = GetChromosomeNames(Cytobands);
        BuildCytobandMeshes(Cytobands);
    }

    // Update is called once per frame
    void Update()
    {
        // Perhaps the following?
        // Material mat
        // Graphics.DrawMesh(mesh, transform.localToWorldMatrix, mat, 0);
    }

    Cytoband[] LoadCytobandsFromAsset(string file)
    {
        TextAsset cytoText = (TextAsset)Resources.Load(file);
        string[] cytobandLines = cytoText.ToString().Split('\n');
        // Length - 1 here because we want to avoid the last line after a return
        Cytoband[] cytobands = new Cytoband[cytobandLines.Length - 1];
        for (int i = 0; i < cytobandLines.Length - 1; i++)
        {
            string[] line = cytobandLines[i].Split('\t');
            cytobands[i] = new Cytoband(line[0], line[1], line[2], line[3], line[4]);
        }

        return cytobands;
    }

    HashSet<string> GetChromosomeNames(Cytoband[] cytobands)
    {
        HashSet<string> names = new HashSet<string>();
        Regex hasUnderscore = new Regex("_");
        for (int i = 0; i < cytobands.Length; i++)
        {
            if (!hasUnderscore.IsMatch(cytobands[i].chr))
            {
                names.Add(cytobands[i].chr);
            }
        }
        return names;
    }

    void BuildCytobandMeshes(Cytoband[] cytobands)
    {
        List<Chromosome> chromosomes = new List<Chromosome>();
        HashSet<string>.Enumerator chrEnum = chrNames.GetEnumerator();
        int i = 0;
        while (chrEnum.MoveNext())
        {
            Chromosome chr = new Chromosome(Array.FindAll(cytobands, band => band.chr == chrEnum.Current));
            chromosomes.Add(chr);
            chr.Translation = new Vector3(i, i, i);
            chr.BuildMesh();
            chr.BuildTexture();
            i++;
        }
    }
}
