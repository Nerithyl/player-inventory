using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour
{
    private UnityAction<InventoryTab> m_OnTabClicked;
    private Button m_ButtonComponent;

    public E_TabType TabType;

    public enum E_TabType
    {
        allItems,
        body,
        arms,
        legs,
        weapons,
        items
    }
    public void Awake()
    {
        m_ButtonComponent = GetComponent<Button>();
        m_ButtonComponent.onClick.AddListener(OnButtonClicked);
    }

    public void Initialize(UnityAction<InventoryTab> onCallbackTabClicked) //allows to use methods from the PlayerInventory class
    {
        m_OnTabClicked = onCallbackTabClicked;
    }

    protected void OnButtonClicked() //delegate - a form of type-safe function pointer
    {
        if (m_OnTabClicked != null)
        {
            m_OnTabClicked.Invoke(this);
        } 
    }
}
