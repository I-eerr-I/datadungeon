using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    UIManager          uiManager;
    TerminalController terminal;

    void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        terminal  = GameObject.FindGameObjectWithTag("Terminal").GetComponent<TerminalController>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Enter"))
        {
            GiveMonsterSecrete();
        }
    }

    public void GiveMonsterSecrete()
    {
        if(uiManager.GetLastRequest().Equals(UIManager.REQUEST_SECRETE))
        {
            InputField input = GetComponent<InputField>();
            input.enabled    = false;
            Text inputText   = input.gameObject.GetComponentInChildren<Text>();
            int secrete;
            if(!int.TryParse(inputText.text, out secrete))
            {
                terminal.ShowNewTextLoading("<color=#ff7777>[ERROR] DIGIT REQUIRED!</color>");
            }
            else
            {
                uiManager.SetCurrentSecrete(secrete);
                uiManager.GetLastPressed().TakeRequest(secrete.ToString());
                uiManager.Punch();
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
