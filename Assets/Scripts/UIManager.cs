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
    public Text inputField;

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
    
    public const string REQUEST_MONSTERID = "monster_id";
    public const string REQUEST_ROOMNAME  = "room_name";
    public const string REQUEST_SECRETE   = "secrete";
    public const string REQUEST_ITEMTYPE  = "item_type";
    public const string REQUEST_KEYID     = "key_id"; 

    CommandController lastPressed = null;
    string            lastRequest = "";

    PlayerController   player_controller;
    GameManager        gameManager;
    TerminalController terminal;

    int    maxAmountOfCommands;
    bool   updateVisual          = true;
    string currentRoom           = "";
    long   currentMonstersAmount = -1;
    string currentMonsterID      = "";
    int    currentSecrete        = -1;
    string currentKeyID          = "";
    string currentItemType       = "";
    string currentItemID         = "";

    void Start()
    {
        gameManager           = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        terminal              = GameObject.FindWithTag("Terminal").GetComponent<TerminalController>();
        currentState          = GameState.InMaze;
        savedState            = currentState;
        maxAmountOfCommands   = Mathf.Max(inMazeCommands.Length, inRoomCommands.Length, inFightCommands.Length); 
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            inputField.text  = "...";
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

                    case CommandController.CommandType.ExitRoom:
                    ExitRoom();
                    break;

                    case CommandController.CommandType.Attack:
                    Attack();
                    break;

                    case CommandController.CommandType.RunAway:
                    RunAway();
                    break;

                    case CommandController.CommandType.Quit:
                    Quit();
                    break;

                    case CommandController.CommandType.TakeKey:
                    TakeKey();
                    break;

                    case CommandController.CommandType.TakeItem:
                    TakeItem();
                    break;

                }
                if(!lastPressed.commandType.Equals(CommandController.CommandType.Punch))
                {
                    lastPressed = null;
                    lastRequest = "";
                } 
            }
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
            currentMonstersAmount = gameManager.CountMonstersInRoom(currentRoom);
            currentState = GameState.InRoom;
            terminal.ShowNewTextLoading("You've opened the " + currentRoom);
            if(currentMonstersAmount == 0) 
            {
                terminal.ShowNewText("<color=#ffff77>This room is clear.</color>");
                player_controller.IncrementVisiableRooms(currentRoom);
            }
            player_controller.AddRoomForVision(currentRoom);
            terminal.ShowRoomLoading(currentRoom);
        }
        else
        {
            bool oppened = player_controller.OpenTheRoom(currentRoom);
            Debug.Log(oppened.ToString() + " " + currentRoom);
            if(!oppened)
            {
                terminal.ShowNewText("<color=#ff7777>[ERROR] THE ROOM IS LOCKED. NEED KEY</color>");
                terminal.ShowNewTextLoading("");
                currentRoom = "";
                currentMonstersAmount = -1;
                terminal.ShowRoomsLoading();
            }
            else
            {
                gameManager.UnlockTheRoom(currentRoom);
                EnterTheRoom();
            }
        }
    }

    public void TakeKey()
    {
        if(currentMonstersAmount <= 0)
        {
            string locked_room = player_controller.AddKey(currentKeyID, currentRoom);
            if(locked_room != "")
            {
                terminal.ShowNewText("<color=#ffffff>You found key for room " + locked_room + "</color>");
                currentKeyID = "";
                player_controller.IncrementVisionForRoom(currentRoom);
                EnterTheRoom();
            }
        }
        else
        {
            terminal.ShowNewText("<color=#ff7777>The key is protected by monsters!</color>");
            EnterTheRoom();
        }
    }

    public void TakeItem()
    {
        player_controller.TakeItem(currentItemType);
        gameManager.DeleteItem(currentItemID, currentRoom);
        currentItemType = "";
        currentItemID   = "";
        EnterTheRoom();
    }

    public void Quit()
    {
        gameManager.Quit();
    }

    public void ExitRoom()
    {
        currentState = GameState.InMaze;
        terminal.ShowNewText("You're leaving the " + currentRoom);
        currentRoom = "";
        currentMonstersAmount = -1;
        terminal.ShowNewTextLoading("Getting rooms list...\n");
        terminal.ShowRoomsLoading();
    }

    public void Attack()
    {
        currentState = GameState.InFight;
        terminal.ShowNewTextLoading("Started monster destroying process.\n");
    }

    public void Punch()
    {
        bool is_lower;
        if(gameManager.PunchMonster(currentMonsterID, currentRoom, currentSecrete, out is_lower))
        {
            terminal.ShowNewText("<color=#7777ff>Monster's been destroyed.</color>");
            EnterTheRoom();
        }
        else
        {
            if(is_lower)
            {
                terminal.ShowNewText("<color=#ff7777>The secrete is lower!</color>");
            }
            else
            {
                terminal.ShowNewText("<color=#ff7777>The secrete is higher!</color>");
            }
        }
        currentSecrete = -1;
    }

    public void RunAway()
    {
        terminal.ShowNewText("Ran away from monster.");
        terminal.ShowNewText("It's okay. You just need to prepare...\n");
        EnterTheRoom();
        currentMonsterID = "";
    }

    public string GetCurrentRoom()
    {
        return currentRoom;
    }

    public void SetCurrentSecrete(int secrete)
    {
        currentSecrete = secrete;
    }

    public void SetCurrentMonsterID(string monster_id)
    {
        currentMonsterID = monster_id;
    }

    public void SetCurrentKeyID(string key_id)
    {
        currentKeyID = key_id;
    }

    public void SetCurrentItemTypeAndID(string item_type, string item_id)
    {
        currentItemType = item_type;
        currentItemID   = item_id;
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


    public void EndGameAnimation()
    {
        StartCoroutine("end_game");
    }

    IEnumerator end_game()
    {
        for(int i = 0; i < 10; i++)
        {
            terminal.ShowNewText("<color=#ff7777>ERROR!</color>");
            yield return new WaitForSeconds(0.5f);
        }
    }
}
