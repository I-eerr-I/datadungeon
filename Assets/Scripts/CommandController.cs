using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class CommandController : MonoBehaviour
{
    [TextArea]
    public string description;
    [TextArea]
    public string command;
    public int    fontSize;
    public bool   useRoomNameFormat = false;
    public string requestFor;
    public enum CommandType {EnterTheRoom, Attack, ExitRoom, Punch, RunAway, TakeItem, TakeKey, UseItem, Quit, ExitMaze, SkipTutorial}; 
    public CommandType commandType;
    public bool isStandardCommand = true;

    Text input;

    
    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Input").GetComponent<Text>();
        if(isStandardCommand)
        {
            if(useRoomNameFormat)
            {
                command = string.Format(command, GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().GetCurrentRoom());
            }
            Text text = GetComponentInChildren<Text>();
            if(description.Length < 30)
                text.text = description + "\t\t" + command;
            else if(description.Length < 3)
                text.text = description + "\t\t\t" + command;
            else
                text.text = description + "\t" + command;
            text.fontSize = fontSize;
        }
    }

    public void TakeRequest(string request)
    {
        input.text = Regex.Replace(command, "(\\["+requestFor+"\\])", request);
    }
}
