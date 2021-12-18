using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {wait, move}


public class Board : MonoBehaviour
{
    public GameState moveState = GameState.move;
    
    //Board-Elements
    public int width;
    public int height;
    public int fall;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] elements;
    public GameObject[,] allElements;

    //Score
    private ScoreManager scoreManagerScript;
    public int baseValue = 20;

    private FindMatch findMatchScript;
    public bool isSpawnable1, isSpawnable2, isSpawnable3, isSpawnable4, isSpawnable5, isSpawnable6, isSpawnable7, isSpawnable8;
    public float refillDelay = 0.5f;
    public string[] tagArray = new string[4];
    

    void Start()
    {
        scoreManagerScript = FindObjectOfType<ScoreManager>();
        findMatchScript = FindObjectOfType<FindMatch>();
        allTiles = new BackgroundTile[width, height];
        allElements = new GameObject[width, height];
        SetupBoard();
    }
    
    private void SetupBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2 (i, j);
                GameObject tile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                tile.transform.parent = this.transform;
                tile.name = "(" + i + "," + j + ")";
                int elementToUse = Random.Range(0, elements.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                {
                    elementToUse = Random.Range(0, elements.Length);
                    maxIterations++;
                }
                maxIterations = 0;
                GameObject element = Instantiate(elements[elementToUse], tempPos, Quaternion.identity);
                element.GetComponent<Element>().row = j;
                element.GetComponent<Element>().column = i;
                element.transform.parent = this.transform;
                element.name = "(" + i + "," + j + ")";
                allElements[i, j] = element;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject shape)
    {
        if(column > 1 && row > 1)
        {
            if (allElements[column - 1, row] != null && allElements[column - 2, row] != null)
            {
                if (allElements[column - 1, row].tag == shape.tag && allElements[column - 2, row].tag == shape.tag)
                {
                    return true;
                }
            }
            if (allElements[column , row-1] != null && allElements[column , row-2] != null)
            {
                if (allElements[column, row - 1].tag == shape.tag && allElements[column, row - 2].tag == shape.tag)
                {
                    return true;
                }
            }
                
        }else if(column<=1 || row <= 1)
        {
            if (row > 1)
            {
                if (allElements[column, row - 1] != null && allElements[column, row - 2] != null)
                {
                    if (allElements[column, row - 1].tag == shape.tag && allElements[column, row - 2].tag == shape.tag)
                    {
                        return true;
                    }
                }               
            }
            if (column > 1)
            {
                if (allElements[column - 1, row] != null && allElements[column - 2, row] != null)
                {
                    if (allElements[column - 1, row].tag == shape.tag && allElements[column - 2, row].tag == shape.tag)
                    {
                        return true;
                    }
                }                  
            }
        }

        return false;
    }

    private void DestoyMatchesAt(int column, int row)
    {
        if (allElements[column, row].GetComponent<Element>().isMatched)
        {
            Destroy(allElements[column, row]);
            scoreManagerScript.AddScore(baseValue);
            allElements[column, row] = null;
        }
    }

    public void DestroyMatch()
    {
        for (int i =0; i<width; i++)
        {
            for(int j=0; j < height; j++)
            {
                if (allElements[i, j] != null)
                {
                    DestoyMatchesAt(i, j);
                }
            }
        }
        findMatchScript.matches.Clear();
        StartCoroutine(CollapseRow());
    }

    private IEnumerator CollapseRow()
    {
        int nullCount = 0;
          yield return new WaitForSeconds(0.05f);
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allElements[i,j] == null)
                {
                    nullCount++;
                }else if(nullCount > 0){
                    allElements[i, j].GetComponent<Element>().row -= nullCount;
                    allElements[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoard());
    }

    private void Refill()
    {
        for(int i =0; i < width; i++)
        {
            for(int j =0; j < height; j++)
            {
                if (allElements[0, j] == null && isSpawnable1)
                {
                    Vector2 tempPos = new Vector2(0, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[0, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 0;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 0 + "," + j + ")";
                }
                if (allElements[1,j] == null && isSpawnable2)
                {
                    Vector2 tempPos = new Vector2(1, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[1, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 1;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 1 + "," + j + ")";
                }
                if (allElements[2, j] == null && isSpawnable3)
                {
                    Vector2 tempPos = new Vector2(2, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[2, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 2;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 2 + "," + j + ")";
                }
                if (allElements[3, j] == null && isSpawnable4)
                {
                    Vector2 tempPos = new Vector2(3, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[3, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 3;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 3 + "," + j + ")";
                }
                if (allElements[4, j] == null && isSpawnable5)
                {
                    Vector2 tempPos = new Vector2(4, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[4, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 4;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 4 + "," + j + ")";
                }
                if (allElements[5, j] == null && isSpawnable6)
                {
                    Vector2 tempPos = new Vector2(5, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[5, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 5;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 5 + "," + j + ")";
                }
                if (allElements[6, j] == null && isSpawnable7)
                {
                    Vector2 tempPos = new Vector2(6, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[6, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 6;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 6 + "," + j + ")";
                }
                if (allElements[7, j] == null && isSpawnable8)
                {
                    Vector2 tempPos = new Vector2(7, j + fall);
                    int elementToUse = Random.Range(0, tagArray.Length);
                    string randomTag = tagArray[elementToUse];
                    GameObject shape = MyPooler.ObjectPooler.Instance.GetFromPool(randomTag, tempPos, Quaternion.identity);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, elements[elementToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        elementToUse = Random.Range(0, elements.Length);
                    }
                    maxIterations = 0;
                    allElements[7, j] = shape;
                    shape.GetComponent<Element>().row = j;
                    shape.GetComponent<Element>().column = 7;
                    shape.transform.parent = this.transform;
                    shape.name = "(" + 7 + "," + j + ")";
                }               
            }
        }
    }

    private bool MatchBoard()
    {
        findMatchScript.FindAllMatches();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] != null)
                {
                    if (allElements[i, j].GetComponent<Element>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        yield return new WaitForSeconds(refillDelay);
        Refill();
        yield return new WaitForSeconds(refillDelay);
        while (MatchBoard())
        {
            DestroyMatch();
            yield break;
        }

        yield return new WaitForSeconds(refillDelay);
        moveState = GameState.move;
    }
}
