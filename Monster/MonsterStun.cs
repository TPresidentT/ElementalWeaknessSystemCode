using System.Collections;
using UnityEngine;

public class MonsterStun : MonoBehaviour
{
    private float baseStunTime = 2.5f;
    private float stunTimeRemaining;
    private bool isPlayerMonster;
    [HideInInspector] public bool isStunned;
    private Coroutine currentStunStopCoroutine;

    private GameObject gameManager;

    private MonsterCombat monsterCombat;
    private MonsterAttack monsterAttack;
    private SetBattleUI setBattleUI;

    private Animator animator;

    [SerializeField] private GameObject offeringOrb;
    [SerializeField] private Sprite[] offeringOrbSprites;

    public void StunMonster()
    {
        //Set animation
        if (isPlayerMonster) { animator.SetTrigger("playerStun"); }
        else { animator.SetTrigger("enemyStun"); }

        isStunned = true;
        monsterAttack.StopAutoAttackCoroutine();

        if (currentStunStopCoroutine != null) { StopCoroutine(currentStunStopCoroutine); }
        currentStunStopCoroutine = StartCoroutine(WaitToStopStun());
    }

    private IEnumerator WaitToStopStun()
    {
        float maxStunTime = stunTimeRemaining + baseStunTime;
        stunTimeRemaining = maxStunTime;

        while (stunTimeRemaining > 0)
        {
            yield return null;
            stunTimeRemaining -= Time.deltaTime;
            float stunValue = stunTimeRemaining / maxStunTime;
            setBattleUI.SetMonsterStunUI(isPlayerMonster, stunValue);
        }
        StopStun();
    }

    private void StopStun()
    {
        if (!isStunned) { return; }

        animator.SetTrigger("stopStun");
        isStunned = false;
        monsterAttack.StartAutoAttackCoroutine();
    }

    public void ApplyStunEffect(ElementData moveElement)
    {
        moveElement.ApplyStunEffect(monsterCombat.TargetMonster, gameObject);
        if (moveElement.stunEffect.stunEffectSound != null)
        {
            gameManager.GetComponent<SoundEffectManager>().PlaySoundEffect(moveElement.stunEffect.stunEffectSound, moveElement.stunEffect.stunEffectSoundVolume);
        }
    }

    public void ApplyDivineOffering(int effectsToApply)
    {
        StartCoroutine(DivineOfferingEffect(effectsToApply));
    }

    private IEnumerator DivineOfferingEffect(int effectsToApply)
    {
        int effectsChosen = 0;

        float timeBetweenChoose = 0.35f;
        float timeBetweenApplyMin = 0.5f;
        float timeBetweenApplyMax = 0.9f;

        ElementData[] elementEffectsToApply = new ElementData[effectsToApply];
        GameObject[] newOfferingOrbs = new GameObject[effectsToApply];

        while (effectsChosen < effectsToApply)
        {
            ElementData effectToApply = monsterCombat.AllElements[Random.Range(0, monsterCombat.AllElements.Length)];
            if (effectToApply == monsterCombat.DivineElement) { continue; }

            yield return new WaitForSeconds(timeBetweenChoose);

            elementEffectsToApply[effectsChosen] = effectToApply;

            Vector3 positionToCreate = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            newOfferingOrbs[effectsChosen] = Instantiate(offeringOrb, positionToCreate, transform.rotation);
            newOfferingOrbs[effectsChosen].GetComponent<SpriteRenderer>().sprite = elementEffectsToApply[effectsChosen].offeringOrbSprite;

            effectsChosen += 1;
        }

        yield return new WaitForSeconds(timeBetweenChoose);

        for (int i = 0; i < effectsToApply; i++)
        {
            float timeToWait = Random.Range(timeBetweenApplyMin, timeBetweenApplyMax);
            if (monsterCombat.TargetMonster != null) { newOfferingOrbs[i].GetComponent<OfferingOrb>().StartOrbMovement(timeToWait, monsterCombat.TargetMonster.transform.position); }
            else { Destroy(newOfferingOrbs[i]); }

            yield return new WaitForSeconds(timeToWait);

            if (monsterCombat.TargetMonster != null) { monsterCombat.TargetMonster.GetComponent<MonsterStun>().ApplyStunEffect(elementEffectsToApply[i]); }
        }
    }

    private void Start()
    {
        //Set variables
        isPlayerMonster = CompareTag("PlayerMonster");
        monsterCombat = GetComponent<MonsterCombat>();
        monsterAttack = GetComponent<MonsterAttack>();
        gameManager = monsterCombat.GameManager;
        setBattleUI = monsterCombat.SetBattleUI;
        animator = GetComponent<Animator>();
    }
}