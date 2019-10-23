using System.Data;
using System;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalController : MonoBehaviour
{
    public GameObject content;
    public GameObject defaultRoomRow;
    public GameObject defaultRoomsTableRow;
    public GameObject defaultText;

    IDbCommand dbCommand;
    UIManager  uiManager;
    PlayerController playerController;
    
    void Start()
    {
        dbCommand = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().dbConnection.CreateCommand();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        StartCoroutine("StartGame");
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void ShowRoom(string room_name)
    {
        dbCommand.CommandText = "SELECT id, name, type FROM "+ room_name;
        IDataReader reader    = dbCommand.ExecuteReader();
        int amount            = playerController.GetVisionForRoom(room_name);
        int i                 = 0;
        ShowNewText("You're openning the " + room_name);
        GameObject startRoomRow = InitObjectInContent(defaultRoomRow);
        startRoomRow.GetComponentInChildren<Button>().interactable = false;
        while(reader.Read() && i < amount)
        {
            GameObject gameRoomRow = InitObjectInContent(defaultRoomRow);
            Button     gameRoomRowButton = gameRoomRow.GetComponentInChildren<Button>();
            int column_index       = 0;
            foreach(Transform column in gameRoomRowButton.transform)
            {
                if(column.gameObject.name == "AVATAR") continue;
                Type columnType = reader.GetFieldType(column_index);
                string inputText = "";
                if(columnType.Equals("".GetType()))
                {
                    inputText = reader.GetString(column_index); 
                }
                else
                {
                    inputText = reader.GetInt64(column_index).ToString();
                }
                if(column.gameObject.name.Equals("TYPE"))
                {
                    if(inputText.Equals("monster"))
                    {
                        gameRoomRowButton.onClick.AddListener(gameRoomRowButton.GetComponent<ButtonFunctions>().GiveMonsterID);
                    }
                }
                column.gameObject.GetComponent<Text>().text = inputText;
                column_index++;
            }
            i++;
        }
        reader.Close();
    }

    IEnumerator StartGame()
    {
        ShowNewText("sqlite3 Maze.db");
        yield return new WaitForSeconds(1f);
        ShowNewText("Processing...");
        yield return new WaitForSeconds(1f);
        ShowRooms();
    }

    public void ShowNewText(string text)
    {
        GameObject gameNewText = InitObjectInContent(defaultText);
        gameNewText.GetComponentInChildren<Text>().text = text;
    }

    public void ShowRooms()
    {
        dbCommand.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name LIKE 'room%' AND name <> 'rooms'";
        IDataReader reader    = dbCommand.ExecuteReader();
        int amount = playerController.GetVisiableRooms();
        int i = 0;
        while(reader.Read() && i < amount)
        {
            GameObject gameRoomsRow = InitObjectInContent(defaultRoomsTableRow); 
            Text roomName           = gameRoomsRow.GetComponentInChildren<Text>();
            roomName.text = reader.GetString(0);
            Button btn    = gameRoomsRow.GetComponentInChildren<Button>();
            btn.onClick.AddListener(gameRoomsRow.GetComponentInChildren<ButtonFunctions>().GiveRoomName);
            i++;
        }
        reader.Close();
    }

    public GameObject InitObjectInContent(GameObject toInit)
    {
        GameObject gameToInit = Instantiate(toInit, toInit.transform.position, Quaternion.identity);
        gameToInit.transform.SetParent(content.transform);
        return gameToInit;
    }
    
}
