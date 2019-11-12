using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VisualController : MonoBehaviour
{
    [TextArea]
    public string objectDescription;
    public Sprite objectImage;

    Image visualSprite;
    Text  visualText;
    Animator visual;

    void  Start()
    {
        GameObject visualPart = GameObject.FindGameObjectWithTag("Visual");
        visualSprite = visualPart.GetComponentInChildren<Image>();
        visualText   = visualPart.GetComponentInChildren<Text>();
    }

    public void OnClick()
    {
        visualSprite.sprite = objectImage;
        visualSprite.color  = new Color(1f,1f,1f,0.9f);
        visualText.text     = objectDescription;
    }
}
