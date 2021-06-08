using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField]
    List<InventoryItem> m_InventoryItemList = new List<InventoryItem>();

    [SerializeField]
    List<InventorySlot> m_InventorySlotList = new List<InventorySlot>();

    [SerializeField]
    List<InventoryTab> m_InventoryTabList = new List<InventoryTab>();

    private InventoryItem m_LastClickedItem;

    private InventoryItem m_LastUsedArmor, m_LastUsedBracers, m_LastUsedShoes,
                          m_LastUsedWeapon, m_LastUsedItem1, m_LastUsedItem2;

    [SerializeField]
    private Transform m_DraggableObjectParent; //used to move a copy of the item's image component in the main area

    //images displayed on top of the player's character, used to simulate wearing the corresponding equipment pieces

    [SerializeField]
    private Image m_PlayerBody;

    [SerializeField]
    private Image m_PlayerArms;

    [SerializeField]
    private Image m_PlayerLegs;

    [SerializeField]
    private Image m_PlayerWeapon;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (InventoryItem item in m_InventoryItemList)
        {
            item.Initialize(OnItemClicked, OnItemDropped, m_DraggableObjectParent);

            if (item.ItemType == InventoryItem.E_ItemType.empty)
            {
                item.Sort(false);
            }
        }

        foreach (InventorySlot slot in m_InventorySlotList)
        {
            slot.Initialize(UseMatchingType, OnItemDropped);
        }

        foreach (InventoryTab tab in m_InventoryTabList)
        {
            tab.Initialize(OnTabClicked);
        }
    }

    private void OnTabClicked(InventoryTab clickedTab)
    {
        foreach (InventoryItem item in m_InventoryItemList)
        {
            if (SelectItemsToShow(item, clickedTab))
            {
                item.Show(clickedTab.TabType != InventoryTab.E_TabType.allItems); //items in the allItems tab won't be sorted
            }
            else
            {
                item.Hide();
            }
        }
    }

    private bool SelectItemsToShow(InventoryItem item, InventoryTab tab)
    {
        switch (tab.TabType)
        {
            case InventoryTab.E_TabType.allItems:
                return true;

            case InventoryTab.E_TabType.body:
                return (item.ItemType == InventoryItem.E_ItemType.armor);

            case InventoryTab.E_TabType.arms:
                return (item.ItemType == InventoryItem.E_ItemType.bracers);

            case InventoryTab.E_TabType.legs:
                return (item.ItemType == InventoryItem.E_ItemType.shoes);

            case InventoryTab.E_TabType.weapons:
                return (item.ItemType == InventoryItem.E_ItemType.weapon);

            case InventoryTab.E_TabType.items:
                return (item.ItemType == InventoryItem.E_ItemType.potion);

            default:
                return false;
        }
    }

    private void OnItemClicked(InventoryElement clickedButton)
    {
        m_LastClickedItem = (InventoryItem)clickedButton;
    }

    private void OnItemDropped(InventoryElement droppedItem, InventoryElement targetDestination)
    {
        InventoryItem item = droppedItem as InventoryItem;

        if (targetDestination is InventorySlot)
        {
            InventorySlot slot = targetDestination as InventorySlot;

            if (item.IsUsed)
            {
                return;
            }

            if (item != null && slot != null && IsTypeMatching(item, slot))
            {
                Image img = SetPlayerImage(slot.SlotType);
                slot.SetImage(item.GetImage());

                if (img != null)
                {
                    img.sprite = item.PlayerEquipment;
                }

                UseItem(slot.SlotType, item);
            }
        }
        else
        {
            InventoryItem anotherItem = targetDestination as InventoryItem;

            if (item == anotherItem)
            {
                return;
            }

            if (item != null && anotherItem != null)
            {
                InventoryItem.E_ItemType tempType = anotherItem.ItemType;
                Sprite tempImage = anotherItem.GetImage();
                Sprite tempSprite = anotherItem.PlayerEquipment;
                Color tempColor = anotherItem.GetComponent<Image>().color;
                bool tempFlag = anotherItem.IsUsed;

                anotherItem.SetItem(item.ItemType, item.GetImage(), item.PlayerEquipment, item.GetComponent<Image>().color, item.IsUsed);
                item.SetItem(tempType, tempImage, tempSprite, tempColor, tempFlag);

            }
            else if (item != null && anotherItem == null)
            {
                anotherItem.SetItem(item.ItemType, item.GetImage(), item.PlayerEquipment, item.GetComponent<Image>().color, item.IsUsed);
                item.SetItem(InventoryItem.E_ItemType.empty, null, null, Color.white, false);

            }
        }
    }

    private void OnSlotClicked(InventoryElement clickedButton)
    {
        InventorySlot slot = clickedButton as InventorySlot;
        slot.SetImage(m_LastClickedItem.GetImage());
        UseItem(slot.SlotType, m_LastClickedItem);
    }

    private void UseMatchingType(InventoryElement clickedButton)
    {
        InventorySlot slot = clickedButton as InventorySlot;

        Image img = SetPlayerImage(slot.SlotType);

        if (IsTypeMatching(m_LastClickedItem, slot))
        {
            OnSlotClicked(clickedButton);

            if (img != null)
            {
                img.sprite = m_LastClickedItem.PlayerEquipment;
            }
        }
    }

    private bool IsTypeMatching(InventoryItem item, InventorySlot slot)
    {
        switch (slot.SlotType)
        {
            case InventorySlot.E_SlotType.body:
                return (item.ItemType == InventoryItem.E_ItemType.armor);

            case InventorySlot.E_SlotType.arms:
                return (item.ItemType == InventoryItem.E_ItemType.bracers);

            case InventorySlot.E_SlotType.legs:
                return (item.ItemType == InventoryItem.E_ItemType.shoes);

            case InventorySlot.E_SlotType.weapon:
                return (item.ItemType == InventoryItem.E_ItemType.weapon);

            case (InventorySlot.E_SlotType.item1):
                return (item.ItemType == InventoryItem.E_ItemType.potion);

            case (InventorySlot.E_SlotType.item2):
                return (item.ItemType == InventoryItem.E_ItemType.potion);

            case InventorySlot.E_SlotType.empty:
                return true;

            default:
                return false;
        }
    }

    private void UseItem(InventorySlot.E_SlotType type, InventoryItem item)
    {
        item.IsUsed = true;

        switch (type)
        {
            case InventorySlot.E_SlotType.body:
                m_LastUsedArmor = ChangeUsedItem(m_LastUsedArmor, item);
                break;

            case InventorySlot.E_SlotType.arms:
                m_LastUsedBracers = ChangeUsedItem(m_LastUsedBracers, item);
                break;

            case InventorySlot.E_SlotType.legs:
                m_LastUsedShoes = ChangeUsedItem(m_LastUsedShoes, item);
                break;

            case InventorySlot.E_SlotType.weapon:
                m_LastUsedWeapon = ChangeUsedItem(m_LastUsedWeapon, item);
                break;

            case (InventorySlot.E_SlotType.item1):
                m_LastUsedItem1 = ChangeUsedItem(m_LastUsedItem1, item);
                break;

            case (InventorySlot.E_SlotType.item2):
                m_LastUsedItem2 = ChangeUsedItem(m_LastUsedItem2, item);
                break;

            default:
                break;
        }
    }

    private InventoryItem ChangeUsedItem(InventoryItem previouslyUsedItem, InventoryItem currentlyUsedItem)
    {
        if (previouslyUsedItem != null)
        {
            previouslyUsedItem.GetComponent<Image>().color = Color.white;
            previouslyUsedItem.IsUsed = false;
        }

        previouslyUsedItem = currentlyUsedItem;
        currentlyUsedItem.GetComponent<Image>().color = Color.gray;

        return previouslyUsedItem;
    }

    private Image SetPlayerImage (InventorySlot.E_SlotType type)
    {
        switch (type)
        {
            case InventorySlot.E_SlotType.body:
                return m_PlayerBody;

            case InventorySlot.E_SlotType.arms:
                return m_PlayerArms;

            case InventorySlot.E_SlotType.legs:
                return m_PlayerLegs;

            case InventorySlot.E_SlotType.weapon:
                return m_PlayerWeapon;

            default:
                return null;
        }
    }
}
