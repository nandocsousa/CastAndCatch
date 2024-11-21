using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public GameObject pauseButton;

    public GameObject pauseMenu;
    public GameObject volumeMenu;
    public GameObject inventoryMenu;
    public GameObject shopMenu;
    public GameObject buyMenu;
    public GameObject sellMenu;

    private GameObject currentMenu;

    private Dictionary<string, GameObject> menus;

    private void Start()
    {
        menus = new Dictionary<string, GameObject>
        {
            { "PauseMenu", pauseMenu },
            { "VolumeMenu", volumeMenu },
            { "InventoryMenu",  inventoryMenu },
            { "ShopMenu", shopMenu },
            { "BuyMenu", buyMenu },
            { "SellMenu", sellMenu }
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
        if (currentMenu == buyMenu || currentMenu == sellMenu)
        {
            OpenMenu("ShopMenu");
        }
        else OpenMenu("PauseMenu");
    }
}