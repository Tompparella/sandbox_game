using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DialogueHandler {
    private string[] rootWords = Constants.ROOTWORDS;
    private string[] supportWords = Constants.SUPPORTWORDS; // Verbs etc.
    private string[] objectWords = Constants.OBJECTWORDS;
    private string[] completeWords;
    private Helper helper = new Helper();

    private string rootWord = null, supportWord = null, objectWord = null;

    public string HandleDoInput(string input, Dialogue dialogue) {
        string[] commands = helper.ParseInput(input);
        foreach(string word in commands) {
            if (supportWords.Contains(word)){
                supportWord = word;
            } else if (objectWords.Contains(word)) {
                objectWord = word;
            }
        }
        completeWords = new string[] {supportWord, objectWord};

        return dialogue.TakeDoInput(completeWords);
    }

    public string HandleSayInput(string input, Dialogue dialogue) {
        string[] words = helper.ParseInput(input);
        foreach(string word in words) {
            if (rootWords.Contains(word)) {
                rootWord = word;
            } else if (supportWords.Contains(word)){
                supportWord = word;
            } else if (objectWords.Contains(word)) {
                objectWord = word;
            }
        }
        completeWords = new string[] {rootWord, supportWord, objectWord};

        return dialogue.TakeSayInput(completeWords);
    }
}