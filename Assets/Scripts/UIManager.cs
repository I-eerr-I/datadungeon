using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("SQL Part")]
    public Text terminal;
    
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

    PlayerController player_controller;
    GameManager      gameManager;

    int    maxAmountOfCommands;
    bool   updateVisual;
    string currentRoom;

    void Start()
    {
        gameManager         = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        currentState        = GameState.InMaze;
        inputField.enabled  = false;
        updateVisual        = true;
        currentRoom         = null;
        savedState          = currentState;
        maxAmountOfCommands = Mathf.Max(inMazeCommands.Length, inRoomCommands.Length, inFightCommands.Length); 
    }

    public string GetCurrentRoom()
    {
        return currentRoom;
    }
    
    public void SetPlayerController(PlayerController player_controller)
    {
        this.player_controller = player_controller;
    }

    public void Update()
    {
        UpdateInventory();
        UpdateHealth();
        UpdateCharacteristics();
        CheckAndSetUpdates();
        if(updateVisual) UpdateVisual();
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
        for(int i = 0; i < commandsList.Length; i++)
        {
            GameObject instantiatedCommand = Instantiate(commandsList[i]);
            instantiatedCommand.transform.SetParent(commands.transform, false);
            RectTransform transformCommand = instantiatedCommand.GetComponent<RectTransform>();
            transformCommand.anchorMin = new Vector2(0, (maxAmountOfCommands-i-1)/(float)maxAmountOfCommands);
            transformCommand.anchorMax = new Vector2(1, (maxAmountOfCommands-i)/(float)maxAmountOfCommands);
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
