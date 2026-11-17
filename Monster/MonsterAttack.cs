using System.Collections;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private float moveCancelThreshold = 0.3f;
    private float timeSinceLastAttack;
    private float autoAttackRandomVarianceMin = 0.85f;
    private float autoAttackRandomVarianceMax = 1f;
    private bool isPlayerMonster;
    private bool moveCancelled;
    private MoveData moveQueued;
    private Animator animator;
    private MonsterCombat monsterCombat;
    private GameObject gameManager;
    private CombatManager combatManager;
    private SoundEffectManager soundEffectManager;

    [HideInInspector] public Coroutine currentAutoAttackCoroutine;
    [HideInInspector] public float[] moveCharges;

    public GameObject moveCancel;
    [SerializeField] private AudioClip attackImpactSound;
    [SerializeField] private AudioClip attackWeakImpactSound;
    public AudioClip moveCancelSound;

    public void StartAutoAttackCoroutine()
    {
        currentAutoAttackCoroutine = StartCoroutine(AutoAttackCooldown());
    }

    public void StopAutoAttackCoroutine()
    {

        if (currentAutoAttackCoroutine == null) { return; }

        StopCoroutine(currentAutoAttackCoroutine);
        currentAutoAttackCoroutine = null;
    }

    private IEnumerator AutoAttackCooldown()
    {
        float timeToWait = Random.Range(monsterCombat.MonsterData.autoAttackInterval * autoAttackRandomVarianceMin, monsterCombat.MonsterData.autoAttackInterval * autoAttackRandomVarianceMax);
        yield return new WaitForSeconds(timeToWait);
        currentAutoAttackCoroutine = null;
        AutoAttack();
    }

    private void AutoAttack()
    {
        //Set animation
        if (isPlayerMonster) { animator.SetTrigger("playerAutoAttack"); }
        else { animator.SetTrigger("enemyAutoAttack"); }

        //Wait for cooldown before next auto attack
        StartAutoAttackCoroutine();
    }

    public void UseMove(MoveData moveToUse, int moveToUncharge)
    {
        animator.SetTrigger("playerMove");
        moveQueued = moveToUse;
        if (timeSinceLastAttack <= moveCancelThreshold) { MoveCancel(); }

        //Uncharge move
        moveCharges[moveToUncharge] = 0;
        monsterCombat.SetBattleUI.SetMoveChargeUI(moveCharges);

        StopAutoAttackCoroutine();
    }

    private void ApplyAttack()
    {
        if (monsterCombat.TargetMonster == null)
        {
            moveQueued = null;
            return;
        }

        if (moveQueued == null) { ApplyAutoAttack(); }
        else { ApplyMove(); }

        timeSinceLastAttack = 0;
        moveCancelled = false;
        soundEffectManager.PlaySoundEffect(attackImpactSound, 0.5f);
    }

    private void ApplyAutoAttack()
    {
        MonsterCombat targetCombat = monsterCombat.TargetMonster.GetComponent<MonsterCombat>();
        MonsterHealth targetHealth = monsterCombat.TargetMonster.GetComponent<MonsterHealth>();

        //Apply damage
        int damageToDeal = combatManager.DamageCalculate(monsterCombat.MonsterData.attack, targetCombat.MonsterData.defence);
        targetHealth.DamageMonster(damageToDeal);

        if (isPlayerMonster) { ChargeMoves(); }
    }

    private void ApplyMove()
    {
        MonsterCombat targetCombat = monsterCombat.TargetMonster.GetComponent<MonsterCombat>();
        MonsterHealth targetHealth = monsterCombat.TargetMonster.GetComponent<MonsterHealth>();
        MonsterStun targetStun = monsterCombat.TargetMonster.GetComponent<MonsterStun>();

        //Get user positive affinity status
        bool userPositiveAffinity = false;
        for (int i = 0; i < monsterCombat.MonsterData.elementalAffinities.Length; i++)
        {
            if (monsterCombat.MonsterData.elementalAffinities[i].element == moveQueued.element)
            {
                if (monsterCombat.MonsterData.elementalAffinities[i].affinityStatus == AffinityStatus.Positive) { userPositiveAffinity = true; }
                continue;
            }
        }

        //Get target negative affinity status
        bool targetNegativeAffinity = false;
        for (int i = 0; i < targetCombat.MonsterData.elementalAffinities.Length; i++)
        {
            if (targetCombat.MonsterData.elementalAffinities[i].element == moveQueued.element)
            {
                if (targetCombat.MonsterData.elementalAffinities[i].affinityStatus == AffinityStatus.Negative) { targetNegativeAffinity = true; }
                continue;
            }
        }

        //Apply damage
        int damageToDeal = combatManager.DamageCalculate(monsterCombat.MonsterData.attack, targetCombat.MonsterData.defence, moveQueued.power, moveCancelled, targetStun.isStunned, userPositiveAffinity, targetNegativeAffinity);
        targetHealth.DamageMonster(damageToDeal, Color.white, userPositiveAffinity, moveQueued.element, targetNegativeAffinity);

        moveQueued = null;
        if (targetHealth.healthCurrent > 0) { StartAutoAttackCoroutine(); }

        if (targetNegativeAffinity) { soundEffectManager.PlaySoundEffect(attackWeakImpactSound, 0.3f); }
    }

    private void ChargeMoves()
    {
        //Charge moves
        for (int i = 0; i < moveCharges.Length; i++)
        {
            if (moveCharges[i] < 1) { moveCharges[i] += (1f / monsterCombat.MonsterData.moves[i].recharge); }
        }
        monsterCombat.SetBattleUI.SetMoveChargeUI(moveCharges);
    }

    private void MoveCancel()
    {
        moveCancelled = true;
        Vector3 moveCancelPosition = monsterCombat.MainCamera.WorldToScreenPoint(new Vector3(0, -1, 0));
        GameObject newMoveCancel = Instantiate(moveCancel, moveCancelPosition, transform.rotation, monsterCombat.UserInterface);
        soundEffectManager.PlaySoundEffect(moveCancelSound, 1);

        ChargeMoves();
        combatManager.StartCoroutine(combatManager.DestroyMoveCancel(newMoveCancel));
    }

    private void Start()
    {
        //Set variables
        isPlayerMonster = CompareTag("PlayerMonster");
        animator = GetComponent<Animator>();
        monsterCombat = GetComponent<MonsterCombat>();
        gameManager = monsterCombat.GameManager;
        combatManager = gameManager.GetComponent<CombatManager>();
        soundEffectManager = gameManager.GetComponent<SoundEffectManager>();
        moveCharges = new float[3];

        //Functions and coroutines
        StartAutoAttackCoroutine();
    }

    private void Update()
    {
        //Increase time since last attack
        timeSinceLastAttack += Time.deltaTime;
    }
}