using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int   level;
    public int   depthOfVision;
    public int   maxHealth;
    public float luck;

    public SaveData(PlayerController playerController)
    {
        level         = playerController.level;
        depthOfVision = playerController.depthOfVision;
        maxHealth     = playerController.maxHealth;
        luck          = playerController.luck;
    }
}
