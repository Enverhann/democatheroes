using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    //Swipe
    private Vector2 firstTouch = Vector2.zero;
    private Vector2 lastTouch = Vector2.zero;
    private Vector2 tempPos;
    public float swipeAngle = 0;
    public float swipeFix = 1f;
    
    //Match Elements
    [Header("Match Elements")]
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    public int previousColumn;
    public int previousRow;
    public bool isMatched = false;
    private GameObject otherElement;
    private Board boardScript;
    private FindMatch findMatchScript;

    void Start()
    {
        boardScript = FindObjectOfType<Board>();
        findMatchScript = FindObjectOfType<FindMatch>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    void Update()
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 0.2f);
        }
        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {//Move toward target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if (boardScript.allElements[column, row] != this.gameObject)
            {
                boardScript.allElements[column, row] = this.gameObject;
                
            }
            findMatchScript.FindAllMatches();
        }
        else
        {//Set the position
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;           
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {//Move toward target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if (boardScript.allElements[column, row] != this.gameObject)
            {
                boardScript.allElements[column, row] = this.gameObject;

            }
            findMatchScript.FindAllMatches();
        }
        else
        {//Set the position
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(0.5f);
        if(otherElement != null)
        {
            if(!isMatched && !otherElement.GetComponent<Element>().isMatched)
            {
                otherElement.GetComponent<Element>().row = row;
                otherElement.GetComponent<Element>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                boardScript.moveState = GameState.move;
            }
            else
            {
                boardScript.DestroyMatch();
            }
            otherElement = null;
        }
    }

    private void OnMouseDown()
    {
        if (boardScript.moveState == GameState.move)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
    }

    private void OnMouseUp()
    {
        if (boardScript.moveState == GameState.move) {
            lastTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Angle();
        }     
    }

    void Angle()
    {
        if(Mathf.Abs(lastTouch.y-firstTouch.y) > swipeFix || Mathf.Abs(lastTouch.x - firstTouch.x) > swipeFix) { 
            swipeAngle = Mathf.Atan2(lastTouch.y - firstTouch.y, lastTouch.x - firstTouch.x) * 180 / Mathf.PI;            
            boardScript.moveState = GameState.wait;
            MovingPieces();
        }
        else
        {
            boardScript.moveState = GameState.move;
        }
    }

    void MovingPiecesAct(Vector2 direction)
    {
        otherElement = boardScript.allElements[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (otherElement != null)
        {
            otherElement.GetComponent<Element>().column += -1 * (int)direction.x;
            otherElement.GetComponent<Element>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMove());
        }
        else
        {
            boardScript.moveState = GameState.move;
        } 
    }

    void MovingPieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < boardScript.width - 1)
        {//Swiping Right
            MovingPiecesAct(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < boardScript.height - 1)
        {//Swiping Up
            MovingPiecesAct(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {//Swiping Left
            MovingPiecesAct(Vector2.left);
        }
        else if (swipeAngle <= -45 && swipeAngle >= -135 && row > 0)
        {//Swiping Down
            MovingPiecesAct(Vector2.down);
        }
        else
        {
            boardScript.moveState = GameState.move;
        }
        
    }
    void FindMatch()
    {
        if(column > 0 && column < boardScript.width - 1 )
        {
            GameObject leftElement1 = boardScript.allElements[column - 1, row];
            GameObject rightElement1 = boardScript.allElements[column + 1, row];
            if (leftElement1 != null && rightElement1 != null && leftElement1 != this.gameObject && rightElement1 != this.gameObject)
            {
                if(leftElement1.tag == this.gameObject.tag && rightElement1.tag == this.gameObject.tag)
            {
                leftElement1.GetComponent<Element>().isMatched = true;
                rightElement1.GetComponent<Element>().isMatched = true;
                isMatched = true;
            }
            }
        }
        if (row > 0 && row < boardScript.height - 1)
        {
            GameObject upElement1 = boardScript.allElements[column, row + 1];
            GameObject downElement1 = boardScript.allElements[column, row - 1];
            if(upElement1!=null && downElement1 != null && upElement1 != this.gameObject && downElement1 != this.gameObject)
            {
                if (upElement1.tag == this.gameObject.tag && downElement1.tag == this.gameObject.tag)
            {
                    upElement1.GetComponent<Element>().isMatched = true;
                    downElement1.GetComponent<Element>().isMatched = true;
                    isMatched = true;
            }
            }
        }
    }
}
