using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{

    public void ToCommandLine()
    {
        Text input = GameObject.FindGameObjectWithTag("Input").GetComponent<Text>();
        input.text = gameObject.GetComponent<CommandController>().command;
    }

    public void DebugOut()
    {
        Debug.Log("CLICK ON " + gameObject.name);
    }

}
