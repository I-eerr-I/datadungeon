using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UIManager : MonoBehaviour
{   
    [Header("Inventory")]
    public Text   keysAmount;
    public Text   bugsAmount;
    public Text   trojansAmount;
    public Text   wormsAmount;
    
    [Header("InputField")]
    public InputField inputField;

    [Header("Avatar")]
    public Slider health;

    [Header("Characteristics")]
    public Text level;
    public Text depthOfVision;
    public Text maxHealth;
    public Text luck;

    [Header("Visual Part")]
    public GameObject commands;

    [Header("Command Buttons")]
    public GameObject[] inMazeCommands;
    public GameObject[] inRoomCommands;
    public GameObject[] inFightCommands;

    public enum GameState {InMaze, InRoom, InFight};
    public GameState currentState;
    GameState savedState;
    
    // public const string REQUEST_MONSTERID = "monster_id";
    // public const string REQUEST_ROOMNAME  = "room_name";
    // public const string REQUEST_SECRETE   = "secrete";
    // public const string REQUEST_ITEMTYPE  = "item_type";
    // public const string REQUEST_KEYID     = "key_id"; 

    Text input;
    CommandController lastPressed;
    string            lastRequest;

    PlayerController   player_controller;
    GameManager        gameManager;
    TerminalController terminal;

    int    maxAmountOfCommands;
    bool   updateVisual;
    string currentRoom;

    void Start()
    {
        gameManager         = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        terminal            = GameObject.FindWithTag("Terminal").GetComponent<TerminalController>();
        currentState        = GameState.InMaze;
        inputField.enabled  = false;
        input               = inputField.GetComponentInChildren<Text>();
        updateVisual        = true;
        currentRoom         = null;
        savedState          = currentState;
        lastRequest         = "";
        lastPressed         = null;
        maxAmountOfCommands = Mathf.Max(inMazeCommands.Length, inRoomCommands.Length, inFightCommands.Length); 
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            input.text  = "...|";
            lastPressed = null;
            lastRequest = "";
        }
        if(Input.GetButtonDown("Enter"))
        {
            if(lastPressed != null)
            {
                switch(lastPressed.commandType)
                {
                    case CommandController.CommandType.EnterTheRoom:
                    EnterTheRoom();
                    break;
                }
            }
            lastPressed = null;
            lastRequest = "";
        }

        UpdateInventory();
        UpdateHealth();
        UpdateCharacteristics();
        CheckAndSetUpdates();
        if(updateVisual) UpdateVisual();
    }

    public void EnterTheRoom()
    {
        if(!gameManager.IsTheRoomLocked(currentRoom))
        {
            currentState = GameState.InRoom;
            player_controller.AddRoomForVision(currentRoom);
            terminal.ShowRoom(currentRoom);
        }
        else
        {
            terminal.ShowNewText("<color=#ff7777>[ERROR] THE ROOM IS LOCKED. NEED KEY</color>");
        }
    }

    public string GetCurrentRoom()
    {
        return currentRoom;
    }

    public void SetRoomName(string name)
    {
        currentRoom = name;
    }
    
    public void SetPlayerController(PlayerController player_controller)
    {
        this.player_controller = player_controller;
    }

    public void SetLastCommand(CommandController lastPressed)
    {
        this.lastPressed = lastPressed;
        this.lastRequest = lastPressed.requestFor;
    }

    public string GetLastRequest()
    {
        return lastRequest;
    }

    public CommandController GetLastPressed()
    {
        return lastPressed;
    }

    void CheckAndSetUpdates()
    {
        if(savedState != currentState)
        {
            updateVisual = true;
        }
        savedState = currentState;
    }

    void UpdateVisual()
    {
        switch (currentState)
        {
            case GameState.InMaze:
            EnterCommandsIn(inMazeCommands);
            break;

            case GameState.InRoom:
            EnterCommandsIn(inRoomCommands);
            break;
            
            case GameState.InFight:
            EnterCommandsIn(inFightCommands);
            break;
        }
        updateVisual = false;
    }

    void EnterCommandsIn(GameObject[] commandsList)
    {
        foreach(Transform initedCommand in commands.transform)
        {
            Destroy(initedCommand.gameObject);
        }
        foreach(GameObject command in commandsList)
        {
            GameObject instantiatedCommand = Instantiate(command);
            instantiatedCommand.transform.SetParent(commands.transform);
        }
    }



    void UpdateHealth()
    {
        health.value = (float)player_controller.health/(float)player_controller.maxHealth;
    }

    void UpdateInventory()
    {
        keysAmount.text    = player_controller.keysAmount.ToString() + "x";
        bugsAmount.text    = player_controller.bugsAmount.ToString() + "x";
        trojansAmount.text = player_controller.trojansAmount.ToString() + "x";
        wormsAmount.text   = player_controller.wormsAmount.ToString() + "x";
    }

    void UpdateCharacteristics()
    {
        level.text         = "---\tLevel:\t" + player_controller.level.ToString() + "\t---";
        depthOfVision.text = player_controller.depthOfVision.ToString();
        maxHealth.text     = player_controller.maxHealth.ToString();
        luck.text          = player_controller.luck.ToString();
    }

}
