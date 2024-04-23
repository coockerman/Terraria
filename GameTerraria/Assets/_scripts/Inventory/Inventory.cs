using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public ToolClass start_Axe;
    public ToolClass start_Hammer;
    public ToolClass start_Pickage;

    public GameObject inventoryUI;
    public GameObject hotbarUI;

    public GameObject inventorySlotPrefab;

    public int inventoryWidth;
    public int inventoryHeight;
    public InventorySlot[,] inventorySlots;
    public InventorySlot[] hotbarSlot;

    public GameObject[,] UISlots;
    public GameObject[] hotbarUISlot;

    private void Start()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        hotbarSlot = new InventorySlot[inventoryWidth];

        UISlots = new GameObject[inventoryWidth, inventoryHeight];
        hotbarUISlot = new GameObject[inventoryWidth];

        SetupUI();
        UpdateInventoryUI();
        AddItem(new ItemClass(start_Axe));
        AddItem(new ItemClass(start_Hammer));
        AddItem(new ItemClass(start_Pickage));
    }
    void SetupUI()
    {
        //setup inventory
        for (int j = 0; j < inventoryHeight; j++)
        {
            for (int i = 0; i < inventoryWidth; i++)
            {
                GameObject objSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, inventoryUI.transform.GetChild(0).GetChild(0).transform);
                UISlots[i, j] = objSlot;
                inventorySlots[i, j] = null;
            }
        }

        //setup hotbar
        for (int i = 0; i < inventoryWidth; i++)
        {
            GameObject objHotBarSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, hotbarUI.transform.GetChild(0).GetChild(0).transform);
            hotbarUISlot[i] = objHotBarSlot;
            hotbarSlot[i] = null;
        }
    }
    void UpdateInventoryUI()
    {
        //update inventory
        for (int j = 0; j < inventoryHeight; j++)
        {
            for (int i = 0; i < inventoryWidth; i++)
            {
                if (inventorySlots[i, j] == null)
                {
                    UISlots[i, j].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    UISlots[i, j].transform.GetChild(0).GetComponent<Image>().enabled = false;

                    UISlots[i, j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                    UISlots[i, j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }
                else
                {
                    UISlots[i, j].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    UISlots[i, j].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[i, j].item.sprite;

                    UISlots[i, j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[i, j].quantity.ToString();
                    UISlots[i, j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }
        //update hotbar
        for (int i = 0; i < inventoryWidth; i++)
        {
            if (inventorySlots[i, 0] == null)
            {
                hotbarUISlot[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarUISlot[i].transform.GetChild(0).GetComponent<Image>().enabled = false;

                hotbarUISlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                hotbarUISlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                hotbarUISlot[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarUISlot[i].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[i, 0].item.sprite;

                hotbarUISlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[i, 0].quantity.ToString();
                hotbarUISlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
            }
        }

    }
    public bool AddItem(ItemClass item)
    {
        Vector2Int itemPos = Contains(item);
        if (itemPos != Vector2Int.one * -1)
        {
            InventorySlot slot = inventorySlots[itemPos.x, itemPos.y];
            if (slot.quantity < slot.stackLimit)
            {
                inventorySlots[itemPos.x, itemPos.y].quantity += 1;
                UpdateInventoryUI();
                return true;
            }
        }

        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] == null)
                {
                    inventorySlots[x, y] = new InventorySlot { item = item, position = new Vector2Int(x, y), quantity = 1 };
                    UpdateInventoryUI();
                    return true;
                }
            }
        }
        return false;
    }
    public Vector2Int Contains(ItemClass item)
    {
        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null)
                {
                    if (inventorySlots[x, y].item.nameTool == item.nameTool)
                    {
                        if (item.isStackable && inventorySlots[x, y].quantity < inventorySlots[x, y].stackLimit)
                            return inventorySlots[x, y].position;
                    }
                }
            }
        }
        return Vector2Int.one * -1;
    }
    
    public bool RemoveItem(ItemClass item)
    {
        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if(inventorySlots[x, y]!=null)
                {
                    if (inventorySlots[x, y].item.nameTool == item.nameTool)
                    {
                        inventorySlots[x, y].quantity -= 1;

                        if (inventorySlots[x, y].quantity == 0)
                        {
                            inventorySlots[x, y] = null;
                        }

                        UpdateInventoryUI();
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
