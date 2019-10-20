using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int   health;
    public int   level;
    public int   levelPoints;
    public int   depthOfVision;
    public int   maxHealth;
    public float luck;

    public SaveData(PlayerController player_controller)
    {
        health        = player_controller.health;
        level         = player_controller.level;
        levelPoints   = player_controller.levelPoints;
        depthOfVision = player_controller.depthOfVision;
        maxHealth     = player_controller.maxHealth;
        luck          = player_controller.luck;
    }
}
