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

    [Header("Icons")]
    public Sprite bugSprite;
    public Sprite trojanSprite;
    public Sprite wormSprite;
    public Sprite keySprite;
    public Sprite monsterSprite;

    string[] loading    = {"|", "/", "-", "\\"};
    bool  is_loading    = false;
    string  room_name   = "";

    bool ended = true;
    bool showStatistics = false;
    int  monster_killed = 0;
    List<IEnumerator> coroutines = new List<IEnumerator>();

    IDbCommand  dbCommand;
    UIManager   uiManager;
    GameManager gameManager;
    PlayerController playerController;
    
    void Start()
    {
        dbCommand   = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().dbConnection.CreateCommand();
        uiManager   = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        StartGame();
    }

    void Update()
    {
        if(showStatistics)
        {
            float animation_speed = 0.1f;
            coroutines.Add(show_counting_text("<color=#ff7777>Killed monsters:\t {0}</color>", monster_killed, 1, animation_speed));
            coroutines.Add(show_counting_text("<color=#ffff77>Cleared rooms:\t\t {0}</color>", playerController.GetCleanedRoomsAmount(), 1, animation_speed));
            coroutines.Add(show_counting_text("<color=#7777ff>Points:\t\t\t\t {0}</color>", playerController.levelPoints, 10, animation_speed));
            coroutines.Add(show_counting_text("<color=#aaaaff>Maze points: \t\t{0}</color>", gameManager.GetMaxLevelPoints(), 10, animation_speed));
            float clearedPercentes = (float)playerController.levelPoints/(float)gameManager.GetMaxLevelPoints() * 100;
            coroutines.Add(show_new_text(String.Format("<color=#77ffff>Cleared {0}% of maze</color>", clearedPercentes)));
            int level_points_diff = playerController.levelPoints - playerController.GetMaxLevelPoints();
            if(level_points_diff > 0)
            {
                coroutines.Add(show_new_text("Level upgrade: TRUE"));
                coroutines.Add(show_upgradable());
            }
            else
            {
                coroutines.Add(show_new_text("<color=#ff7777>Level upgrade: FALSE</color>"));
                coroutines.Add(show_counting_text("To new level: {0}", Mathf.Abs(level_points_diff), 10, animation_speed));
                coroutines.Add(show_new_text("<color=#ffffff>Press Enter to continue..."));
                gameManager.ActivateWaitForEnterToEnd();
            }
            
            showStatistics = false;
        }
        if(coroutines.Count > 0)
        {
            foreach(IEnumerator coroutine in coroutines)
            {
                if(ended)
                {
                    StartCoroutine(coroutine);
                    coroutines.Remove(coroutine);
                    break;
                }
            }
        }
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    IEnumerator show_upgradable()
    {
        IEnumerator coroutine = show_new_text("");
        StartCoroutine(coroutine);
        for(int i = 0; i < 3; i++)
        {
            GameObject row   = InitObjectInContent(defaultRoomsTableRow);
            Text text        = row.GetComponentInChildren<Text>();
            Button rowButton = row.GetComponentInChildren<Button>();
            switch(i)
            {
                case 0:
                text.text = "<color=#000000>Upgrade Depth Of Vision</color>";
                rowButton.onClick.AddListener(rowButton.gameObject.GetComponent<ButtonFunctions>().UpgradeDOV);
                break;

                case 1:
                text.text = "<color=#ff7777>Upgrade Max Health</color>";
                rowButton.onClick.AddListener(rowButton.gameObject.GetComponent<ButtonFunctions>().UpgradeMH);
                break;

                case 2:
                text.text = "<color=#ffff77>Upgrade Luck</color>";
                rowButton.onClick.AddListener(rowButton.gameObject.GetComponent<ButtonFunctions>().UpgradeLuck);
                break;
            }
            yield return new WaitForSeconds(loadingSpeed);
        }
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
        string this_type       = "";
        Image avatar_image     = null;
        VisualController visualController = gameRoomRow.GetComponentInChildren<VisualController>();
        foreach(Transform column in gameRoomRowButton.transform)
        {
            if(column.gameObject.name.Equals("AVATAR"))
            {
                avatar_image  = column.gameObject.GetComponent<Image>();
                continue;
            }
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
            if(column.gameObject.name.Equals("NAME"))
            {
                if(inputText.Equals("bug"))
                {
                    avatar_image.sprite          = bugSprite;
                    visualController.objectImage = bugSprite;
                }
                if(inputText.Equals("worm"))
                {
                    avatar_image.sprite          = wormSprite;
                    visualController.objectImage = wormSprite;
                }
                if(inputText.Equals("trojan"))
                {
                    avatar_image.sprite          = trojanSprite;
                    visualController.objectImage = trojanSprite;
                }
                if(inputText.Equals("MAZE EXIT"))
                {
                    CommandController commandController = gameRoomRowButton.gameObject.AddComponent(typeof(CommandController)) as CommandController;
                    commandController.isStandardCommand = false;
                    commandController.command           = "CALL EXIT_MAZE()";
                    commandController.commandType       = CommandController.CommandType.ExitMaze;
                    gameRoomRowButton.onClick.AddListener(gameRoomRowButton.GetComponent<ButtonFunctions>().ToCommandLine);
                }
            }
            if(column.gameObject.name.Equals("TYPE"))
            {
                if(inputText.Equals("monster"))
                {
                    avatar_image.sprite          = monsterSprite;
                    visualController.objectImage = monsterSprite;
                    gameRoomRowButton.onClick.AddListener(gameRoomRowButton.GetComponent<ButtonFunctions>().GiveMonsterID);
                }
                if(inputText.Equals("key"))
                {
                    avatar_image.sprite          = keySprite;
                    visualController.objectImage = keySprite;
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

    public void ShowError(string location)
    {
        ShowNewText("<color=#ff7777>UNEXPECTED ERROR in "+location+"</color>");
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

    public void ShowStatistics(int monster_killed)
    {
        this.monster_killed = monster_killed;
        showStatistics      = true;
    }

    IEnumerator show_counting_text(string text, int max_count, int iter, float animation_speed)
    {
        ended = false;
        Text gameNewText = InitObjectInContent(defaultText).GetComponentInChildren<Text>();
        for(int i = 0; i <= max_count; i += iter)
        {
            gameNewText.text = String.Format(text, i);
            yield return new WaitForSeconds(animation_speed);
        }
        ended = true;
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
        IEnumerator coroutine = show_new_text(text);
        StartCoroutine(coroutine);
    }

    IEnumerator show_new_text(string text)
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
        gameNewText.text = text;
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
