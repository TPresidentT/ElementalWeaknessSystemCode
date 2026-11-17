using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [Header("Monster Data")]
    [SerializeField] private MonsterData monsterData;

    [Header("Scene References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform userInterface;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject targetMonster;

    [Header("Elements")]
    [SerializeField] private ElementData[] allElements;
    [SerializeField] private ElementData divineElement;

    private SetBattleUI setBattleUI;


    public MonsterData MonsterData => monsterData;
    public Camera MainCamera => mainCamera;
    public Transform UserInterface => userInterface;
    public GameObject GameManager => gameManager;
    public GameObject TargetMonster => targetMonster;
    public ElementData[] AllElements => allElements;
    public ElementData DivineElement => divineElement;
    public SetBattleUI SetBattleUI => setBattleUI;

    public void MonsterInitialise(MonsterData dataToSet, Camera cameraToSet, Transform uiToSet, GameObject managerToSet, GameObject targetToSet)
    {
        monsterData = dataToSet;
        mainCamera = cameraToSet;
        userInterface = uiToSet;
        gameManager = managerToSet;
        targetMonster = targetToSet;
        GetComponent<SpriteRenderer>().sprite = monsterData.image;
    }

    public void SetNewTarget(GameObject targetToSet)
    {
        targetMonster = targetToSet;
    }

    private void Start()
    {
        //Set variables
        setBattleUI = gameManager.GetComponent<SetBattleUI>();

        //Functions and coroutines
        setBattleUI.SetMonsterElementUI();
    }
}