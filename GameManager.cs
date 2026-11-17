using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int gems;

    //Gem currency variables
    [SerializeField] private GameObject gemCurrency;
    private int currencyPerGemMin = 1;
    private int currencyPerGemMax = 3;

    //Battle background variables
    [SerializeField] private GameObject battleBackground;
    [SerializeField] private Sprite[] allBattleBackgrounds;
    [SerializeField] private GameObject currentBattleBackground;

    public void IncreaseGems(int amountToIncrease)
    {
        gems += amountToIncrease;
        //GetComponent<SetBattleUI>().SetGemCurrencyUI(gems);
    }

    public void CreateGemCurrency(int gemsToCreate)
    {
        int totalGemsToGive = 0;

        for (int i = 0; i < gemsToCreate; i++)
        {
            Vector3 offsetDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Vector3 positionToCreate = GetComponent<CombatManager>().enemyMonster.transform.position + (offsetDirection * 0.5f);
            int currencyThisGem = Random.Range(currencyPerGemMin, currencyPerGemMax + 1);

            var newGemCurrency = Instantiate(gemCurrency, positionToCreate, transform.rotation);
            newGemCurrency.GetComponent<GemCurrency>().gemsToGive = currencyThisGem;
            newGemCurrency.GetComponent<GemCurrency>().burstDirection = offsetDirection;

            totalGemsToGive += currencyThisGem;
        }
        IncreaseGems(totalGemsToGive);
    }

    public IEnumerator ChangeBattleBackground()
    {
        //Get background to set
        Sprite battleBackgroundToSet = allBattleBackgrounds[Random.Range(0, allBattleBackgrounds.Length)];
        while (battleBackgroundToSet == currentBattleBackground.GetComponent<SpriteRenderer>().sprite) { battleBackgroundToSet = allBattleBackgrounds[Random.Range(0, allBattleBackgrounds.Length)]; }

        //Set background
        GameObject newBattleBackground = Instantiate(battleBackground);
        newBattleBackground.GetComponent<SpriteRenderer>().sprite = battleBackgroundToSet;
        newBattleBackground.GetComponent<Animator>().SetTrigger("battleBackgroundFade");
        currentBattleBackground.GetComponent<SpriteRenderer>().sortingOrder -= 1;

        yield return new WaitForSeconds(0.5f);

        Destroy(currentBattleBackground);
        currentBattleBackground = newBattleBackground;

    }
}