using System.Collections;
using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    private bool isPlayerMonster;

    private Coroutine currentWaterVeilCoroutine;
    private Coroutine currentBlazeBurnCoroutine;

    [HideInInspector] public AppliedStatusEffect[] appliedStatusEffects = new AppliedStatusEffect[4];

    private MonsterCombat monsterCombat;
    private MonsterHealth monsterHealth;
    private SetBattleUI setBattleUI;

    private Color waterVeilColor = new Color(0.36f, 0.36f, 0.9f);
    private Color blazeBurnColor = new Color(0.9f, 0.36f, 0.45f);

    public IEnumerator TickStatusEffect(AppliedStatusEffect appliedStatusEffect, float effectTime)
    {
        float timeRemaining = effectTime;

        while (timeRemaining > appliedStatusEffect.statusEffect.timePerTick)
        {
            yield return new WaitForSeconds(appliedStatusEffect.statusEffect.timePerTick);

            if (appliedStatusEffect.statusEffect.statusEffectType == StatusEffectType.WaterVeil) { monsterHealth.HealMonster(appliedStatusEffect.statusEffect.amountPerTick, waterVeilColor); }
            else { monsterHealth.DamageMonster(appliedStatusEffect.statusEffect.amountPerTick, blazeBurnColor); }

            timeRemaining -= appliedStatusEffect.statusEffect.timePerTick;
        }
    }

    private void StopWaterVeil()
    {
        if (currentWaterVeilCoroutine == null) { return; }

        StopCoroutine(currentWaterVeilCoroutine);
        currentWaterVeilCoroutine = null;
    }

    private void StopBlazeBurn()
    {
        if (currentBlazeBurnCoroutine == null) { return; }

        StopCoroutine(currentBlazeBurnCoroutine);
        currentBlazeBurnCoroutine = null;
    }

    public void ApplyStatusEffect(StatusEffectData statusEffectToApply, float timeToApply)
    {
        AppliedStatusEffect statusEffectApplying = new AppliedStatusEffect(statusEffectToApply, timeToApply);

        //Check to see if status is already on monster
        bool statusOnMonster = false;
        int existingStatusPosition = -1;
        for (int i = 0; i < appliedStatusEffects.Length; i++)
        {
            if (appliedStatusEffects[i] != null && appliedStatusEffects[i].statusEffect == statusEffectApplying.statusEffect)
            {
                statusOnMonster = true;
                existingStatusPosition = i;
                continue;
            }
        }

        //If not, find next clear spot
        if (!statusOnMonster)
        {
            for (int i = 0; i < appliedStatusEffects.Length; i++)
            {
                if (appliedStatusEffects[i] == null)
                {
                    appliedStatusEffects[i] = statusEffectApplying;
                    break;
                }
            }
        }

        //If it is, increase timer
        else
        {
            timeToApply += appliedStatusEffects[existingStatusPosition].timeRemaining;
            appliedStatusEffects[existingStatusPosition].timeRemaining = timeToApply;
            appliedStatusEffects[existingStatusPosition].maxTime = timeToApply;
        }

        switch (statusEffectToApply.statusEffectType)
        {
            case StatusEffectType.WaterVeil:
                StopWaterVeil();
                currentWaterVeilCoroutine = StartCoroutine(TickStatusEffect(statusEffectApplying, timeToApply));
                break;

            case StatusEffectType.BlazeBurn:
                StopBlazeBurn();
                currentBlazeBurnCoroutine = StartCoroutine(TickStatusEffect(statusEffectApplying, timeToApply));
                break;
        }
    }

    private void RemoveStatusEffect(int statusToRemove)
    {
        //Stop status effect coroutine
        if (appliedStatusEffects[statusToRemove].statusEffect.statusEffectType == StatusEffectType.WaterVeil) { StopWaterVeil(); }
        else { StopBlazeBurn(); }

        //Remove status effect
        appliedStatusEffects[statusToRemove] = null;

        //Move down other status effects
        for (int i = 0; i < appliedStatusEffects.Length; i++)
        {
            if (i > statusToRemove)
            {
                appliedStatusEffects[i - 1] = appliedStatusEffects[i];
                appliedStatusEffects[i] = null;
            }
        }
    }

    private void DecreaseStatusTimers()
    {
        for (int i = appliedStatusEffects.Length - 1; i >= 0; i--)
        {
            if (appliedStatusEffects[i] == null) { continue; }

            appliedStatusEffects[i].timeRemaining -= Time.deltaTime;
            if (appliedStatusEffects[i].timeRemaining <= 0) { RemoveStatusEffect(i); }
        }
    }

    private void UpdateStatusUI()
    {
        if (isPlayerMonster) { setBattleUI.SetPlayerStatusUI(); }
        else { setBattleUI.SetEnemyStatusUI(); }
    }

    private void Start()
    {
        //Set variables
        isPlayerMonster = CompareTag("PlayerMonster");
        monsterCombat = GetComponent<MonsterCombat>();
        monsterHealth = GetComponent<MonsterHealth>();
        setBattleUI = monsterCombat.SetBattleUI;
    }

    private void Update()
    {
        DecreaseStatusTimers();
        UpdateStatusUI();
    }
}