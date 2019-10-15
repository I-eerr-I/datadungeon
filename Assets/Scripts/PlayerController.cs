using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int   level = 10;
    public int   levelPoints;
    public int   depthOfVision;
    public int   maxHealth;
    public float luck;

    int   minDepthOfVision = 2;
    int   minMaxHealth     = 5;
    float minLuck          = 0.2f;

    int   depthOfVisionUpgrade = 1;
    int   maxHealthUpgrade     = 1;
    float luckUpgrade          = 0.1f;

    public int GetMinDepthOfVision()
    {
        return minDepthOfVision;
    }

    public int GetMinMaxHealth()
    {
        return minMaxHealth;
    }

    public float GetMinLuck()
    {
        return minLuck;
    }
}
