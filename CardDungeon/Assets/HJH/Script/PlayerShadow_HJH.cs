using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow_HJH : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public float shadowSpeed;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShadowOn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShadowOn()
    {
        Color color = playerSprite.color;
        while (true)
        {
            color.a -= shadowSpeed * Time.deltaTime;
            playerSprite.color = color;
            if(color.a <= 0)
            {
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }
}
