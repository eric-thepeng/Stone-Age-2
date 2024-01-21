using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SOReadCSV 
{
    public static Dictionary<string, string> ReadCSV(string file)
    {
        var result = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(file);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] col = lines[i].Split(',');
            result.Add(col[0], col[1]);
        }

        return result;
    }
}
