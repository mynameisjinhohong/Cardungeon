using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public GameObject player;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //�÷��̾� ���� 1ĭ �̵�
    public void Idx0Func()
    {
        player.transform.position = new Vector3(player.transform.position.x,player.transform.position.y + 1,player.transform.position.z);
    }
    //�÷��̾� ���� 2ĭ �̵�
    public void Idx0EnforceFunc()
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);
    }
    //�÷��̾� ���������� 1ĭ �̵�
    public void Idx1Func()
    {
        player.transform.position = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);
    }
    //�÷��̾� ���������� 2ĭ �̵�
    public void Idx1EnforceFunc()
    {
        player.transform.position = new Vector3(player.transform.position.x + 2, player.transform.position.y, player.transform.position.z);
    }
    //�÷��̾� �������� 1ĭ �̵�
    public void Idx2Func()
    {
        player.transform.position = new Vector3(player.transform.position.x -1, player.transform.position.y, player.transform.position.z);
    }
    //�÷��̾� �������� 2ĭ �̵�
    public void Idx2EnforceFunc()
    {
        player.transform.position = new Vector3(player.transform.position.x -2 , player.transform.position.y, player.transform.position.z);
    }
    //�÷��̾� �Ʒ��� 1ĭ �̵�
    public void Idx3Func()
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z);
    }
    //�÷��̾� �Ʒ��� 2ĭ �̵�
    public void Idx3EnforceFunc()
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 2, player.transform.position.z);
    }

}
