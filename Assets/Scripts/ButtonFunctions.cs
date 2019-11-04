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

    public void ToCommandLinePunch()
    {
        ToCommandLine();
        terminal.ShowNewTextInput("Type monster secrete:");
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
                    string monster_id = column.gameObject.GetComponent<Text>().text;
                    uiManager.GetLastPressed().TakeRequest(monster_id);
                    uiManager.SetCurrentMonsterID(monster_id);
                    break;
                }
            }
        }
    }

    public void GiveKeyID()
    {
        if(uiManager.GetLastRequest().Equals(UIManager.REQUEST_KEYID))
        {
            foreach(Transform column in transform)
            {
                if(column.gameObject.name.Equals("ID"))
                {
                    string key_id = column.gameObject.GetComponent<Text>().text;
                    uiManager.GetLastPressed().TakeRequest(key_id);
                    uiManager.SetCurrentKeyID(key_id);
                    break;
                }
            }
        }
    }

    public void GiveItemType()
    {
        if(uiManager.GetLastRequest().Equals(UIManager.REQUEST_ITEMTYPE))
        {
            string item_type = ""; 
            string item_id   = "";
            foreach(Transform column in transform)
            {
                if(column.gameObject.name.Equals("NAME"))
                {
                    item_type = column.gameObject.GetComponent<Text>().text;
                    uiManager.GetLastPressed().TakeRequest(item_type);
                }
                else if(column.gameObject.name.Equals("ID"))
                {
                    item_id = column.gameObject.GetComponent<Text>().text;
                }
            }
            uiManager.SetCurrentItemTypeAndID(item_type, item_id);
        }
    }
}
