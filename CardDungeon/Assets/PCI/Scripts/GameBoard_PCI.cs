using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using Range = DG.DemiLib.Range;

public class GameBoard_PCI : MonoBehaviour
{
    [SerializeField]
    private Tile_PCI tilePrefab;
    [SerializeField]
    private TileObject_PCI bedrockPrefab;
    [SerializeField]
    private TileObject_PCI bedrockPrefab2;
    [SerializeField]
    private TileObject_PCI undestructablePrefab;
    [SerializeField]
    private TileObject_PCI blockPrefab;
    [SerializeField]
    private TileObject_PCI doorPrefab;
    [SerializeField]
    private Item_PCI itemPrefab;
    [SerializeField]
    private ItemBox_PCI itemBoxPrefab;
    [SerializeField]
    private ItemDataList_PCI itemList;
    [SerializeField]
    private List<Sprite> tileSprites = new List<Sprite>();

    [Header("Generate Settings")]
    [SerializeField]
    [Range(12, 16)]
    public int width, height;
    [SerializeField]
    [Range(1, 4)]
    private int padding;
    (int, int)[] offset;

    private Tile_PCI[,] board;

    public List<Transform> StartingPoints = new List<Transform>();

    public bool generate = false;

    private void Awake()
    {
        offset = new (int, int)[] { (padding, padding), (padding, height / 2), (padding, height - padding), (width / 2, padding), (width / 2, height - padding), (width - padding, padding), (width - padding, height / 2), (width - padding, height - padding) };
    }

    public void Generate(int seed, int headCount)
    {
        Generate(seed, width, height, headCount);
    }

    public void Generate(int seed, int width, int height, int headCount)
    {
        switch (headCount)
        {
            case 1:
                padding = 1; 
                break;
            case 2:
                padding = 1;
                break;
            case 4:
                padding = 2; 
                break;
            case 8:
                padding = 3; 
                break;
        }
        
        this.width = width;
        this.height = height;
        generate= true;
        
        offset = new (int, int)[]
        {
            // 서 북
            (padding, height - padding),
            // 동 남
            (width - padding, padding),
            // 서 남
            (padding, padding),
            // 동 북
            (width - padding, height - padding),
            // 서
            (padding, height / 2),
            // 동
            (width - padding, height / 2),
            // 남
            (width / 2, padding),
            // 북
            (width / 2, height - padding)
        };
        board = new Tile_PCI[width, height];
        
        int[,] tempBoard = new int[width, height];

        Random.InitState(seed);
        // Logic
        for(int i = 1; i < 9; i++)
        {
            (int, int) curPos = offset[i-1];
            StartingPoints[i - 1].position = new Vector3(curPos.Item1, curPos.Item2, 0);
            while (true)
            {
                tempBoard[curPos.Item1, curPos.Item2] = i;
                if (curPos == (width / 2, height / 2)) break;

                float rand = Random.Range(0f, 1f);
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
        
        // 맵의 안쪽으로 더 많이 타일을 채우도록 수정한 부분
        for (int i = 0; i < 8; i++)
        {
            (int, int) curPos = offset[i];
            
            tempBoard[curPos.Item1, curPos.Item2] = 1;
            // for (int x = -2; x <= 2; x++) // 채우는 범위를 확장
            // {
            //     for (int y = -2; y <= 2; y++) // 채우는 범위를 확장
            //     {
            //         if (Mathf.Abs(x + y) == 2) continue; // 기존 조건 유지
            //         tempBoard[curPos.Item1 + x, curPos.Item2 + y] = 1;
            //         
            //         Debug.Log($"{curPos.Item1 + x},{curPos.Item2 + y}");
            //     }
            // }
        }

        // Render
        for (int i = -5; i < width+5; i++)
        {
            for (int j = -5; j < height+5; j++)
            {
                // 범위 바깥에는 기반암 생성
                if(i < 0 || j < 0 || i >= width || j >= height+1)
                {
                    Instantiate(bedrockPrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                    continue;
                }
                if(j == height)
                {
                    Instantiate(bedrockPrefab2, new Vector3(i, j, 0), Quaternion.identity, transform);
                    continue;
                }
                // 기본 타일 생성
                var newTile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                int x = Mathf.Abs(i - width/2);
                int y = Mathf.Abs(j - height/2);
                int r = x*x + y*y;
                r = (128 * r) / ((width+height)*(width+height));
                int randi = Random.Range(0, 4);
                newTile.spriteRenderer.sprite = tileSprites[Mathf.Clamp(8-r + randi, 0, tileSprites.Count)];
                board[i, j] = newTile;
                if(tempBoard[i, j] == 0)
                {
                    // 10% 확률로 파괴 불가능한 오브젝트 생성
                    float randf = Random.value;
                    if(randf < 0.1f)
                    {
                        var newTileObject = Instantiate(undestructablePrefab, new Vector3(i, j, 0), Quaternion.identity, newTile.transform);
                        newTile.AddTileObject(newTileObject);
                    }
                    // 90% 확률로 파괴 가능한 오브젝트 생성
                    else
                    {
                        var newTileObject = Instantiate(blockPrefab, new Vector3(i, j, 0), Quaternion.identity, newTile.transform);
                        newTile.AddTileObject(newTileObject);
                    }
                }
            }
        }
        // Generate Items
        foreach (var item in itemList.itemDataList)
        {
            int k = 0;

            switch (headCount)
            {
                case 1:
                    k = item.amount;
                    break;
                case 2:
                    k = item.amount;
                    break;
                case 4:
                    k = (int)(item.amount * 2f);
                    break;
                case 8:
                    k = (int)(item.amount * 3f);
                    break;
            }
            
            while (k != 0)
            {
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);

                bool flag = true;
                for(int i = 0; i < 8; i++)
                {
                    if ((x, y) == offset[i]) { flag = false; break; }
                }
                if (flag)
                {
                    if (!IsInteractable(new Vector2Int(x, y)))
                    {
                        if (Random.value < 0.5f)
                        {
                            // 아이템 박스 생성
                            var targetTile = board[x, y];
                            var newItemObject = Instantiate(itemBoxPrefab, new Vector3(x, y, 0), Quaternion.identity, targetTile.transform);
                            newItemObject.SetData(item);
                            targetTile.AddTileObject(newItemObject);
                            newItemObject.itemBoxFrame.SetActive(true);
                            newItemObject.itemPotionFrame.SetActive(false);
                            
                            k--;
                        }
                        else
                        {
                            // 아이템 바로 생성
                            var targetTile = board[x, y];
                            var newItemObject = Instantiate(itemPrefab, new Vector3(x, y, 0), Quaternion.identity, targetTile.transform);
                            newItemObject.SetData(item);
                            targetTile.AddTileObject(newItemObject);
                            newItemObject.itemBoxFrame.SetActive(false);
                            newItemObject.itemPotionFrame.SetActive(true);
                            k--;
                        }
                    }
                }
            }
        }
        // 탈출구 생성
        var _tile = board[width / 2, height / 2];
        var door = Instantiate(doorPrefab, new Vector3(width / 2, height / 2, 0), Quaternion.identity, _tile.transform);
        _tile.AddTileObject(door);

        
        // 추격자토끼 생성
        bool isPositionFound = false;
        int chaserX = 0;
        int chaserY = 0;
        while (!isPositionFound)
        {
            int ranPosX = Random.Range(1, 4);
            int ranPosY = Random.Range(1, 4);

            chaserX = (width / 2) + ranPosX;
            chaserY = (height / 2) + ranPosY;

            // Check if the selected position is empty
            if (board[chaserX, chaserY].onTileObjects.Count == 0)
            {
                isPositionFound = true;
            }
        }
    
        var _chaser = board[chaserX, chaserY];
        var chaserObj = Instantiate(GamePlayManager.Instance.chaserObj, new Vector3(chaserX, chaserY, 0), Quaternion.identity, _chaser.transform);
        Debug.Log("추격자 토끼 생성완료");

        GamePlayManager.Instance.chaser = chaserObj.GetComponent<Chaser>();

        StartCoroutine(SuddenDeathTimer(GamePlayManager.Instance.chaser));
    }

    public void Clear()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                var item = board[i, j];

            }
        }
    }

    public void AddKeyObject(Vector2Int target)
    {
        var targetTile = board[target.x, target.y];
        var newTileObject = Instantiate(itemPrefab, new Vector3(target.x, target.y, 0), Quaternion.identity, targetTile.transform);
        ItemData_PCI data = null;
        foreach(var item in itemList.itemDataList)
        {
            if(item.name == "Key")
            {
                data = item;
            }
        }
        if(data == null)
        {
            Debug.LogError("열쇠 데이터를 찾을 수 없습니다");
            return;
        }
        newTileObject.SetData(data);
        targetTile.AddTileObject(newTileObject);
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
    
    public void InteractChaser(Vector2Int target, Chaser chaser)
    {
        if (!IsInteractable(target)) return;
        board[target.x, target.y].ChaserOnInteracted(chaser);
    }

    public bool SetTile(TileObject_PCI obj, int x, int y, int time)
    {
        Vector2Int vec = new Vector2Int(x, y);
        if (!IsValidCoordinate(vec)) return false;
        if(!IsPathable(vec)) return false;
        if (IsInteractable(vec)) return false;

        var newTileObject = Instantiate(obj, new Vector3(x, y, 0), Quaternion.identity, board[x, y].transform);
        Vine_PCI trap = null;
        if(newTileObject.TryGetComponent<Vine_PCI>(out trap))
        {
            trap.stunTime = time;
        }
        board[x, y].AddTileObject(newTileObject);

        return false;
    }

    public bool SetTile(TileObject_PCI obj, Vector2Int cord)
    {
        int x = cord.x;
        int y = cord.y;
        if (!IsValidCoordinate(cord)) return false;
        if (!IsPathable(cord)) return false;
        if (IsInteractable(cord)) return false;

        var newTileObject = Instantiate(obj, new Vector3(x, y, 0), Quaternion.identity, board[x, y].transform);
        board[x, y].AddTileObject(newTileObject);

        return false;
    }
    
    private IEnumerator SuddenDeathTimer(Chaser chaser)
    {
        int timeValue = Random.Range(1, 4);

        yield return new WaitForSeconds(timeValue * 10);

        chaser.gameObject.SetActive(true);

        Debug.Log("추격 시작");
        
        GamePlayManager.Instance.mainUi.chaserWarning.SetActive(true);

        yield return new WaitForSeconds(3);
        
        GamePlayManager.Instance.mainUi.chaserWarning.SetActive(false);
    }
}
