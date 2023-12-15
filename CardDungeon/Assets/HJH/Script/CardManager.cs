using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    public CardObject_HJH cardList;


    public void OnCardStart(Transform player, int cardIdx)
    {
        switch (cardIdx)
        {
            case 1:
                if (cardIdx < 0)
                {
                    Idx0EnforceFunc(player);
                }
                else
                {
                    Idx0Func(player);
                }
                break;
            case 2:
                if (cardIdx < 0)
                {
                    Idx1EnforceFunc(player);
                }
                else
                {
                    Idx1Func(player);
                }
                break;
            case 3:
                if (cardIdx < 0)
                {
                    Idx2EnforceFunc(player);
                }
                else
                {
                    Idx2Func(player);
                }
                break;
            case 4:
                if (cardIdx < 0)
                {
                    Idx3EnforceFunc(player);
                }
                else
                {
                    Idx3Func(player);
                }
                break;

        }

    }


    //플레이어 위로 1칸 이동
    void Idx0Func(Transform player)
    {
        if(GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x,(int)player.transform.position.y + 1)))
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
        }
    }
    //플레이어 위로 2칸 이동
    void Idx0EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 2)))
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);

        }
    }
    //플레이어 오른쪽으로 1칸 이동
    void Idx1Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x+1, (int)player.transform.position.y)))
        {
            player.transform.position = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);

        }
    }
    //플레이어 오른쪽으로 2칸 이동
    void Idx1EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + 2, (int)player.transform.position.y)))
        {
            player.transform.position = new Vector3(player.transform.position.x + 2, player.transform.position.y, player.transform.position.z);

        }
    }
    //플레이어 왼쪽으로 1칸 이동
    void Idx2Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x -1 , (int)player.transform.position.y)))
        {
            player.transform.position = new Vector3(player.transform.position.x - 1, player.transform.position.y, player.transform.position.z);
        }
    }
    //플레이어 왼쪽으로 2칸 이동
    void Idx2EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x - 2, (int)player.transform.position.y)))
        {
            player.transform.position = new Vector3(player.transform.position.x - 2, player.transform.position.y, player.transform.position.z);
        }
    }
    //플레이어 아래로 1칸 이동
    void Idx3Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 1)))
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z);
        }
    }
    //플레이어 아래로 2칸 이동
    void Idx3EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 2)))
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 2, player.transform.position.z);
        }
    }

}
