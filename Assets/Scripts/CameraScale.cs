using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    private Board boardScript;

    //Camera Values
    public float offset;
    public float aspectRatio = 0.625f;
    public float padding;

    void Start()
    {
        boardScript = FindObjectOfType<Board>();
        if (boardScript != null)
        {
            PositionCamera(boardScript.width - 1, boardScript.height - 1);
        }
    }

    void PositionCamera(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2, y / 2, offset);
        transform.position = tempPos;
        if (boardScript.width >= boardScript.height)
        {
            Camera.main.orthographicSize = (boardScript.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = (boardScript.height / 2 + padding) + 2 * offset;
        }
    }
}
