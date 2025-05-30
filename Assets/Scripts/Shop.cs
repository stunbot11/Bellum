using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{ 
    private GameManager gameManager;

    public TextMeshProUGUI lifeTXT;
    public TextMeshProUGUI moneyTXT;
    public int money;

    [Header("UpgradePoint stuff")]
    public int upgradePoints;
    public int upgradePointLevel;
    public int upgradePointsBaseCost;
    public TextMeshProUGUI upgradePointsBaseCostTXT;

    [Header("Regain health stuff")]
    public int health;
    public int healthRegenLevel;
    public int healthBaseCost;
    public TextMeshProUGUI healthBaseCostTXT;

    [Header("Max health stuff")]
    public int maxHealth;
    public int maxHealthLevel;
    public int maxHealthBaseCost;
    public TextMeshProUGUI maxHealthBaseCostTXT;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        money = gameManager.money;
        health = gameManager.health;
        maxHealth = gameManager.maxHealth;
        moneyTXT.text = money.ToString();
        lifeTXT.text = health + "/" + maxHealth;
        upgradePointsBaseCostTXT.text = Mathf.RoundToInt(upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel * 2)).ToString();
        healthBaseCostTXT.text = Mathf.RoundToInt(healthBaseCost * Mathf.Pow(1.25f, healthRegenLevel * 1)).ToString();
        maxHealthBaseCostTXT.text = Mathf.RoundToInt(maxHealthBaseCost * Mathf.Pow(1.25f, maxHealthLevel * 1.5f)).ToString();
    }

    public void buyUpgradePoint()
    {
        int cost = Mathf.RoundToInt(upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel * 2));
        if (upgradePoints < 10 && cost <= money) //amount of upgrades + 1
        {
            money -= cost;
            upgradePointLevel++;          
            upgradePointsBaseCostTXT.text = Mathf.RoundToInt(upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel * 2)).ToString();
            moneyTXT.text = money.ToString();
            if (upgradePointLevel > 9 || money < upgradePointsBaseCost * Mathf.Pow(1.25f, upgradePointLevel * 2)) //checks to see if you can buy again
                upgradePointsBaseCostTXT.GetComponentInParent<Button>().interactable = false;
        }
    }

    public void buyHealthRegen()
    {
        int cost = Mathf.RoundToInt(Mathf.Pow(1.25f, healthRegenLevel));
        if (healthBaseCost * cost <= money)
        {
            money -= cost;
            healthRegenLevel++;
            healthBaseCostTXT.text = Mathf.RoundToInt(healthBaseCost * Mathf.Pow(1.25f, healthRegenLevel * 1)).ToString();
            moneyTXT.text = money.ToString();
            health = (maxHealth / 4) + health >= maxHealth ? maxHealth : health + (maxHealth / 4);
            lifeTXT.text = health + "/" + maxHealth;
            if (money < healthBaseCost * Mathf.Pow(1.25f, healthRegenLevel * 1))
                healthBaseCostTXT.GetComponentInParent<Button>().interactable = false;
        }
    }

    public void buyMaxHealth()
    {
        int cost = Mathf.RoundToInt(Mathf.Pow(1.25f, maxHealthLevel * 1.5f));
        if (maxHealthBaseCost * cost <= money)
        {
            money -= cost;
            maxHealthLevel++;
            maxHealthBaseCostTXT.text = Mathf.RoundToInt(maxHealthBaseCost * Mathf.Pow(1.25f, maxHealthLevel * 1.5f)).ToString();
            moneyTXT.text = money.ToString();
            maxHealth += 20;
            health += 20;
            lifeTXT.text = health + "/" + maxHealth;
            if (money < maxHealthBaseCost * Mathf.Pow(1.25f, maxHealthLevel * 1.5f))
                maxHealthBaseCostTXT.GetComponentInParent<Button>().interactable = false;
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
