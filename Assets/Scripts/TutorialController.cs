using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public float waitTime      = 7f;
    public float pauseTime     = 2f;

    TerminalController terminal;
    UIManager          uiManager;

    bool ended_part     = true;
    int  tutorial_index = 1;

    void Start()
    {
        terminal    = GameObject.FindGameObjectWithTag("Terminal").GetComponent<TerminalController>();
        uiManager   = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>(); 
    }

    void Update()
    {
        if(tutorial_index == 1)
        {
            if(uiManager.GetLastPressed() != null && uiManager.GetLastPressed().commandType == CommandController.CommandType.EnterTheRoom)
            {
                NextTutorial();
            }
        }
        if(tutorial_index == 2)
        {
            if(uiManager.GetCurrentRoom() != "")
            {
                NextTutorial();
            }
        }
        if(ended_part)
        {
            switch(tutorial_index)
            {
                case 1:
                StartCoroutine("run_tutorial1");
                break;

                case 2:
                StartCoroutine("run_tutorial2");
                break;

                case 3:
                StartCoroutine("run_tutorial3");
                break;
            }
        }
    }

    void NextTutorial()
    {
        tutorial_index++;
        ended_part = true;
    }

    IEnumerator run_tutorial1()
    {
        ended_part = false;
        terminal.ShowNewTextLoading("Started manual for <color=#ff7777>MONSTER-DESTROYER-PROGRAM</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("You're an AI that learned to <color=#ffff77>find</color> and <color=#ff7777>destroy</color> trash files");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("This files are <color=#ff7777>MONSTERS</color> of computers' data bases.");
        terminal.ShowNewText("Nobody, except <color=#fff>you</color>, can help people and their PCs");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("The data base is your dangerous <color=#7777ff>MAZE</color> with");
        terminal.ShowNewText("<color=#ffff77>ROOMS, KEYS, VIRUSES and BUGS</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Now you are in the corridor. Let me show you visual rooms:");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowRoomsLoading();
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("You can <color=#ffffff>SEE</color> 2 rooms now."); 
        terminal.ShowNewText("It depends on your <color=#ffffff>Depth Of Vision</color>");
        terminal.ShowNewText("The more Depth of Vision you have,");
        terminal.ShowNewText("the more objects become visual for you.");
        yield return new WaitForSeconds(2*waitTime);
        terminal.ShowNewTextLoading("Let me show you your commands ->");
        uiManager.AviableShowRoomCommands();
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("Click on the command <color=#77f>\"Enter the room\"</color>");
    }

    IEnumerator run_tutorial2()
    {
        ended_part = false;
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("Now click on the <color=#77f>ROOM</color> you want to enter");
        terminal.ShowNewText("And then <color=#77f>ENTER</color> it by clicking <color=#77f>ENTER</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowRoomsLoading();
    }

    IEnumerator run_tutorial3()
    {
        ended_part = false;
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("See? Every time you follow the algorithm:");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#f77>1. Click on the command</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#ff7>2. Click on the object (if the command needs)</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#3f3>3. Press ENTER</color>");
        yield return new WaitForSeconds(waitTime);
        // CONTINUE HERE
    }

    
}
