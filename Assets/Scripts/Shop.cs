using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{ 
    private GameManager gameManager;

    public TextMeshProUGUI moneyTXT;
    public int money;

    [Header("UpgradePoint stuff")]
    public int upgradePoints;
    public int upgradePointLevel;
    public int upgradePointsBaseCost;

    [Header("Regain health stuff")]
    public int health;
    public int healthRegenLevel;
    public int healthBaseCost;

    [Header("Max health stuff")]
    public int maxHealth;
    public int maxHealthLevel;
    public int maxHealthBaseCost;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        money = gameManager.money;
        moneyTXT.text = money.ToString();
    }

    public void buyUpgradePoint()
    {
        if (upgradePoints < 10 && upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel * 2) <= money) //amount of upgrades + 1
        {
            
        }
    }

    public void buyHealthRegen()
    {
        if (upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel) <= money)
        {

        }
    }

    public void buyMaxHealth()
    {
        if (upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel * 1.5f) <= money)
        {

        }
    }

    public void confirm()
    {
        gameManager.money = money;
        gameManager.upgradePoints = upgradePoints;
        gameManager.health = health;
        gameManager.maxHealth = maxHealth;
        StartCoroutine(gameManager.start());
    }
}
