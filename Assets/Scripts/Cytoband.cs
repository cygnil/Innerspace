using System;
using System.Text.RegularExpressions;

public class Cytoband
{
    public string chr;
    public int start;
    public int end;
    public char arm;
    public float armPos;
    public string pos;
    public  int stain;

    public Cytoband(string chr, int start, int end, string arm, string g)
    {
        this.init(chr, start, end, arm, g);
    }

    public Cytoband(string chr, string start, string end, string arm, string g)
    {
        this.init(chr, int.Parse(start), int.Parse(end), arm, g);
    }

    private void init(string chr, int start, int end, string arm, string g)
    {
        this.chr = chr;
        this.start = start;
        this.end = end;
        if (arm.Length > 0)
        {
            this.arm = arm[0];
            this.armPos = float.Parse(arm.Substring(1));
        }
        this.pos = new Regex(@"\d*").Replace(g, "");
        try
        {
            this.stain = int.Parse(new Regex(@"[^0-9]*").Replace(g, ""));
        }
        catch (Exception) { }
    }
}
