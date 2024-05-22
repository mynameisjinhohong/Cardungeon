using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow_HJH : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public Animator currentAnimator;
    public float shadowSpeed;
    // Start is called before the first frame update
    public void StartFadeOut(Animator getAnim)
    {
        currentAnimator.runtimeAnimatorController = getAnim.runtimeAnimatorController;
        StartCoroutine(ShadowOn());
    }

    IEnumerator ShadowOn()
    {
        Color color = playerSprite.color;
        
        currentAnimator.Play("Walk");
        
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
