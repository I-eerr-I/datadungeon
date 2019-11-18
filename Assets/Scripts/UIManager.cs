using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Mono.Data.Sqlite;

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
    public VisualAnimation visual;

    [Header("Command Buttons")]
    public GameObject[] inMazeCommands;
    public GameObject[] inRoomCommands;
    public GameObject[] inFightCommands;

    public enum GameState {InMaze, InRoom, InFight, InMazeExit};
    public GameState currentState;
    GameState savedState;
    
    public const string REQUEST_MONSTERID = "monster_id";
    public const string REQUEST_ROOMNAME  = "room_name";
    public const string REQUEST_SECRETE   = "secrete";
    public const string REQUEST_ITEMTYPE  = "item_type";
    public const string REQUEST_KEYID     = "key_id"; 

    CommandController lastPressed = null;
    string            lastRequest = "";

    Dictionary<string, bool> tutorialEvents = new Dictionary<string, bool>();
    long waitForSecret = -1;

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
    string currentUseItemType    = ""; 

    void Start()
    {
        gameManager           = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        terminal              = GameObject.FindWithTag("Terminal").GetComponent<TerminalController>();
        currentState          = GameState.InMaze;
        savedState            = currentState;
        maxAmountOfCommands   = Mathf.Max(inMazeCommands.Length, inRoomCommands.Length, inFightCommands.Length);

        tutorialEvents.Add("show_room_commands", false);
        tutorialEvents.Add("entered_the_room", false);
        tutorialEvents.Add("attacked_monster", false);
        tutorialEvents.Add("punched_monster", false);
        tutorialEvents.Add("used_trojan", false);
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            ResetAll();
        }
        if(Input.GetButtonDown("Enter"))
        {
            try
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

                        case CommandController.CommandType.UseItem:
                        UseItem();
                        break;

                        case CommandController.CommandType.ExitMaze:
                        ExitMaze();
                        break;

                        case CommandController.CommandType.SkipTutorial:
                        SkipTutorial();
                        break;

                    }
                    if(!lastPressed.commandType.Equals(CommandController.CommandType.Punch))
                    {
                        lastPressed = null;
                        lastRequest = "";
                    }
                }   
            }
            catch(SqliteException)
            {
                ResetAll();
            } 
        }

        UpdateInventory();
        UpdateHealth();
        UpdateCharacteristics();
        CheckAndSetUpdates();
        if(updateVisual) UpdateVisual();
    }

    public void ResetAll()
    {
        lastPressed = null;
        lastRequest = "";
        inputField.text  = "...";
    }

    public void SkipTutorial()
    {
        gameManager.SkipTutorial();
        ExitRoom();
        tutorialEvents["show_room_commands"] = true;
        updateVisual = true;
    }

    public void ExitMaze()
    {
        currentState = GameState.InMazeExit;
        terminal.ShowStatistics(gameManager.GetKilledMonsters());
    }

    public void EnterTheRoom()
    {
        if(gameManager.isTutorial) tutorialEvents["entered_the_room"] = true;
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

    public bool GetEnteredTheRoom()
    {
        return tutorialEvents["entered_the_room"];
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
        if(currentItemType == "" || currentItemID == "" || currentRoom == "") terminal.ShowError("TakeItem");
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
        if(gameManager.isTutorial) tutorialEvents["attacked_monster"] = true;
        currentState = GameState.InFight;
        terminal.ShowNewTextLoading("Started monster destroying process.\n");
    }

    public bool GetAttackedMonster()
    {
        return tutorialEvents["attacked_monster"];
    }

    public void UseItem()
    {
        if(gameManager.isTutorial && currentUseItemType.Equals("trojan")) tutorialEvents["used_trojan"];
        PlayerController.UsedItemType usedItemType = player_controller.UseItem(currentUseItemType, currentRoom);
        if(usedItemType == PlayerController.UsedItemType.Nothing)
        {
            terminal.ShowNewText("<color=#ff7777>CAN'T USE ITEM</color>");
        }
        if(usedItemType == PlayerController.UsedItemType.Bug)
        {
            terminal.ShowNewText("<color=#33ff33>You've restored your health.</color>");
        }
        if(usedItemType == PlayerController.UsedItemType.Trojan)
        {
            visual.PlayDeadAnimation();
            terminal.ShowNewText("<color=#7777ff>The monster has been destroied.</color>");
            EnterTheRoom();
        }
        if(usedItemType == PlayerController.UsedItemType.Worm)
        {
            terminal.ShowNewText("<color=#77ffff>Monster's secrete has been expanded.</color>");
        }
        currentUseItemType = "";
    }

    public bool GetUsedTrojan()
    {
        return tutorialEvents["used_trojan"];
    }

    public void Punch()
    {
        bool is_lower;
        if(gameManager.isTutorial) 
        {
            if(currentSecrete.Equals((int)waitForSecret))
                tutorialEvents["punched_monster"] = true;
            else
                terminal.ShowNewText("Believe me - enter <color=#ff7>"+waitForSecret.ToString()+"</color>");
        }
        if(gameManager.PunchMonster(currentMonsterID, currentRoom, currentSecrete, out is_lower))
        {
            visual.PlayDeadAnimation();
            terminal.ShowNewText("<color=#7777ff>Monster's been destroyed.</color>");
            EnterTheRoom();
        }
        else
        {
            visual.PlayMonsterAttackAnimation();
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

    public void SetWaitForSecret(long secret)
    {
        waitForSecret = secret;
    }

    public bool GetPunchedMonster()
    {
        return tutorialEvents["punched_monster"];
    }

    public void RunAway()
    {
        terminal.ShowNewText("Ran away from monster.");
        terminal.ShowNewText("It's okay. You just need to prepare...\n");
        EnterTheRoom();
        currentMonsterID = "";
    }

    public void MonsterMissed()
    {
        terminal.ShowNewTextLoading("<color=#ffff77>Monster's missed!</color>");
    }

    public string GetCurrentMonsterID()
    {
        return currentMonsterID;
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

    public void SetCurrentUseItemType(string item_type)
    {
        currentUseItemType = item_type;
    }

    public string GetLastRequest()
    {
        return lastRequest;
    }

    public CommandController GetLastPressed()
    {
        return lastPressed;
    }

    public void AviableShowRoomCommands()
    {
        updateVisual = true;
        tutorialEvents["show_room_commands"] = true;
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

            case GameState.InMazeExit:
            ClearCommands();
            break;
        }
        updateVisual = false;
    }

    void EnterCommandsIn(GameObject[] commandsList)
    {
        ClearCommands();
        foreach(GameObject command in commandsList)
        {
            CommandController commandController = command.GetComponent<CommandController>();
            if(commandController.commandType == CommandController.CommandType.SkipTutorial && !gameManager.isTutorial)
                continue;
            if(!tutorialEvents["show_room_commands"] && commandController.commandType == CommandController.CommandType.EnterTheRoom) 
                continue;
            GameObject instantiatedCommand = Instantiate(command);
            instantiatedCommand.transform.SetParent(commands.transform);
        }
    }

    void ClearCommands()
    {
        foreach(Transform initedCommand in commands.transform)
        {
            Destroy(initedCommand.gameObject);
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


    public void EndGameAnimation(int monster_killed)
    {
        IEnumerator coroutine = end_game(monster_killed);
        StartCoroutine(coroutine);
    }


    IEnumerator end_game(int monster_killed)
    {
        for(int i = 0; i < 10; i++)
        {
            terminal.ShowNewText("<color=#ff7777>ERROR!</color>");
            yield return new WaitForSeconds(0.1f);
        }
        terminal.ShowNewText("<color=#ffff77>You've killed " + monster_killed.ToString() + " monsters</color>");
    }
}
