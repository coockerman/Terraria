using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public ToolClass[] start_Tool;
    public TileClass[] start_Tile;
    public WeaponClass[] start_Weapon;

    public GameObject inventoryUI;
    public GameObject hotbarUI;

    public GameObject inventorySlotPrefab;

    public int inventoryWidth;
    public int inventoryHeight;
    public InventorySlot[,] inventorySlots;
    public InventorySlot[] hotbarSlot;
    public InventorySlot pickSlot;

    public GameObject[,] UISlots;
    public GameObject[] hotbarUISlot;
    public GameObject pickUISlot;

    public WeaponClass[] ListAllWeapon;
    InventorySlot[] BanCheTaoSlot;
    InventorySlot KetQuaBanCheTao;

    GameObject[] UiBanCheTaoSlots;
    GameObject UiKetQuaBanCheTao;

    private void Start()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        hotbarSlot = new InventorySlot[inventoryWidth];
        pickSlot = new InventorySlot();
        BanCheTaoSlot = new InventorySlot[4];
        KetQuaBanCheTao = new InventorySlot();

        UISlots = new GameObject[inventoryWidth, inventoryHeight];
        hotbarUISlot = new GameObject[inventoryWidth];
        pickUISlot = new GameObject();
        UiBanCheTaoSlots = new GameObject[4];
        UiKetQuaBanCheTao = new GameObject();

        SetupUI();
        UpdateInventoryUI();
        StartAddItem();
    }
    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        pickUISlot.transform.position = new Vector3(mouse.x - 1, mouse.y - 1, 0);
    }
    void StartAddItem()
    {
        foreach (ToolClass tool in start_Tool)
        {
            AddItem(new ItemClass(tool));
        }
        foreach (TileClass tile in start_Tile)
        {
            AddItem(new ItemClass(tile));
        }
        foreach (WeaponClass weapon in start_Weapon)
        {
            AddItem(new ItemClass(weapon));
        }
    }
    void SetupUI()
    {
        //setup inventory
        for (int j = 0; j < inventoryHeight; j++)
        {
            for (int i = 0; i < inventoryWidth; i++)
            {
                int x = i;
                int y = j;
                GameObject objSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, inventoryUI.transform.GetChild(0).GetChild(0).transform);
                objSlot.AddComponent<Button>();
                objSlot.GetComponent<Button>().onClick.AddListener(() => { SelectItem(x, y); });
                UISlots[x, y] = objSlot;
                inventorySlots[x, y] = null;
            }
        }

        //setup banchetao
        for (int i = 0; i < 4; i++)
        {
            int x = i;
            GameObject objSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, inventoryUI.transform.GetChild(0).GetChild(1).transform);
            objSlot.AddComponent<Button>();
            objSlot.GetComponent<Button>().onClick.AddListener(() => { SelectItemBanCheTao(x); });
            UiBanCheTaoSlots[x] = objSlot;
            BanCheTaoSlot[x] = null;
        }

        //setup ketqua banchetao
        GameObject objKetQuaSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, inventoryUI.transform.GetChild(0).GetChild(2).transform);
        objKetQuaSlot.AddComponent<Button>();
        UiKetQuaBanCheTao = objKetQuaSlot;
        KetQuaBanCheTao = null;

        //setup hotbar
        for (int i = 0; i < inventoryWidth; i++)
        {
            GameObject objHotBarSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, hotbarUI.transform.GetChild(0).GetChild(0).transform);
            hotbarUISlot[i] = objHotBarSlot;
            hotbarSlot[i] = null;
        }

        //setup pick
        GameObject objPickSlot = Instantiate(inventorySlotPrefab, Vector2.zero, Quaternion.identity, inventoryUI.transform.GetChild(0).transform);
        objPickSlot.SetActive(false);
        objPickSlot.GetComponent<RectTransform>().pivot = Vector2.one;
        objPickSlot.GetComponent<Image>().color = Color.clear;
        pickUISlot = objPickSlot;
        pickSlot = null;
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

        //update pickSlot
        if (pickSlot == null)
        {
            pickUISlot.transform.GetChild(0).GetComponent<Image>().sprite = null;
            pickUISlot.transform.GetChild(0).GetComponent<Image>().enabled = false;

            pickUISlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
            pickUISlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
            pickUISlot.gameObject.SetActive(false);

        }
        else
        {
            pickUISlot.transform.GetChild(0).GetComponent<Image>().enabled = true;
            pickUISlot.transform.GetChild(0).GetComponent<Image>().sprite = pickSlot.item.sprite;

            pickUISlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = pickSlot.quantity.ToString();
            pickUISlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
            pickUISlot.gameObject.SetActive(true);
        }
        //update banchetao
        for (int i = 0; i < 4; i++)
        {
            if (BanCheTaoSlot[i] == null)
            {
                UiBanCheTaoSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                UiBanCheTaoSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;

                UiBanCheTaoSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                UiBanCheTaoSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                UiBanCheTaoSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                UiBanCheTaoSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = BanCheTaoSlot[i].item.sprite;

                UiBanCheTaoSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[i, 0].quantity.ToString();
                UiBanCheTaoSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
            }
        }

        //update ketqua banchetao
        if (KetQuaBanCheTao == null)
        {
            UiKetQuaBanCheTao.transform.GetChild(0).GetComponent<Image>().sprite = null;
            UiKetQuaBanCheTao.transform.GetChild(0).GetComponent<Image>().enabled = false;

            UiKetQuaBanCheTao.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            UiKetQuaBanCheTao.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
        }
        else
        {
            UiKetQuaBanCheTao.transform.GetChild(0).GetComponent<Image>().enabled = true;
            UiKetQuaBanCheTao.transform.GetChild(0).GetComponent<Image>().sprite = KetQuaBanCheTao.item.sprite;

            UiKetQuaBanCheTao.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "1";
            UiKetQuaBanCheTao.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
        }


    }
    void XuLyCheTao()
    {
        CheckCheTao();
        if (KetQuaBanCheTao!=null)
        {
            UiKetQuaBanCheTao.GetComponent<Button>().onClick.AddListener(() => { SelectKetQuaBanCheTao(); });
            UpdateInventoryUI();
            Debug.Log("Che tao thanh cong");

        }
        else
        {
            Debug.Log("Khong thanh pham");
            UiKetQuaBanCheTao.GetComponent<Button>().onClick.RemoveAllListeners();
            UpdateInventoryUI();
        }
    }
    void CheckCheTao()
    {
        for (int i = 0; i < ListAllWeapon.Length; i++)
        {
            bool isCheck2 = true;
            foreach (TileClass b in ListAllWeapon[i].nguyenLieuCheTao)
            {
                bool isCheck = false;
                foreach (InventorySlot a in BanCheTaoSlot)
                {
                    Debug.Log(isCheck);
                    if (b != null && a != null)
                    {
                        if (b.tileName.ToString() == a.item.nameTool.ToString())
                        {
                            isCheck = true;
                            
                            break;
                        }
                    }
                }
                if (isCheck==false)
                {
                    isCheck2 = false;
                    break;
                }
            }

            if(isCheck2 == true)
            {
                KetQuaBanCheTao = new InventorySlot(new ItemClass(ListAllWeapon[i]));
                return;
            }
            else
            {
                KetQuaBanCheTao = null;
            }
        }
    }
    void RemoveDoCheTao(TileClass[] nguyenLieuXoa)
    {
        foreach (TileClass nl in nguyenLieuXoa)
        {

        }
    }
    public bool AddItem(ItemClass item)
    {
        Vector2Int itemPos = Contains(item);
        if (itemPos != Vector2Int.one * -1)
        {
            InventorySlot slot = inventorySlots[itemPos.x, itemPos.y];
            Debug.Log(slot.stackLimit + " " + slot.quantity);
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
                        if (item.isImpact && inventorySlots[x, y].quantity < inventorySlots[x, y].stackLimit)
                            return new Vector2Int(x, y);
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
                if (inventorySlots[x, y] != null)
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
    void SelectItem(int i, int j)
    {
        if (pickSlot != null)
        {
            if (inventorySlots[i, j] != null)
            {
                InventorySlot t = pickSlot;
                pickSlot = inventorySlots[i, j];
                inventorySlots[i, j] = t;
            }
            else
            {
                inventorySlots[i, j] = pickSlot;
                pickSlot = null;
            }

        }
        else
        {
            if (inventorySlots[i, j] != null)
            {
                pickSlot = inventorySlots[i, j];
                inventorySlots[i, j] = null;
            }
        }
        UpdateInventoryUI();
    }
    void SelectItemBanCheTao(int i)
    {
        if (pickSlot != null)
        {
            if (BanCheTaoSlot[i] != null)
            {
                InventorySlot t = pickSlot;
                pickSlot = BanCheTaoSlot[i];
                BanCheTaoSlot[i] = t;
            }
            else
            {
                BanCheTaoSlot[i] = pickSlot;
                pickSlot = null;
            }

        }
        else
        {
            if (BanCheTaoSlot[i] != null)
            {
                pickSlot = BanCheTaoSlot[i];
                BanCheTaoSlot[i] = null;
            }
        }
        XuLyCheTao();
        UpdateInventoryUI();
    }
    void SelectKetQuaBanCheTao()
    {
        if (pickSlot == null)
        {
            if (KetQuaBanCheTao != null)
            {
                pickSlot = KetQuaBanCheTao;
                KetQuaBanCheTao = null;
            }
        }
        UpdateInventoryUI();
    }
}
