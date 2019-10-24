using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public Color color;
    public enum Theme {Custom, Black, DarkGreen, Green, Red};
    public Theme currentTheme;
    public Image background;

    [Header("Text")]
    public Color allTextColor;

    Color currentTextColor;
    Color savedColor;

    // Black Theme
    Color bgBlack = new Color(0, 0, 0, 1);

    // Dark Green Theme
    Color bgDarkGreen = new Color(0, 0.07f, 0, 1);

    // Green Theme
    Color bgGreen  = new Color(0, 0.1f, 0, 1);
    Color txtGreen = new Color(0.1764706f, 0.5764706f, 0.1215686f, 1);

    // Red Theme
    Color bgRed   = new Color(0.5f, 0, 0, 1);
    Color txtRed  = new Color(1, 0.5f, 0.5f, 1);

    bool isAttack = false;
    UIManager uiManager;

    void Start()
    {
        background       = GetComponent<Image>();
        background.color = color;
        currentTheme     = Theme.Custom;
        currentTextColor = allTextColor;
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    void Update()
    {
        // if(uiManager.currentState == UIManager.GameState.InFight)
        // {
        //     isAttack = true;
        //     savedColor = color;
        //     AssignColor(bgRed);
        //     ChangeTextColor(txtRed);
        // }
        // else
        // {
        //     isAttack = false;
        //     AssignColor(savedColor);
        //     ChangeTextColor(currentTextColor);
        // }

        if(!isAttack)
        {
            if(!currentTextColor.Equals(allTextColor))
            {
                currentTextColor = allTextColor;
                ChangeTextColor(currentTextColor);
            }

            if(!color.Equals(background.color))
            {
                currentTheme     = Theme.Custom;
                background.color = color;
            }
            else
            {
                if(currentTheme != Theme.Custom)
                {
                    switch (currentTheme)
                    {
                        case Theme.Black:
                        AssignColor(bgBlack);
                        break;
                        
                        case Theme.DarkGreen:
                        AssignColor(bgDarkGreen);
                        break;

                        case Theme.Green:
                        AssignColor(bgGreen);
                        break;
                    }
                }
            }
        }
    }

    void AssignColor(Color newColor)
    {
        background.color = newColor;
        color            = newColor;
    }

    void ChangeTextColor(Color textColor)
    {
        GameObject[] textes = GameObject.FindGameObjectsWithTag("Text");
        foreach(GameObject text in textes)
        {
            text.GetComponent<Text>().color = textColor;
        }
        GameObject.FindGameObjectWithTag("Text").GetComponent<Text>().color = textColor;
    }

}
