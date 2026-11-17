using UnityEngine;
using UnityEngine.UI;

public class MonsterHealth : MonoBehaviour
{
    [HideInInspector] public int healthCurrent;
    private float damageNumberRange = 0.75f;
    private bool isPlayerMonster;
    private bool monsterKilled;
    private Vector3 startPosition;

    public GameObject damageNumber;
    public GameObject positiveAffinityMarker;
    public GameObject weakText;
    private Transform userInterface;

    private MonsterCombat monsterCombat;
    private MonsterStun monsterStun;
    private MonsterAttack monsterAttack;
    private Camera mainCamera;

    private GameObject gameManager;
    private CombatManager combatManager;
    private SetBattleUI setBattleUI;

    public void DamageMonster(int calculatedDamage, Color textColor = default, bool userPositiveAffinity = false, ElementData moveElement = null, bool targetNegativeAffinity = false)
    {
        if (monsterKilled) { return; }

        healthCurrent = Mathf.Clamp(healthCurrent - calculatedDamage, 0, monsterCombat.MonsterData.healthMax);

        //Set health bar ui
        setBattleUI.SetHealthBarUI(isPlayerMonster, healthCurrent, monsterCombat.MonsterData.healthMax);

        CreateFloatingNumber(calculatedDamage, textColor, moveElement, userPositiveAffinity, targetNegativeAffinity);

        //Apply stun effect
        if (moveElement != null && monsterStun.isStunned) { monsterStun.ApplyStunEffect(moveElement); }

        //Stun monster
        if (targetNegativeAffinity) { monsterStun.StunMonster(); }

        //Kill monster
        if (healthCurrent <= 0) { KillMonster(); }
    }

    public void HealMonster(int healthToHeal, Color textColor = default)
    {
        healthCurrent = Mathf.Clamp(healthCurrent + healthToHeal, 0, monsterCombat.MonsterData.healthMax);

        //Set health bar ui
        setBattleUI.SetHealthBarUI(isPlayerMonster, healthCurrent, monsterCombat.MonsterData.healthMax);

        CreateFloatingNumber(healthToHeal, textColor);
    }

    private void KillMonster()
    {
        if (monsterKilled) { return; }

        monsterKilled = true;

        MonsterAttack targetMonsterAttack = monsterCombat.TargetMonster.GetComponent<MonsterAttack>();

        monsterAttack.StopAutoAttackCoroutine();
        targetMonsterAttack.StopAutoAttackCoroutine();

        bool playerWin = !isPlayerMonster;
        combatManager.EndBattle(playerWin);

        Destroy(gameObject);
    }

    private void CreateFloatingNumber(int valueToSet, Color textColor, ElementData moveElement = null, bool userPositiveAffinity = false, bool targetNegativeAffinity = false)
    {
        //Create number
        Vector3 floatingNumberOffset = new Vector3(Random.Range(-damageNumberRange, damageNumberRange), 4.5f, 0);
        Vector3 floatingNumberPosition = mainCamera.WorldToScreenPoint(startPosition + floatingNumberOffset);
        var newFloatingNumber = Instantiate(damageNumber, floatingNumberPosition, transform.rotation, userInterface);

        //Set value
        var newFloatingNumberText = newFloatingNumber.GetComponent<TMPro.TextMeshProUGUI>();
        newFloatingNumberText.text = valueToSet.ToString();
        if (textColor != default) { newFloatingNumberText.color = textColor; }
        else { newFloatingNumberText.color = Color.white; }

        //Create positive affinity marker
        if (userPositiveAffinity) { CreatePositiveAffinityMarker(newFloatingNumber, moveElement); }

        //Create weak text
        if (targetNegativeAffinity) { CreateWeakText(newFloatingNumber); }
    }

    private void CreatePositiveAffinityMarker(GameObject newFloatingNumber, ElementData moveElement)
    {
        Vector3 positiveAffinityMarkerOffset = new Vector3(1.5f, 0f, 0f);
        Vector3 positiveAffinityMarkerPosition = mainCamera.WorldToScreenPoint(mainCamera.ScreenToWorldPoint(newFloatingNumber.transform.position) + positiveAffinityMarkerOffset);
        var newPositiveAffinityMarker = Instantiate(positiveAffinityMarker, positiveAffinityMarkerPosition, transform.rotation, newFloatingNumber.transform);
        newPositiveAffinityMarker.GetComponent<Image>().sprite = moveElement.image;
    }

    private void CreateWeakText(GameObject newFloatingNumber)
    {
        //Create weak text
        Vector3 weakTextOffset = new Vector3(1.5f, 5.5f, 0);
        Vector3 weakTextPosition = mainCamera.WorldToScreenPoint(startPosition + weakTextOffset);
        var newWeakText = Instantiate(weakText, weakTextPosition, transform.rotation, newFloatingNumber.transform);
    }

    private void Awake()
    {
        //Set variables
        isPlayerMonster = CompareTag("PlayerMonster");
        startPosition = transform.position;
    }

    private void Start()
    {
        //Set variables
        monsterCombat = GetComponent<MonsterCombat>();
        monsterStun = GetComponent<MonsterStun>();
        monsterAttack = GetComponent<MonsterAttack>();
        healthCurrent = monsterCombat.MonsterData.healthMax;
        userInterface = monsterCombat.UserInterface;
        mainCamera = monsterCombat.MainCamera;
        gameManager = monsterCombat.GameManager;
        combatManager = gameManager.GetComponent<CombatManager>();
        setBattleUI = monsterCombat.SetBattleUI;

        setBattleUI.SetHealthBarUI(isPlayerMonster, healthCurrent, monsterCombat.MonsterData.healthMax);
    }
}