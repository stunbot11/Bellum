using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    private PlayerController playerController;
    public TextMeshProUGUI primaryName;
    public TextMeshProUGUI secondaryName;
    public GameObject[] path1 = new GameObject[3];
    public GameObject[] path2 = new GameObject[3];
    public GameObject[] path3 = new GameObject[3];

    public bool[] bool1;
    public bool[] bool2;
    public bool[] bool3;

    public int points;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        bool1 = new bool[path1.Length + 1];
        bool2 = new bool[path2.Length + 1];
        bool3 = new bool[path3.Length + 1];

        if (playerController.gameManager.arcadeMode)
            points = playerController.gameManager.upgradePoints;

        switch (playerController.gameManager.classType)
        {
            case 1:
                primaryName.text = "Sword";
                secondaryName.text = "Shield";
                path1[0].GetComponentInChildren<TextMeshProUGUI>().text = "Sword poisons enemies";
                path1[1].GetComponentInChildren<TextMeshProUGUI>().text = "Faster attack speed";
                path1[2].GetComponentInChildren<TextMeshProUGUI>().text = "Enmies that are poisoned take more damage";
                path2[0].GetComponentInChildren<TextMeshProUGUI>().text = "Blocking blocks more damage";
                path2[1].GetComponentInChildren<TextMeshProUGUI>().text = "Perfect block is easier";
                path2[2].GetComponentInChildren<TextMeshProUGUI>().text = "Deal more damage after perfect block";
                break;

            case 2:
                primaryName.text = "Bow";
                secondaryName.text = "Dodge";
                path1[0].GetComponentInChildren<TextMeshProUGUI>().text = "Charge your arrows faster";
                path1[1].GetComponentInChildren<TextMeshProUGUI>().text = "Arrows ignite enemies";
                path1[2].GetComponentInChildren<TextMeshProUGUI>().text = "Shoot more arrows";
                path2[0].GetComponentInChildren<TextMeshProUGUI>().text = "Dodge cooldown is shorter";
                path2[1].GetComponentInChildren<TextMeshProUGUI>().text = "Gain another dodge charge";
                path2[2].GetComponentInChildren<TextMeshProUGUI>().text = "Dodging through enemies deals damage";
                break;

            case 3:
                primaryName.text = "Trident";
                secondaryName.text = "Net";
                path1[0].GetComponentInChildren<TextMeshProUGUI>().text = "Longer attack range";
                path1[1].GetComponentInChildren<TextMeshProUGUI>().text = "Wider attack range";
                path1[2].GetComponentInChildren<TextMeshProUGUI>().text = "More attack damage";
                path2[0].GetComponentInChildren<TextMeshProUGUI>().text = "Enemies are immoblized for longer";
                path2[1].GetComponentInChildren<TextMeshProUGUI>().text = "Faster cooldown for net";
                path2[2].GetComponentInChildren<TextMeshProUGUI>().text = "Deal more damage to netted enemies";
                break;
        }
    }
    public void upgrade1(int num)
    {
        if (points > 0 && !bool1[num])
        {
            points--;
            bool1[num] = true;
            path1[num].GetComponent<Image>().color = Color.green;
            if (num != path1.Length - 1)
                path1[num + 1].SetActive(true);
        }
        else if (bool1[num] && !bool1[num + 1])
        {
            points++;
            bool1[num] = false;
            path1[num].GetComponent<Image>().color = Color.red;
            if (num != path1.Length - 1)
                path1[num + 1].SetActive(false);
        }
    }

    public void upgrade2(int num)
    {
        if (points > 0 && !bool2[num])
        {
            points--;
            bool2[num] = true;
            path2[num].GetComponent<Image>().color = Color.green;
            if (num != path2.Length - 1)
                path2[num + 1].SetActive(true);
        }
        else if (bool2[num] && !bool2[num + 1])
        {
            points++;
            bool2[num] = false;
            path2[num].GetComponent<Image>().color = Color.red;
            if (num != path2.Length - 1)
                path2[num + 1].SetActive(false);
        }
    }

    public void upgrade3(int num)
    {
        if (points > 0 && !bool3[num])
        {
            points--;
            bool3[num] = true;
            path3[num].GetComponent<Image>().color = Color.green;
            if (num != path3.Length - 1)
                path3[num + 1].SetActive(true);
        }
        else if (bool3[num] && !bool3[num + 1])
        {
            points++;
            bool3[num] = false;
            path3[num].GetComponent<Image>().color = Color.red;
            if (num != path3.Length - 1)
                path3[num + 1].SetActive(false);
        }
    }

    public void confirm()
    {
        for (int i = 0; i < bool1.Length; i++)
        {
            if (bool1[i])
                playerController.upgrades[0]++;
            else
                break;
        }

        for (int i = 0; i < bool2.Length; i++)
        {
            if (bool2[i])
                playerController.upgrades[1]++;
            else
                break;
        }

        for (int i = 0; i < bool3.Length; i++)
        {
            if (bool3[i])
                playerController.upgrades[2]++;
            else
                break;
        }

        this.gameObject.SetActive(false);
        playerController.fakeStart();
    }
}
