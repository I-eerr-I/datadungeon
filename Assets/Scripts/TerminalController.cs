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

    IDbCommand dbCommand;
    UIManager  uiManager;
    
    void Start()
    {
        dbCommand = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().dbConnection.CreateCommand();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        //StartGame();    
        //ShowTables();
        ShowRoom("room0");
    }

    void ShowRoom(string room_name)
    {
        dbCommand.CommandText = "SELECT id, type, name FROM "+ room_name;
        IDataReader reader = dbCommand.ExecuteReader();
        while(reader.Read())
        {
            GameObject gameRoomRow = Instantiate(defaultRoomRow, defaultRoomRow.transform.position, Quaternion.identity);
            gameRoomRow.transform.SetParent(content.transform);
            int column_index = 0;
            foreach(Transform column in gameRoomRow.GetComponentInChildren<Button>().transform)
            {
                if(column.gameObject.name == "AVATAR") continue;
                Type columnType = reader.GetFieldType(column_index);
                if(columnType.Equals("".GetType()))
                {
                    column.gameObject.GetComponent<Text>().text = reader.GetString(column_index); 
                }
                else
                {
                    column.gameObject.GetComponent<Text>().text = reader.GetInt64(column_index).ToString();
                }
                column_index++;
            }
        }
        reader.Close();
    }

    void StartGame()
    {
        
    }

    void ShowTables()
    {
        
    }

    
}
