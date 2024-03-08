using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_HJH : MonoBehaviour
{
    public GameBoard_PCI gameBoard;
    bool setDone = false;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!setDone)
        {
            if (gameBoard.generate)
            {
                int width = gameBoard.width;
                int height = gameBoard.height;
                transform.position = new Vector3(width / 2, height / 2, -10);
                int that = Mathf.Max(width, height);
                cam.orthographicSize = that / 2;
                setDone= true;
            }
        }
    }
}
