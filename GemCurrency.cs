using System.Collections;
using UnityEngine;

public class GemCurrency : MonoBehaviour
{
    private float burstTime = 0.1f;
    private float timeToCornerMin = 0.75f;
    private float timeToCornerMax = 1.5f;
    private float timeToCorner;
    private float burstDistance = 1.25f;
    private Vector3 startPosition;
    private Vector3 positionAfterBurst;
    private Vector3 cornerPosition = new Vector3(-8.15f, 4.25f, 0);
    private GameObject gameManager;

    [HideInInspector] public int gemsToGive;
    [HideInInspector] public Vector3 burstDirection;

    [SerializeField] private AudioClip currencyCollect;

    private IEnumerator InitialBurst()
    {
        float timePassed = 0;

        while (timePassed < burstTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, positionAfterBurst, timePassed / burstTime);
        }

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveToCorner());
    }

    private IEnumerator MoveToCorner()
    {
        //Set variables
        float timePassed = 0f;
        bool soundEffectPlayed = false;

        SoundEffectManager soundEffectManager = gameManager.GetComponent<SoundEffectManager>();
        SetBattleUI setBattleUI = gameManager.GetComponent<SetBattleUI>();

        //Apply movement
        while (timePassed < timeToCorner)
        {
            yield return null;

            timePassed += Time.deltaTime;
            float moveProgression = Mathf.Clamp01(timePassed / timeToCorner);

            float easedMoveProgression = moveProgression < 0.5f ? 2f * moveProgression * moveProgression : 1f - Mathf.Pow(-2f * moveProgression + 2f, 2f) / 2f;

            transform.position = Vector3.Lerp(positionAfterBurst, cornerPosition, easedMoveProgression);

            //Play sound effect
            if (moveProgression > 0.9f && !soundEffectPlayed)
            {
                soundEffectManager.PlaySoundEffect(currencyCollect, 0.75f);
                soundEffectPlayed = true;
            }
        }

        setBattleUI.SetGemCurrencyUI(gemsToGive);
        Destroy(gameObject);
    }

    private void Awake()
    {
        //Set variables
        timeToCorner = Random.Range(timeToCornerMin, timeToCornerMax);
        startPosition = transform.position;
        gameManager = GameObject.Find("Game Manager");

        //Functions and coroutines
        StartCoroutine(InitialBurst());
    }

    private void Start()
    {
        //Set variables
        positionAfterBurst = startPosition + (burstDirection * burstDistance);
    }
}