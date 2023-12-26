using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_HJH : MonoBehaviour
{
    public GameBoard_PCI gameBoard;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        int width = gameBoard.width;
        int height = gameBoard.height;
        transform.position = new Vector3(width / 2, height / 2, -10);
        int that = Mathf.Max(width, height);
        cam.orthographicSize = that / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
