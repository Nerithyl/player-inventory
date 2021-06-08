using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : InventoryElement,
             IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum E_ItemType
    {
        empty,
        armor,
        bracers,
        shoes,
        weapon,
        potion
    }

    public E_ItemType ItemType;
    public Sprite PlayerEquipment;

    private Image m_DraggableObject;
    private Transform m_DraggableObjectParent;

    public bool IsUsed;

    [SerializeField]
    private Canvas m_Canvas;

    private RectTransform m_Transform;
    private Vector2 m_BeginPosition;
    private CanvasGroup m_CanvasGroup;

    protected override void Awake()
    {
        base.Awake();
        m_Transform = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Initialize(UnityAction<InventoryElement> onCallbackElementClicked,
                       UnityAction<InventoryElement, InventoryElement> onCallbackElementDropped,
                       Transform draggableObjectParent = null)
    {
        base.Initialize(onCallbackElementClicked, onCallbackElementDropped, draggableObjectParent);
        m_DraggableObjectParent = draggableObjectParent;
    }

    public void Hide()
    {
        m_ButtonComponent.enabled = false;
        m_ImageComponent.enabled = false;
    }

    public void Show(bool sort)
    {
        m_ButtonComponent.enabled = true;
        m_ImageComponent.enabled = true;

        if (sort)
        {
            Sort();
        }
    }

    public void Sort(bool sortAsFirst = true)
    {
        if (sortAsFirst)
        {
            transform.SetAsFirstSibling();
        }
        else
        {
            transform.SetAsLastSibling();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_BeginPosition = m_Transform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_CanvasGroup.blocksRaycasts = false; //disables blocking mouse events for objects in this canvas group

        //creates a copy of an object's image component
        m_DraggableObject = Instantiate(m_ImageComponent, m_DraggableObjectParent, true); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Transform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
        m_DraggableObject.rectTransform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_CanvasGroup.blocksRaycasts = true;
        m_Transform.anchoredPosition = m_BeginPosition;
        Destroy(m_DraggableObject.gameObject);
    }

    public void SetItem(E_ItemType type, Sprite icon, Sprite playerSprite, Color color, bool flag) //copies an object's properties
    {
        ItemType = type;
        PlayerEquipment = playerSprite;
        m_ImageComponent.sprite = icon;
        m_ImageComponent.color = color;
        IsUsed = flag;
    }
}
