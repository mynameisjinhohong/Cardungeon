using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBoard_PCI : MonoBehaviour
{
    [SerializeField]
    private Tile_PCI tilePrefab;
    [SerializeField]
    private TileObject_PCI blockPrefab;
    [SerializeField]
    private const int width = 40, height = 40;
    (int, int)[] offset = { (2, 2), (2, height/2), (2, height-2), (width/2, 2), (width/2, height-2), (width-2, 2), (width-2, height/2), (width-2, height-2) };

    private Tile_PCI[,] board = new Tile_PCI[width,height];

    private void Awake()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        Generate(rand);
    }
    private void Generate(int seed)
    {
        int[,] tempBoard = new int[width, height];

        UnityEngine.Random.InitState(seed);
        // Logic
        for(int i = 1; i < 9; i++)
        {
            (int, int) curPos = offset[i-1];
            while (true)
            {
                tempBoard[curPos.Item1, curPos.Item2] = i;
                if (curPos == (width / 2, height / 2)) break;

                float rand = UnityEngine.Random.Range(0f, 1f);
                (int, int) nextPos;
                if (rand < 0.25f)
                {
                    nextPos = (Mathf.Clamp(curPos.Item1 + 1, 0, width - 1), curPos.Item2);
                } else if (rand < 0.5f)
                {
                    nextPos = (Mathf.Clamp(curPos.Item1 - 1, 0, width - 1), curPos.Item2);
                } else if (rand < 0.75f)
                {
                    nextPos = (curPos.Item1, Mathf.Clamp(curPos.Item2 + 1, 0, height - 1));
                }
                else
                {
                    nextPos = (curPos.Item1, Mathf.Clamp(curPos.Item2 - 1, 0, height - 1));
                }
                int nextRegion = tempBoard[nextPos.Item1, nextPos.Item2];
                if (nextRegion != 0 && nextRegion != i)
                {
                    break;
                }
                curPos = nextPos;
            }
        }

        // Render
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var newTile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                board[i, j] = newTile;
                if(tempBoard[i, j] == 0)
                {
                    var newTileObject = Instantiate(blockPrefab, new Vector3(i, j, 0), Quaternion.identity, newTile.gameObject.transform);
                    newTile.AddTileObject(newTileObject);
                }
            }
        }
    }

    public bool IsPathable(Vector2Int target)
    {
        return true;
    }
}
