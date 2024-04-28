using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utility
{
    public static string stageDataPath = Application.persistentDataPath + "/stageData.json";
    public static string StageDataSavePrefKey = "StageDataSaved";

    public static Dictionary<string, string> GetParametersFromURL(string url)
    {
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        Uri uri = new Uri(url);
        if (uri == null)
            throw new ArgumentNullException("uri");

        if (uri.Query.Length == 0)
            return new Dictionary<string, string>();

        pairs = uri.Query.TrimStart('?')
                        .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                        .GroupBy(parts => parts[0],
                                 parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                        .ToDictionary(grouping => grouping.Key,
                                      grouping => string.Join(",", grouping));
        string s= "Pairs\n";
        foreach (var pair in pairs)
        {
            s += string.Format("{0} = {1}\n", pair.Key, pair.Value);
        }
        Debug.Log(s);
        return pairs;
    }
}
