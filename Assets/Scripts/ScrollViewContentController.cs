using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContentController : MonoBehaviour
{

    void Update()
    {
        foreach(Transform element in transform)
        {
            RectTransform rectTransform = element.gameObject.GetComponent<RectTransform>();
            if(rectTransform.anchoredPosition.y == -rectTransform.rect.height/2)
            {
                Destroy(element.gameObject);
            }
        }
    }
}
