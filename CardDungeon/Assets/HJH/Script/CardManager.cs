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
            case 5:
                if (cardIdx < 0)
                {
                    Idx5EnforceFunc(player);
                }
                else
                {
                    Idx5Func(player);
                }
                break;
            case 6:
                if (cardIdx < 0)
                {
                    Idx6EnforceFunc(player);
                }
                else
                {
                    Idx6Func(player);
                }
                break;
            case 7:
                if (cardIdx < 0)
                {
                    Idx7EnforceFunc(player);
                }
                else
                {
                    Idx7Func(player);
                }
                break;
            case 8:
                if (cardIdx < 0)
                {
                    Idx8EnforceFunc(player);
                }
                else
                {
                    Idx8Func(player);
                }
                break;
            case 9:
                if(cardIdx < 0)
                {
                    Idx9EnforceFunc(player.GetComponent<Player_HJH>());
                }
                else
                {
                    Idx9Func(player.GetComponent<Player_HJH>());
                }
                break;
        }

    }

    public bool OnCardCheck(Player_HJH player,int cardIdx)
    {
        bool ret = true;
        switch(Mathf.Abs(cardIdx))
        {
            case 1:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 2));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 1));
                }
                break;
                case 2:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + 2, (int)player.transform.position.y));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + 1, (int)player.transform.position.y));
                }
                break;
            case 3:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x - 2, (int)player.transform.position.y));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x - 1, (int)player.transform.position.y));
                }
                break;
            case 4:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 2));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 1));
                }
                break;
            case 5:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 2));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 1));
                }
                break;
            case 6:
                if (cardIdx < 0)
                {
                    ret = false;
                    for (int i = -3; i < 4; i++)
                    {
                        for (int j = -3; j < 4; j++)
                        {
                            if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + j, (int)player.transform.position.y + i)))
                            {
                                ret = true;
                            }
                        }
                    }
                }
                else
                {
                    ret = false;
                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++)
                        {
                            if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + j, (int)player.transform.position.y + i)))
                            {
                                ret = true;
                            }
                        }
                    }
                }
                break;
        }
        return ret;

    }


    //플레이어 위로 1칸 이동
    void Idx0Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 1)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 1),player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
        }
        Player_HJH p;
        if(player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 위로 2칸 이동
    void Idx0EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 2)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 2), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);

        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 오른쪽으로 1칸 이동
    void Idx1Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + 1, (int)player.transform.position.y)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x + 1, (int)player.transform.position.y ), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);

        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 오른쪽으로 2칸 이동
    void Idx1EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + 2, (int)player.transform.position.y)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x + 2, (int)player.transform.position.y), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x + 2, player.transform.position.y, player.transform.position.z);

        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 왼쪽으로 1칸 이동
    void Idx2Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x - 1, (int)player.transform.position.y)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x - 1, (int)player.transform.position.y), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x - 1, player.transform.position.y, player.transform.position.z);
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 왼쪽으로 2칸 이동
    void Idx2EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x - 2, (int)player.transform.position.y)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x - 2, (int)player.transform.position.y), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x - 2, player.transform.position.y, player.transform.position.z);
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 아래로 1칸 이동
    void Idx3Func(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 1)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y -1), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z);
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    //플레이어 아래로 2칸 이동
    void Idx3EnforceFunc(Transform player)
    {
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 2)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 2), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 2, player.transform.position.z);
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    // 플레이어가 5*5 안에서 랜덤하게 이동
    void Idx5Func(Transform player)
    {

        int x = Random.Range(-2, 3);
        int y = Random.Range(-2, 3);
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + x, (int)player.transform.position.y + y)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x + x, (int)player.transform.position.y + y), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x + x, player.transform.position.y + y, player.transform.position.z);
        }
        else
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + j, (int)player.transform.position.y + i)))
                    {
                        GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x + j, (int)player.transform.position.y + i), player.GetComponent<Player_HJH>());
                        player.transform.position = new Vector3(player.transform.position.x + j, player.transform.position.y + i, player.transform.position.z);
                    }
                }
            }
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    // 플레이어가 7*7 안에서 랜덤하게 이동
    void Idx5EnforceFunc(Transform player)
    {
        int x = Random.Range(-3, 4);
        int y = Random.Range(-3, 4);
        if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + x, (int)player.transform.position.y + y)))
        {
            GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x + x, (int)player.transform.position.y + y), player.GetComponent<Player_HJH>());
            player.transform.position = new Vector3(player.transform.position.x + x, player.transform.position.y + y, player.transform.position.z);
        }
        else
        {
            for (int i = -3; i < 4; i++)
            {
                for (int j = -3; j < 4; j++)
                {
                    if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)player.transform.position.x + j, (int)player.transform.position.y + i)))
                    {
                        GamePlayManager.Instance.gameBoard.Interact(new Vector2Int((int)player.transform.position.x + j, (int)player.transform.position.y + i), player.GetComponent<Player_HJH>());
                        player.transform.position = new Vector3(player.transform.position.x + j, player.transform.position.y + i, player.transform.position.z);
                    }
                }
            }
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("Walk");
        }
    }
    // 플레이어가 위아래 공격
    void Idx6Func(Transform player)
    {
        Vector2Int up = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 1);
        Vector2Int down = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 1);
        GamePlayManager.Instance.GoDamage(up, 1);
        GamePlayManager.Instance.GoDamage(down, 1);
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("SwingVertical");
        }
    }
    // 플레이어가 위아래 강하게공격
    void Idx6EnforceFunc(Transform player)
    {
        Vector2Int up = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y + 1);
        Vector2Int down = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y - 1);
        GamePlayManager.Instance.GoDamage(up, 2);
        GamePlayManager.Instance.GoDamage(down, 2);
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("SwingVertical");
        }
    }
    // 플레이어가 좌우 공격
    void Idx7Func(Transform player)
    {
        Vector2Int right = new Vector2Int((int)player.transform.position.x + 1, (int)player.transform.position.y);
        Vector2Int left = new Vector2Int((int)player.transform.position.x - 1, (int)player.transform.position.y);
        GamePlayManager.Instance.GoDamage(right, 1);
        GamePlayManager.Instance.GoDamage(left, 1);
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("SwingHorizontal");
        }
    }
    // 플레이어가 좌우 강하게공격
    void Idx7EnforceFunc(Transform player)
    {
        Vector2Int right = new Vector2Int((int)player.transform.position.x + 1, (int)player.transform.position.y);
        Vector2Int left = new Vector2Int((int)player.transform.position.x - 1, (int)player.transform.position.y);
        GamePlayManager.Instance.GoDamage(right, 2);
        GamePlayManager.Instance.GoDamage(left, 2);
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("SwingHorizontal");
        }
    }
    // 플레이어가 모든 방향 공격
    void Idx8Func(Transform player)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2Int vec = new Vector2Int((int)player.transform.position.x + i, (int)player.transform.position.y + j);
                GamePlayManager.Instance.GoDamage(vec, 1);
            }
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("SmashDown");
        }
    }
    // 플레이어가 모든방향 강하게공격
    void Idx8EnforceFunc(Transform player)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2Int vec = new Vector2Int((int)player.transform.position.x + i, (int)player.transform.position.y + j);
                GamePlayManager.Instance.GoDamage(vec, 2);
            }
        }
        Player_HJH p;
        if (player.TryGetComponent<Player_HJH>(out p))
        {
            p.animator.Play("SmashDown");
        }
    }
    //3초 안에 공격 들어오면 막기
    void Idx9Func(Player_HJH player)
    {
        player.ShieldOn(3);
    }
    //5초 안에 공격 들어오면 막기
    void Idx9EnforceFunc(Player_HJH player)
    {
        player.ShieldOn(5);
    }

}
