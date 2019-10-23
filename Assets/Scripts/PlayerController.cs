using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Characteristics")]
    public int   level;
    public int   health;
    public int   levelPoints;
    public int   depthOfVision;
    public int   maxHealth;
    public float luck;

    [Header("Inventory")]
    public int keysAmount    = 0;
    public int bugsAmount    = 0;
    public int trojansAmount = 0;
    public int wormsAmount   = 0;

    int maxLevelPoints;

    // Minimum Characteristics
    int   minLevel          = 1;
    int   minLevelPoints    = 0;
    int   minMaxLevelPoints = 100; 
    int   minDepthOfVision  = 2;
    int   minMaxHealth      = 5;
    float minLuck           = 0.2f;

    // Upgrades
    int   depthOfVisionUpgrade = 1;
    int   maxHealthUpgrade     = 1;
    float luckUpgrade          = 0.1f;

    int amountOfVisiableRooms;

    Dictionary<string, int> visionForRoom = new Dictionary<string, int>();

    IDbCommand command;

    void Start()
    {
        amountOfVisiableRooms = depthOfVision;
        command = GameObject.FindWithTag("GameController").GetComponent<GameManager>().dbConnection.CreateCommand();
        command.CommandText = "INSERT INTO inventory(trojan, worm, bug) VALUES("+trojansAmount.ToString()+","+wormsAmount.ToString()+","+bugsAmount.ToString()+")";
        command.ExecuteNonQuery();
    }


    public void SetIDbCommand(IDbCommand command)
    {
        this.command = command;
    }

    public void SetStartCharacteristics()
    {
        level          = minLevel;
        levelPoints    = minLevelPoints;
        maxLevelPoints = minMaxLevelPoints * level;
        depthOfVision  = minDepthOfVision;
        maxHealth      = minMaxHealth;
        luck           = minLuck;
        health         = maxHealth;
    }

    public void SetCharacteristics(SaveData data)
    {
        this.level         = data.level;
        this.levelPoints   = data.levelPoints;
        this.depthOfVision = data.depthOfVision;
        this.maxHealth     = data.maxHealth;
        this.luck          = data.luck;
    }

    public void AddRoomForVision(string roomName)
    {
        if(!visionForRoom.ContainsKey(roomName))
            visionForRoom.Add(roomName, depthOfVision);
    }

    public void IncrementVisiableRooms()
    {
        amountOfVisiableRooms++;
    }

    public void IncrementVisionForRoom(string roomName)
    {
        visionForRoom[roomName]++;
    }

    public int GetVisionForRoom(string roomName)
    {
        return visionForRoom[roomName];
    }

    public int GetVisiableRooms()
    {
        return amountOfVisiableRooms;
    }
}
