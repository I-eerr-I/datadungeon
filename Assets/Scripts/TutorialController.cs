using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public float waitTime      = 7f;
    public float pauseTime     = 2f;
    public float startTime     = 1f;

    TerminalController terminal;
    UIManager          uiManager;
    GameManager        gameManager;
    PlayerController   playerController;

    bool ended_part     = true;
    int  tutorial_index = 1;

    void Start()
    {
        terminal    = GameObject.FindGameObjectWithTag("Terminal").GetComponent<TerminalController>();
        uiManager   = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>(); 
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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
        if(tutorial_index == 2 && uiManager.GetEnteredTheRoom()) NextTutorial();
        
        if(tutorial_index == 3 && uiManager.GetAttackedMonster()) NextTutorial();
    
        if(tutorial_index == 4 && uiManager.GetPunchedMonster()) NextTutorial();

        if(tutorial_index == 5 && uiManager.GetUsedTrojan()) NextTutorial();

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

                case 4:
                StartCoroutine("run_tutorial4");
                break;

                case 5:
                StartCoroutine("run_tutorial5");
                break;
            }
        }
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
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
        yield return new WaitForSeconds(startTime);
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
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewTextLoading("See? Every time you follow the algorithm:");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#f77>1. CLICK on the COMMAND</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#ff7>2. CLICK on the OBJECT or TYPE text</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#3f3>3. Press ENTER</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Now. You are in the room alone with <color=#f77>MONSTERS</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Don't be afraid and fight one of them!");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("Select command <color=#f77>Attack</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("Click on the <color=#ff7>MONSTER</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("And <color=#1f1>ENTER</color> the fight!");
        uiManager.EnterTheRoom();
    }

    IEnumerator run_tutorial4()
    {
        ended_part = false;
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("Every <color=#f77>MONSTER</color> has his <color=#088>SECRET</color>");
        terminal.ShowNewText("<color=#088>SECRET</color> - number from 1 to 100");
        terminal.ShowNewText("The <color=#f77>MORE STRONGER</color> monster,");
        terminal.ShowNewText("the <color=#ff7>LESS SECRETS</color> it has");
        yield return new WaitForSeconds(waitTime*2);
        terminal.ShowNewTextLoading("They are <color=#FFC0CB>SHY</color> <color=#ff5555>TO DEATH</color> about secrets.");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("So you need just <color=#f77>PUNCH</color> them");
        terminal.ShowNewText("<color=#ff7>TYPE</color> secret in the appeared box");
        terminal.ShowNewText("<color=#1f1>ENTER</color> this secret");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Actualy, I can help you with this one.");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("Select <color=#f77>Punch</color> command.");
        long secret = gameManager.GetSecretMaximum(uiManager.GetCurrentMonsterID()) + 1;
        if(secret > 100)
        {
            secret = gameManager.GetSecretMinimum(uiManager.GetCurrentMonsterID()) - 1;
        }
        terminal.ShowNewText("And type in the box <color=#fff>"+secret.ToString()+"</color>");
        terminal.ShowNewText("It should <color=#f77>DESTROY</color> the beast!");
        uiManager.SetWaitForSecret(secret);
    }

    IEnumerator run_tutorial5()
    {
        ended_part = false;
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("... Oops... <color=#777>Sorry...</color>");
        terminal.ShowNewText("It just <color=#f77>HIT</color> you");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("That's okay. I'm gonna give you <color=#77f>TROJAN</color>");
        yield return new WaitForSeconds(pauseTime);
        playerController.TakeItem("trojan");
        terminal.ShowNewTextLoading("");
        terminal.ShowNewText("That's right!");
        terminal.ShowNewText("See? You have your inventory in bottom right corner.");
        terminal.ShowNewText("Now you have <color=#77f>TROJAN</color> to destroy");
        terminal.ShowNewText("<color=#f77>MONSTER</color> without fight!");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Use it with <color=#77f>Use Item</color> command");

        // NEXT TUTOR ABOUT NEXT MONSTER - BUGS AND WORMS - TAKING ITEMS
        // THEN TUTOR ABOUT KEYS, MAZE RANDOMNESS AND END
    }

    
}
