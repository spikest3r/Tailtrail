using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class DialogLine
{
    public string[] text; // random text
    public bool hasChoices;
    public List<DialogChoice> choices;
    public bool DontProceed; // dont do next line, end dialog
}

[System.Serializable]
public class DialogChoice
{
    public string text;
    public UnityEvent onChosen;
}