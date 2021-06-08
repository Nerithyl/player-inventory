using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryElement : MonoBehaviour, IDropHandler
{
    protected UnityAction <InventoryElement> m_OnElementClicked;
    protected UnityAction<InventoryElement, InventoryElement> m_OnElementDropped;

    protected Image m_ImageComponent;
    protected Button m_ButtonComponent;

    protected virtual void Awake()
    {
        m_ImageComponent = GetComponent<Image>();
        m_ButtonComponent = GetComponent<Button>();
        m_ButtonComponent.onClick.AddListener(OnButtonClicked);
    }

    public virtual void Initialize (UnityAction <InventoryElement> onCallbackElementClicked, 
                           UnityAction<InventoryElement, InventoryElement> onCallbackElementDropped, 
                           Transform draggableObjectParent = null)  //allows to use methods from the PlayerInventory class
    {
        m_OnElementClicked = onCallbackElementClicked;
        m_OnElementDropped = onCallbackElementDropped;
    }

    protected void OnButtonClicked() //delegate - a form of type-safe function pointer
    {
        m_OnElementClicked.Invoke(this);
    }

    public void SetImage(Sprite spriteImage)
    {
        m_ImageComponent.sprite = spriteImage;
    }

    public Sprite GetImage()
    {
        return m_ImageComponent.sprite;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }

        if (eventData.pointerPress != null)
        {
            InventoryElement droppedItem = eventData.pointerPress.GetComponent<InventoryElement>();

            if (droppedItem != null && m_OnElementDropped != null)
            {
                m_OnElementDropped.Invoke(droppedItem, this);
            }
        }
    }
}
 