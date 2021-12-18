using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatch : MonoBehaviour
{
    private Board boardScript;
    public List<GameObject> matches = new List<GameObject>();

    void Start()
    {
        boardScript = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindMatches());
    }

    private void AddToListAndMatch(GameObject element)
    {
        if (!matches.Contains(element))
        {
            matches.Add(element);
        }
        element.GetComponent<Element>().isMatched = true;
    }

    private void GetNearShapes(GameObject element1, GameObject element2, GameObject element3)
    {
        AddToListAndMatch(element1);
        AddToListAndMatch(element2);
        AddToListAndMatch(element3);
    }

    private IEnumerator FindMatches()
    {
        //yield return new WaitForSeconds(0.2f);
        yield return null;
        for(int i = 0; i < boardScript.width; i++)
        {
            for(int j = 0; j < boardScript.height; j++)
            {
                GameObject currentElement = boardScript.allElements[i, j];
                if(currentElement != null)
                {
                    if(i>0 && i < boardScript.width - 1)
                    {
                        GameObject leftElement = boardScript.allElements[i - 1, j];
                        GameObject rightElement = boardScript.allElements[i + 1, j];
                        if(leftElement!=null && rightElement != null)
                        {
                            if(leftElement.tag == currentElement.tag && rightElement.tag == currentElement.tag)
                            {
                                GetNearShapes(leftElement, rightElement, currentElement);                               
                            }
                        }
                    }
                    if (j > 0 && j < boardScript.height - 1)
                    {
                        GameObject upElement = boardScript.allElements[i, j + 1];
                        GameObject downElement = boardScript.allElements[i, j - 1];
                        if (upElement != null && downElement != null)
                        {
                            if (upElement.tag == currentElement.tag && downElement.tag == currentElement.tag)
                            {
                                GetNearShapes(upElement, downElement, currentElement);
                            }
                        }
                    }
                }
            }
        }
    }
}
