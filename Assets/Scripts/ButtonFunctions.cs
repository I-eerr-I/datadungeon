using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    Text               input;
    GameManager        gameManager;
    TerminalController terminal;
    UIManager          uiManager;
    CommandController  commandController; 


    void Start()
    {
        input             = GameObject.FindGameObjectWithTag("Input").GetComponent<Text>();
        terminal          = GameObject.FindGameObjectWithTag("Terminal").GetComponent<TerminalController>();
        uiManager         = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        commandController = GetComponent<CommandController>();
        gameManager       = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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

    public void UpgradeDOV()
    {
        gameManager.Upgrade(GameManager.UPGRADABLE.DOV);
    }

    public void UpgradeMH()
    {
        gameManager.Upgrade(GameManager.UPGRADABLE.MH);
    }

    public void UpgradeLuck()
    {
        gameManager.Upgrade(GameManager.UPGRADABLE.LUCK);
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
                    string monster_id = gameManager.GetMonsterID(column.gameObject.GetComponent<Text>().text, uiManager.GetCurrentRoom());
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

    public void GiveItemTypeForUse()
    {
        if(uiManager.GetLastRequest().Equals(UIManager.REQUEST_ITEMTYPE) && 
           uiManager.GetLastPressed().commandType.Equals(CommandController.CommandType.UseItem))
        {
            InitTakeItemType(gameObject.tag);
        }
    }

    void InitTakeItemType(string item_type)
    {
        uiManager.GetLastPressed().TakeRequest(item_type);
        uiManager.SetCurrentUseItemType(item_type);
    }
}
