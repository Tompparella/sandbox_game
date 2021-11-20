using Godot;
using System;
using System.Linq;

public class Helper
{
    public string RemoveSpecialCharacters(string str) {
        if (str.Any()) {
            string[] chars = new string[] { ",", ".", "/", "!", "?", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
        }
        return str;
    }
    public string[] ParseInput(string input) {
        input = RemoveSpecialCharacters(input).ToLower();
        string[] keywords = input.Split(' ');
        return keywords;
    }
}