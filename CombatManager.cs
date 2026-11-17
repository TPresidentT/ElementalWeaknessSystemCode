using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public GameObject playerMonster;
    public GameObject enemyMonster;

    private MonsterCombat playerMonsterCombat;
    private MonsterStun playerMonsterStun;
    private MonsterAttack playerMonsterAttack;
    private MonsterData playerMonsterData;

    private PlayerInput playerInput;

    private float damageRandomVarianceMin = 0.85f;
    private float damageRandomVarianceMax = 1f;

    [SerializeField] private MonsterData[] allMonsters;
    [SerializeField] private GameObject monsterObject;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform userInterface;

    //Modifier values
    private float moveCancelModifier = 1.25f;
    private float stunModifier = 1.5f;
    private float positiveAffinityModifier = 1.25f;
    private float negativeAffinityModifier = 2f;

    public int DamageCalculate(int monsterAttack, float targetDefence, int movePower = 0, bool moveCancelled = false, bool targetStunned = false, bool userPositiveAffinity = false, bool targetNegativeAffinity = false)
    {
        //Calculate damage before modifiers
        float damageRandomVariance = Random.Range(damageRandomVarianceMin, damageRandomVarianceMax);

        int totalPower = monsterAttack + movePower;

        float damageBeforeModifiers = (float)totalPower * (1 - targetDefence) * damageRandomVariance;

        //Apply modifiers
        float damageAfterModifiers = damageBeforeModifiers;

        if (moveCancelled) { damageAfterModifiers *= moveCancelModifier; }
        if (targetStunned) { damageAfterModifiers *= stunModifier; }
        if (userPositiveAffinity) { damageAfterModifiers *= positiveAffinityModifier; }
        if (targetNegativeAffinity) { damageAfterModifiers *= negativeAffinityModifier; }

        int finalDamage = (int)damageAfterModifiers;

        return finalDamage;
    }

    private void CreateRandomEnemyMonster()
    {
        Vector3 enemyPosition = new Vector3(5f, -2.5f, 0f);
        MonsterData monsterToCreateData = allMonsters[Random.Range(0, allMonsters.Length)];

        GameObject newMonster = Instantiate(monsterObject, enemyPosition, transform.rotation);
        newMonster.GetComponent<Animator>().SetTrigger("enemyFade");
        newMonster.GetComponent<MonsterCombat>().MonsterInitialise(monsterToCreateData, mainCamera, userInterface, this.gameObject, playerMonster);
        enemyMonster = newMonster;
        playerMonsterCombat.SetNewTarget(newMonster);
        GetComponent<SetBattleUI>().SetMonsterStunUI(false, 0);
    }

    public IEnumerator NextBattle()
    {
        Time.timeScale = 0.25f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(3f);

        CreateRandomEnemyMonster();
        playerMonsterAttack.StartAutoAttackCoroutine();

        int changeBattleBackground = Random.Range(0, 2);
        if (changeBattleBackground == 0) { StartCoroutine(GetComponent<GameManager>().ChangeBattleBackground()); }
    }

    public IEnumerator RestartGame()
    {
        Time.timeScale = 0.25f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(0);
    }

    public void EndBattle(bool playerWin)
    {
        if (playerWin)
        {
            GetComponent<GameManager>().CreateGemCurrency(Random.Range(10, 15 + 1));
            StartCoroutine(NextBattle());
        }
        else
        {
            StartCoroutine(RestartGame());
        }
    }

    public IEnumerator DestroyMoveCancel(GameObject moveCancelToDestroy)
    {
        float timeToDestroy = 1.5f;
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(moveCancelToDestroy);
    }

    private void Awake()
    {
        //Set variables
        playerMonsterCombat = playerMonster.GetComponent<MonsterCombat>();
        playerMonsterStun = playerMonster.GetComponent<MonsterStun>();
        playerMonsterAttack = playerMonster.GetComponent<MonsterAttack>();
        playerMonsterData = playerMonsterCombat.MonsterData;
        playerInput = GetComponent<PlayerInput>();

        //Functions and coroutines
        CreateRandomEnemyMonster();
    }

    private void Update()
    {
        //Check to use moves
        bool canUseMove = playerMonsterAttack.currentAutoAttackCoroutine != null && !playerMonsterStun.isStunned;
        if (playerInput.actions["Move1"].WasPressedThisFrame() && playerMonsterAttack.moveCharges[0] >= 1 && canUseMove) { playerMonsterAttack.UseMove(playerMonsterData.moves[0], 0); }
        else if (playerInput.actions["Move2"].WasPressedThisFrame() && playerMonsterAttack.moveCharges[1] >= 1 && canUseMove) { playerMonsterAttack.UseMove(playerMonsterData.moves[1], 1); }
        else if (playerInput.actions["Move3"].WasPressedThisFrame() && playerMonsterAttack.moveCharges[2] >= 1 && canUseMove) { playerMonsterAttack.UseMove(playerMonsterData.moves[2], 2); }
    }
}