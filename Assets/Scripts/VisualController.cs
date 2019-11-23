using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VisualController : MonoBehaviour
{
    [TextArea]
    public string objectDescription;
    public Sprite objectImage;
    public bool   levelDependent;
    public bool   gameManagerConstant;
    public string constant;
    public int    level;
    public Image  spriteImage;
    public Sprite lowLevelSprite;
    public Text   textObject;
    public string lowLevelText;

    int   playerLevel;
    Image visualSprite;
    Text  visualText;

    void  Start()
    {
        GameObject visualPart = GameObject.FindGameObjectWithTag("Visual");
        visualSprite          = visualPart.GetComponentInChildren<Image>();
        visualText            = visualPart.GetComponentInChildren<Text>();
        if(levelDependent)
        {
            if(gameManagerConstant)
            {
                if(constant == "worm")
                {
                    level = GameManager.WORM_LEVEL;
                }
                if(constant == "trojan")
                {
                    level = GameManager.TROJAN_LEVEL;
                }
            }
            playerLevel = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().GetPlayerController().level;
            if(playerLevel < level)
            {
                objectDescription  = "<color=#f00>CLOSED TILL LEVEL "+level.ToString()+"</color>";
                spriteImage.sprite = lowLevelSprite;
                objectImage        = lowLevelSprite;
                textObject.text    = lowLevelText;
            }
        }
    }

    public void OnClick()
    {
        visualSprite.sprite = objectImage;
        visualSprite.color  = new Color(1f,1f,1f,0.9f);
        visualText.text     = objectDescription;
    }
}
