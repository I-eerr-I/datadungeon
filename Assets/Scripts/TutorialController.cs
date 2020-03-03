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
    public int  tutorial_index = 1;

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

        if(tutorial_index == 5 && uiManager.GetUsedBug()) NextTutorial();

        if(tutorial_index == 6 && uiManager.GetPunchedMonster()) NextTutorial();

        if(tutorial_index == 7 && uiManager.GetPunchedMonster()) NextTutorial();

        if(ended_part)
        {
            StartCoroutine("run_tutorial"+tutorial_index.ToString());
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
        terminal.ShowNewTextLoading("Started manual for <color=#ff5555>MONSTER-DESTROYER-PROGRAM</color>", true);
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("You're an AI that learned to <color=#ffff55>FIND</color> and <color=#ff5555>DESTROY</color> trash files");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("This files are <color=#ff5555>MONSTERS</color> of computers' data bases.", true);
        terminal.ShowNewText("Nobody, except <color=#fff>YOU</color>, can help people and their PCs");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("The data base is your dangerous <color=#5555ff>MAZE</color> with");
        terminal.ShowNewText("<color=#ffff55>ROOMS, KEYS, VIRUSES AND BUGS</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Now you are in the corridor. Let me show you visual rooms:");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowRoomsLoading();
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("You can <color=#fff>SEE</color> 2 rooms now."); 
        terminal.ShowNewText("It depends on your <color=#ffffff>VISION</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("The more <color=#fff>VISION</color> you have,");
        terminal.ShowNewText("the more objects become visual for you.");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Let me show you your commands =======>");
        uiManager.AviableShowRoomCommands();
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("Click on the command <color=#0f0>Enter the room</color>");
    }

    IEnumerator run_tutorial2()
    {
        ended_part = false;
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Now click on the <color=#fff>ROOM</color> you want to enter");
        terminal.ShowNewText("And then <color=#fff>ENTER</color> it by clicking <color=#fff>ENTER</color> button");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowRoomsLoading();
    }

    IEnumerator run_tutorial3()
    {
        ended_part = false;
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewTextLoading("See? Every time you follow the algorithm:");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#f55>1. CLICK on the COMMAND</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#ff5>2. CLICK on the OBJECT or TYPE text</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t<color=#0f0>3. Press ENTER</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("Now. You are in the room alone with <color=#f55>MONSTERS</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Don't be afraid and fight one of them!");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Select command <color=#f55>Attack</color>");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Click on the <color=#ff5>MONSTER</color>");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("And <color=#fff>ENTER</color> the fight!");
        uiManager.EnterTheRoom();
    }

    IEnumerator run_tutorial4()
    {
        ended_part = false;
        long secret = gameManager.GetSecretMaximum(uiManager.GetCurrentMonsterID()) + 1;
        if(secret > 100)
        {
            secret = gameManager.GetSecretMinimum(uiManager.GetCurrentMonsterID()) - 1;
        }
        uiManager.SetWaitForSecret(secret);
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("Every <color=#f55>MONSTER</color> has his <color=#170>SECRET</color>");
        terminal.ShowNewText("<color=#170>SECRET</color> - number from 1 to 100");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("The <color=#f55>MORE STRONGER</color> monster,");
        terminal.ShowNewText("the <color=#ff5>LESS SECRETS</color> it has");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("They are <color=#FFC0CB>SHY</color> <color=#f55>TO DEATH</color> about secrets.");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("So you need just <color=#f55>PUNCH</color> them");
        terminal.ShowNewText("<color=#ff5>TYPE</color> secret in the appeared box");
        terminal.ShowNewText("<color=#fff>ENTER</color> this secret");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewText("");
        terminal.ShowNewTextLoading("Actualy, I can help you with this one.");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Select <color=#f55>Punch</color> command.");
        
        terminal.ShowNewText("And type in the box <color=#fff>"+secret.ToString()+"</color>");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("It should <color=#f55>DESTROY</color> the beast!");
    }

    IEnumerator run_tutorial5()
    {
        ended_part = false;
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("... Oops... <color=#555>Sorry...</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("It's just <color=#f55>HIT</color> you");
        terminal.ShowNewText("Be careful. You're <color=#f55>OUT OF HEALTH</color>");
        terminal.ShowNewText("you're <color=#f55>DEAD</color>");
        yield return new WaitForSeconds(waitTime);
        terminal.ShowNewTextLoading("You can upgrade you <color=#f55>HEALTH</color> later.");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("You can find you <color=#f37>CURRENT HEALTH</color> under you <color=#fff>AVATAR</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("That's okay. I'm gonna give you a <color=#55f>BUG</color>");
        yield return new WaitForSeconds(pauseTime);
        playerController.TakeItem("bug");
        terminal.ShowNewText("");
        terminal.ShowNewText("That's right!");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("See? You have your <color=#5ff>INVENTORY</color> in bottom right corner =>");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Now you have <color=#55f>ONE BUG</color> to restore health");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("Use it with <color=#77f>Use Item</color> command");
    }

    IEnumerator run_tutorial6()
    {
        ended_part = false;
        long secret = gameManager.GetSecretMaximum(uiManager.GetCurrentMonsterID()) + 2;
        if(secret > 100)
        {
            secret = gameManager.GetSecretMinimum(uiManager.GetCurrentMonsterID()) - 2;
        }
        uiManager.SetWaitForSecret(secret);
        gameManager.SetMonsterMissNext();
        
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewTextLoading("Now you can <color=#f55>PUNCH MONSTER</color> with <color=#fff>"+secret.ToString()+"</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("Try it.");
    }

    IEnumerator run_tutorial7()
    {
        ended_part = false;
        long secret = gameManager.GetSecretMaximum(uiManager.GetCurrentMonsterID()) - 2;
        uiManager.SetWaitForSecret(secret);
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("<color=#555>...</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("I thought it was the right secret...");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("Actualy, the monster's just <color=#ff5>MISSED</color>...");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("The chance of it depends on you <color=#ff5>LUCK</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("And!!! Another good news! I've got <color=#fff>REAL SECRET</color>!");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("<color=#fff>"+secret.ToString()+"</color> - yes, it is.");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewTextLoading("I know, it hard to believe me now...");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("But I can give you infinite amount of <color=#55f>BUGS</color>");
        terminal.ShowNewText("<color=#030>if I've missed again</color>");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("So, just hit him with <color=#fff>" + secret.ToString() + "</color>");
    }

    IEnumerator run_tutorial8()
    {
        ended_part = false;
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewTextLoading("<color=#555>So... That's it...</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("<color=#555>Now you are by your own.</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("And last few details:");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("\t-- The <color=#ff5>MAZE</color> is always random.");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("\t-- Click on <color=#fff>EXIT</color> in room to <color=#fff>EXIT MAZE</color>");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("And last... The most important thing...");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("The last, the main thing i want to say...");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewTextLoading("");
        yield return new WaitForSeconds(pauseTime);
        terminal.ShowNewText("Please, Don't forget to press <color=#fff>ENTER</color> to finish command.");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("<color=#f15>With love.</color>");
        yield return new WaitForSeconds(startTime);
        terminal.ShowNewText("");
        terminal.ShowNewText("<color=#f15>Your daddy...</color>");
        yield return new WaitForSeconds(waitTime);
        uiManager.EnterTheRoom();
        gameManager.isTutorial = false;
    }

    
}
