using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBoard_PCI : MonoBehaviour
{
    [SerializeField]
    private Tile_PCI tilePrefab;
    [SerializeField]
    private TileObject_PCI bedrockPrefab;
    [SerializeField]
    private TileObject_PCI undestructablePrefab;
    [SerializeField]
    private TileObject_PCI blockPrefab;
    [SerializeField]
    private Item_PCI itemPrefab;
    [SerializeField]
    private ItemDataList_PCI itemList;
    [SerializeField]
    private const int width = 40, height = 40;
    private const int padding = 4;
    (int, int)[] offset = { (padding, padding), (padding, height/2), (padding, height- padding), (width/2, padding), (width/2, height- padding), (width- padding, padding), (width- padding, height/2), (width- padding, height- padding) };

    private Tile_PCI[,] board = new Tile_PCI[width,height];

    public List<Transform> StartingPoints = new List<Transform>();
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
            StartingPoints[i - 1].position = new Vector3(curPos.Item1, curPos.Item2, 0);
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
        for (int i = 0; i < 8; i++)
        {
            (int, int) curPos = offset[i];
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    if (Mathf.Abs(x + y) == 4) continue;
                    tempBoard[curPos.Item1 + x, curPos.Item2 + y] = 1;
                }
            }
        }

        // Render
        for (int i = -5; i < width+5; i++)
        {
            for (int j = -5; j < height+5; j++)
            {
                if(i < 0 || j < 0 || i >= width || j >= height)
                {
                    Instantiate(bedrockPrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                    continue;
                }
                var newTile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                board[i, j] = newTile;
                if(tempBoard[i, j] == 0)
                {
                    float rand = UnityEngine.Random.value;
                    if(rand < 0.1f)
                    {
                        var newTileObject = Instantiate(undestructablePrefab, new Vector3(i, j, 0), Quaternion.identity, newTile.transform);
                        newTile.AddTileObject(newTileObject);
                    }
                    else
                    {
                        var newTileObject = Instantiate(blockPrefab, new Vector3(i, j, 0), Quaternion.identity, newTile.transform);
                        newTile.AddTileObject(newTileObject);
                    }
                }
            }
        }
        // Generate Items
        foreach (var e in itemList.itemDataList)
        {
            int k = e.amount;
            while (k != 0)
            {
                int x = UnityEngine.Random.Range(0, width);
                int y = UnityEngine.Random.Range(0, height);

                bool flag = true;
                for(int i = 0; i < 8; i++)
                {
                    if ((x, y) == offset[i]) { flag = false; break; }
                }
                if (!flag) continue;
                if (!IsInteractable(new Vector2Int(x, y)))
                {
                    var targetTile = board[x, y];
                    var newItemObject = Instantiate(itemPrefab, new Vector3(x, y, 0), Quaternion.identity, targetTile.transform);
                    newItemObject.SetData(e);
                    targetTile.AddTileObject(newItemObject);
                    k--;
                }
            }
        }
    }

    public TileObject_PCI AddTileObject(Vector2Int target, TileObject_PCI prefab)
    {
        var targetTile = board[target.x, target.y];
        var newTileObject = Instantiate(prefab, new Vector3(target.x, target.y, 0), Quaternion.identity, targetTile.transform);
        targetTile.AddTileObject(newTileObject);
        return newTileObject;
    }

    private bool IsValidCoordinate(Vector2Int target)
    {
        if(target.x < 0 || target.x >= width || target.y < 0 || target.y >= height)
        {
            return false;
        }
        return true;
    }
    public bool IsPathable(Vector2Int target)
    {
        if (!IsValidCoordinate(target)) return false;
        return board[target.x, target.y].IsPahtable();
    }

    public bool IsDestructable(Vector2Int target)
    {
        if (!IsValidCoordinate(target)) return false;
        return board[target.x, target.y].IsDestructable();
    }

    public bool IsInteractable(Vector2Int target)
    {
        if (!IsValidCoordinate(target)) return false;
        return board[target.x, target.y].IsInteractable();
    }

    public void Attack(Vector2Int target, Player_HJH player)
    {
        if (!IsDestructable(target)) return;
        board[target.x, target.y].OnDamaged(player);
    }

    public void Interact(Vector2Int target, Player_HJH player)
    {
        if (!IsInteractable(target)) return;
        board[target.x, target.y].OnInteracted(player);
    }
}
