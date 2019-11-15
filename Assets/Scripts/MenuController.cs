using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject continueButton;

    void Start()
    {
        if(SaveSystem.Load() == null)
        {
            continueButton.SetActive(false);
        }
    }
}
