using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    Text               input;
    TerminalController terminal;
    UIManager          uiManager;
    CommandController  commandController; 

    void Start()
    {
        input             = GameObject.FindGameObjectWithTag("Input").GetComponent<Text>();
        terminal          = GameObject.FindGameObjectWithTag("Terminal").GetComponent<TerminalController>();
        uiManager         = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        commandController = GetComponent<CommandController>();
    }

    public void ToCommandLine()
    {
        input.text = commandController.command;
        uiManager.SetLastCommand(commandController);
    }

    public void GiveRoomName()
    {
        if(uiManager.GetLastRequest().Equals(UIManager.REQUEST_ROOMNAME))
        {
            string roomName = GetComponentInChildren<Text>().text;
            uiManager.GetLastPressed().TakeRequest(roomName);
            uiManager.SetRoomName(roomName); 
        }
    }

    public void GiveMonsterID()
    {
        if(uiManager.GetLastRequest().Equals(UIManager.REQUEST_MONSTERID))
        {
            foreach(Transform column in transform)
            {
                if(column.gameObject.name.Equals("ID"))
                {
                    uiManager.GetLastPressed().TakeRequest(column.gameObject.GetComponent<Text>().text);
                    break;
                }
            }
        }
    }
}
