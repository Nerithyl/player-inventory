public class InventorySlot : InventoryElement
{
    public E_SlotType SlotType;

    public enum E_SlotType
    {
        body,
        arms,
        legs,
        weapon,
        item1,
        item2,
        empty
    }
}
