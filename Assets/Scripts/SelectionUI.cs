using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    public EventSystem eventSystem;
    [HideInInspector] public int selectionPart; //0 = title, 1 = class, 2 = boss, 3 = challenges/start
    private bool flickerOn = true;
    public GameObject startTxt;
    public GameObject[] parts;
    public Image[] playerClass;
    public Image[] bossType;

    public bool scrollActive;
    GameObject activeScroll;

    public InputActionReference backButton;

    public GameObject titelScreen;
    public GameObject selectionScreen;

    private void Start()
    {
        backButton.action.started += back;
        StartCoroutine(buttonFlicker());
    }


    public void back(InputAction.CallbackContext phase)
    {
        if (scrollActive)
        {
            scrollActive = false;
            activeScroll.GetComponent<Animator>().SetTrigger("Close");
            activeScroll.transform.GetChild(0).gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(parts[selectionPart]);
        }
        else if (selectionPart == 0)
            Application.Quit();
        else if (selectionPart == 1)
        {
            selectionPart = 0;
            titelScreen.SetActive(true);
            selectionScreen.SetActive(false);
            eventSystem.SetSelectedGameObject(parts[selectionPart]);
        }
        else if (selectionPart > 0)
        {
            selectionPart--;
            eventSystem.SetSelectedGameObject(parts[selectionPart]);
            if (selectionPart == 1)
            {
                for (int i = 0; i < playerClass.Length; i++)
                    playerClass[i].color = Color.red;
            }
            else if (selectionPart == 2)
            {
                for (int i = 0; i < bossType.Length; i++)
                    bossType[i].color = Color.red;
            }
        }
    }

    public void confirmPart()
    {
        selectionPart++;
        eventSystem.SetSelectedGameObject(parts[selectionPart]);
        scrollActive = false;
    }

    public void changeActiveScroll(GameObject scroll)
    {
        activeScroll = scroll;
    }

    public void changeActive(bool active)
    {
        scrollActive = active;
    }

    IEnumerator buttonFlicker()
    {
        yield return new WaitForSeconds(.75f);
        flickerOn = !flickerOn;
        startTxt.SetActive(flickerOn);
        StartCoroutine(buttonFlicker());
    }

    public void discard()
    {
        backButton.action.started -= back;
    }
}
