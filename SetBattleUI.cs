using UnityEngine;
using UnityEngine.UI;

public class SetBattleUI : MonoBehaviour
{
    //Gem currency variables
    [SerializeField] private Transform gemText;

    //Monster element variables
    [SerializeField] private Transform playerElements;
    [SerializeField] private Transform enemyElements;

    //Health bar variables
    private float healthBarWidthMax;
    [SerializeField] private GameObject playerHealthMain;
    [SerializeField] private GameObject enemyHealthMain;

    //Monster stun variables
    [SerializeField] private GameObject playerStunMain;
    [SerializeField] private GameObject enemyStunMain;
    private GameObject enemyStunParent;

    //Monster status variables
    [SerializeField] private Transform playerStatus;
    [SerializeField] private Transform enemyStatus;

    [SerializeField] private Transform selectableMoves;

    public void SetGemCurrencyUI(int partialGemIncrease = 0)
    {
        if (partialGemIncrease == 0) { gemText.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.gems.ToString(); }
        else
        {
            int currentGemsDisplayed = int.Parse(gemText.GetComponent<TMPro.TextMeshProUGUI>().text);
            gemText.GetComponent<TMPro.TextMeshProUGUI>().text = (currentGemsDisplayed + partialGemIncrease).ToString();
        }
    }

    public void SetMonsterElementUI()
    {
        //Set player elements
        MonsterData playerMonsterData = GetComponent<CombatManager>().playerMonster.GetComponent<MonsterCombat>().MonsterData;
        int elementsSetPlayer = 0;
        int elementsRemovedPlayer = 0;
        for (int i = 0; i < playerMonsterData.elementalAffinities.Length; i++)
        {
            if (playerMonsterData.elementalAffinities[i].affinityStatus == AffinityStatus.Positive)
            {
                playerElements.GetChild(elementsSetPlayer).gameObject.SetActive(true);
                playerElements.GetChild(elementsSetPlayer).GetComponent<Image>().sprite = playerMonsterData.elementalAffinities[i].element.image;
                elementsSetPlayer += 1;
            }
            else
            {
                playerElements.GetChild(playerElements.childCount - 1 - elementsRemovedPlayer).gameObject.SetActive(false);
                elementsRemovedPlayer += 1;
            }
        }

        //Set enemy elements
        MonsterData enemyMonsterData = GameObject.Find("MonsterObject(Clone)").GetComponent<MonsterCombat>().MonsterData;
        int elementsSetEnemy = 0;
        int elementsRemovedEnemy = 0;
        for (int i = 0; i < enemyMonsterData.elementalAffinities.Length; i++)
        {
            if (enemyMonsterData.elementalAffinities[i].affinityStatus == AffinityStatus.Positive)
            {
                enemyElements.GetChild(elementsSetEnemy).gameObject.SetActive(true);
                enemyElements.GetChild(elementsSetEnemy).GetComponent<Image>().sprite = enemyMonsterData.elementalAffinities[i].element.image;
                elementsSetEnemy += 1;
            }
            else
            {
                enemyElements.GetChild(enemyElements.childCount - 1 - elementsRemovedEnemy).gameObject.SetActive(false);
                elementsRemovedEnemy += 1;
            }
        }
    }

    public void SetHealthBarUI(bool playerHealth, int healthCurrent, int healthMax)
    {
        //Get health bar to set
        GameObject healthBar;
        if (playerHealth) { healthBar = playerHealthMain; }
        else { healthBar = enemyHealthMain; }

        //Set health bar
        float healthBarRatio = (float)healthCurrent / (float)healthMax;
        healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWidthMax * healthBarRatio);
    }

    public void SetMonsterStunUI(bool playerStunGauge, float valueToSet)
    {
        if (playerStunGauge) { playerStunMain.GetComponent<Image>().fillAmount = valueToSet; }
        else
        {
            enemyStunMain.GetComponent<Image>().fillAmount = valueToSet;
            bool showStunGauge = valueToSet > 0;
            if (enemyStunParent.activeSelf != showStunGauge) { enemyStunParent.SetActive(showStunGauge); }
        }
    }

    public void SetPlayerStatusUI()
    {
        MonsterStatus playerMonsterStatus = GetComponent<CombatManager>().playerMonster.GetComponent<MonsterStatus>();

        for (int i = 0; i < enemyStatus.childCount; i++)
        {
            if (playerMonsterStatus.appliedStatusEffects[i] != null)
            {
                if (!playerStatus.GetChild(i).gameObject.activeSelf) { playerStatus.GetChild(i).gameObject.SetActive(true); }
                playerStatus.GetChild(i).GetChild(2).GetComponent<Image>().sprite = playerMonsterStatus.appliedStatusEffects[i].statusEffect.image;
                playerStatus.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = playerMonsterStatus.appliedStatusEffects[i].timeRemaining / playerMonsterStatus.appliedStatusEffects[i].maxTime;
            }
            else
            {
                if (playerStatus.GetChild(i).gameObject.activeSelf) { playerStatus.GetChild(i).gameObject.SetActive(false); }
            }
        }
    }

    public void SetEnemyStatusUI()
    {
        MonsterStatus enemyMonsterStatus = GetComponent<CombatManager>().enemyMonster.GetComponent<MonsterStatus>();

        for (int i = 0; i < enemyStatus.childCount; i++)
        {
            if (enemyMonsterStatus.appliedStatusEffects[i] != null)
            {
                if (!enemyStatus.GetChild(i).gameObject.activeSelf) { enemyStatus.GetChild(i).gameObject.SetActive(true); }
                enemyStatus.GetChild(i).GetChild(2).GetComponent<Image>().sprite = enemyMonsterStatus.appliedStatusEffects[i].statusEffect.image;
                enemyStatus.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = enemyMonsterStatus.appliedStatusEffects[i].timeRemaining / enemyMonsterStatus.appliedStatusEffects[i].maxTime;
            }
            else
            {
                if (enemyStatus.GetChild(i).gameObject.activeSelf) { enemyStatus.GetChild(i).gameObject.SetActive(false); }
            }
        }
    }

    private void SetMovesUI()
    {
        MoveData[] playerMoves = GetComponent<CombatManager>().playerMonster.GetComponent<MonsterCombat>().MonsterData.moves;

        for (int i = 0; i < selectableMoves.childCount; i++)
        {
            //Set move images
            selectableMoves.GetChild(i).GetChild(1).GetComponent<Image>().sprite = playerMoves[i].element.image;

            //Set move names
            selectableMoves.GetChild(i).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = playerMoves[i].name;
        }
    }

    public void SetMoveChargeUI(float[] moveCharges)
    {
        for (int i = 0; i < selectableMoves.childCount; i++)
        {
            selectableMoves.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = moveCharges[i];
        }
    }

    private void Awake()
    {
        //Set variables
        healthBarWidthMax = playerHealthMain.GetComponent<RectTransform>().rect.width;
        enemyStunParent = enemyStunMain.transform.parent.gameObject;

        //Functions and coroutines
        SetGemCurrencyUI();
        SetMovesUI();
    }
}