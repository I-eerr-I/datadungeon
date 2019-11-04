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
    public GameObject defaultTextInput;

    [Header("Loading Parametes")]
    public float loadingSpeed = 0.2f;
    public int   animIters    = 8;

    string[] loading    = {"|", "/", "-", "\\"};
    bool  is_loading    = false;
    string  room_name   = "";
    string newText      = "";

    IDbCommand dbCommand;
    UIManager  uiManager;
    PlayerController playerController;
    
    void Start()
    {
        dbCommand = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().dbConnection.CreateCommand();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        StartGame();
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void ShowRoomLoading(string room_name)
    {
        this.room_name = room_name;
        StartCoroutine("show_room");
    }

    IDataReader GetOneRoomSelectReader(string room_name)
    {
        dbCommand.CommandText = "SELECT id, name, type FROM "+ room_name;
        IDataReader reader    = dbCommand.ExecuteReader();
        return reader;
    }

    void InitStartRoomRow()
    {
        GameObject startRoomRow = InitObjectInContent(defaultRoomRow);
        startRoomRow.GetComponentInChildren<Button>().interactable = false;
    }

    void InitRoomRow(IDataReader reader)
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
                if(inputText.Equals("key"))
                {
                    gameRoomRowButton.onClick.AddListener(gameRoomRowButton.GetComponent<ButtonFunctions>().GiveKeyID);
                }
                if(inputText.Equals("item"))
                {
                    gameRoomRowButton.onClick.AddListener(gameRoomRowButton.GetComponent<ButtonFunctions>().GiveItemType);
                }
            }
            column.gameObject.GetComponent<Text>().text = inputText;
            column_index++;
        }
    }

    public void ShowNewTextInput(string text)
    {
        GameObject gameNewTextInput = InitObjectInContent(defaultTextInput);
        Text newText = gameNewTextInput.GetComponentInChildren<Text>();
        newText.text = text;
    }

    public void ShowRoom(string room_name)
    {
        IDataReader reader    = GetOneRoomSelectReader(room_name);
        int amount            = playerController.GetVisionForRoom(room_name);
        int i                 = 0;
        InitStartRoomRow();
        while(reader.Read() && i < amount)
        {
            InitRoomRow(reader);
            i++;
        }
        reader.Close();
    }

    IEnumerator show_room()
    {
        IDataReader reader    = GetOneRoomSelectReader(this.room_name);
        int amount            = playerController.GetVisionForRoom(this.room_name);
        int i                 = 0;
        InitStartRoomRow();
        if(is_loading) yield return new WaitForSeconds(loadingSpeed*2);
        is_loading = true;
        while(reader.Read() && i < amount)
        {
            InitRoomRow(reader);
            i++;
            yield return new WaitForSeconds(loadingSpeed);
        }
        is_loading = false;
        reader.Close();
    }

    void StartGame()
    {
        ShowNewText("sqlite3 Maze.db");
        ShowNewTextLoading("");
        ShowRoomsLoading();
    }

    public void ShowNewText(string text)
    {
        Text gameNewText = InitObjectInContent(defaultText).GetComponentInChildren<Text>();
        gameNewText.text = text;
    }

    public void ShowNewTextLoading(string text)
    {
        newText = text;
        StartCoroutine("show_new_text");
    }

    IEnumerator show_new_text()
    {
        Text gameNewText = InitObjectInContent(defaultText).GetComponentInChildren<Text>();
        string startText = "Processing... {0}";
        gameNewText.text = startText;
        if(is_loading) yield return new WaitForSeconds(loadingSpeed*2);
        is_loading       = true;
        for(int i = 0; i < animIters; i++)
        {
            gameNewText.text = string.Format(gameNewText.text, loading[i%loading.Length]);
            yield return new WaitForSeconds(loadingSpeed);
            gameNewText.text = startText;
        }
        is_loading       = false;
        gameNewText.text = newText;
    }

    IDataReader GetRoomsSelectReader()
    {
        dbCommand.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name LIKE 'room%' AND name <> 'rooms'";
        IDataReader reader    = dbCommand.ExecuteReader();
        return reader;
    }

    void InitRoomsLine(IDataReader reader)
    {
        GameObject gameRoomsRow = InitObjectInContent(defaultRoomsTableRow); 
        Text roomName           = gameRoomsRow.GetComponentInChildren<Text>();
        roomName.text           = reader.GetString(0);
        Button btn              = gameRoomsRow.GetComponentInChildren<Button>();
        btn.onClick.AddListener(gameRoomsRow.GetComponentInChildren<ButtonFunctions>().GiveRoomName);
    }

    public void ShowRooms()
    {
        IDataReader reader = GetRoomsSelectReader(); 
        int amount = playerController.GetVisiableRooms();
        int i = 0;
        while(reader.Read() && i < amount)
        {
            InitRoomsLine(reader);
            i++;
        }
        reader.Close();
    }

    public void ShowRoomsLoading()
    {
        StartCoroutine("show_rooms");
    }

    IEnumerator show_rooms()
    {
        IDataReader reader = GetRoomsSelectReader();
        int amount = playerController.GetVisiableRooms();
        int i = 0;
        if(is_loading) yield return new WaitForSeconds(loadingSpeed *2);
        is_loading = true;
        while(reader.Read() && i < amount)
        {
            InitRoomsLine(reader);
            i++;
            yield return new WaitForSeconds(loadingSpeed);
        }
        is_loading = false;
        reader.Close();
    }

    public GameObject InitObjectInContent(GameObject toInit)
    {
        GameObject gameToInit = Instantiate(toInit, toInit.transform.position, Quaternion.identity);
        gameToInit.transform.SetParent(content.transform);
        return gameToInit;
    }
    
}
