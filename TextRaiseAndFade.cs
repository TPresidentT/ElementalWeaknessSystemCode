using System.Collections;
using UnityEngine;

public class TextRaiseAndFade : MonoBehaviour
{
    //Raise variables
    private float raiseSpeed = 125f;

    //Fade variables
    private bool canFade;
    private float fadeDelay = 0.75f;
    private float fadeSpeed = 3.25f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private IEnumerator WaitBeforeFade()
    {
        yield return new WaitForSeconds(fadeDelay);
        canFade = true;
    }

    private void Awake()
    {
        //Set variables
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        //Functions and coroutines
        StartCoroutine(WaitBeforeFade());
    }

    private void Update()
    {
        //Raise text
        rectTransform.anchoredPosition += Vector2.up * raiseSpeed * Time.deltaTime;

        //Fade text
        if (canFade) { canvasGroup.alpha -= fadeSpeed * Time.deltaTime; }

        //Destroy text
        if (canvasGroup.alpha <= 0) { Destroy(gameObject); }
    }
}