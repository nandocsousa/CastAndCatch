using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public GameObject pauseButton;

    public GameObject pauseMenu;
    public GameObject volumeMenu;
    public GameObject inventoryMenu;

    private GameObject currentMenu;

    private Dictionary<string, GameObject> menus;

    private void Start()
    {
        menus = new Dictionary<string, GameObject>
        {
            { "PauseMenu", pauseMenu },
            { "VolumeMenu", volumeMenu },
            { "InventoryMenu",  inventoryMenu }
        };

        CloseAllMenus();
    }

    public void OpenMenu(string menuName)
    {
        CloseAllMenus();

        if (menus.ContainsKey(menuName))
        {
            if (currentMenu != null)
            {
                currentMenu.SetActive(false);
            }

            currentMenu = menus[menuName];
            currentMenu.SetActive(true);

            Time.timeScale = 0;
        }

        pauseButton.SetActive(false);
    }

    public void CloseAllMenus()
    {
        foreach (var menu in menus.Values)
        {
            menu.SetActive(false);
        }

        if (currentMenu == pauseMenu)
        {
            Time.timeScale = 1;
        }

        currentMenu = null;

        pauseButton.SetActive(true);
    }

    public void ReturnToPreviousMenu()
    {
        OpenMenu("PauseMenu");
    }
}