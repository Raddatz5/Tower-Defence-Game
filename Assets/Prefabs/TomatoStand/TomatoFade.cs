using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TomatoFade : MonoBehaviour
{   
    SpriteRenderer spriteRenderer;
    [SerializeField] float destroyTimer = 5f;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] GameObject tomatoSplat;
    bool breakTheLoop = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }  
    void OnEnable()
    {   
        StartCoroutine(FadeOutSprite());
    }

     private IEnumerator FadeOutSprite()
    {   
        yield return new WaitForSeconds(destroyTimer);
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {   
            if(breakTheLoop)
            {
                break;
            }
            float t = elapsedTime / fadeDuration;
            spriteRenderer.color = Color.Lerp(startColor, Color.clear, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        spriteRenderer.color = startColor;
        breakTheLoop =false;
    }
    void OnDisable()
    {
        breakTheLoop = true;
    }
}
