using System.Collections.Generic;
using System.Linq;

public static class CloudList
{
    public static string StringsToSeparatedString(string[] strings, string separator = ",")
    {
        string newStr = "";
        for (int i = 0; i < strings.Length-1; i++)
        {
            newStr += strings[i] + separator;
        }
        newStr += strings[^1];
        return newStr;
    }

    public static T[] SeparatedStringToValues<T>(string separatedString, string separator = ",")
    {
        List<T> values = new();
        string[] strings = separatedString.Split(separator);
        for (int i = 0; i < strings.Length; i++)
        {
            values.Add((T)strings[i].Cast<T>());
        }
        return values.ToArray();
    }
}
