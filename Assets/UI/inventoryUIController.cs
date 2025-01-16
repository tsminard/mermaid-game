using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq; 

public class inventoryUIController : MonoBehaviour
{

    public Dictionary<int, InventorySlot> inventorySlotsById = new Dictionary<int, InventorySlot>();
    private List<InventorySlot> inventorySlots = new List<InventorySlot>(); // keeping this list so we can quickly check for overlap when doing drag + drop 
    private VisualElement m_root;
    public VisualElement m_inventoryWindow;

    // handle drag + drop
    private static VisualElement m_ghostIcon;
    private static bool isDragging;
    private static InventorySlot m_originalSlot;
    private void Awake()
    {
        m_root = GetComponent<UIDocument>().rootVisualElement; // retrieve root from UI document
        m_inventoryWindow = m_root.Query("SlotsContainer"); // retrieve Inventory from the UI document. this should hold slots
        m_ghostIcon = m_root.Query("GhostElement");

        for(int i = 0; i < 20; i++)
        {
            InventorySlot item = new InventorySlot(i);
            inventorySlotsById.Add(i, item);
            inventorySlots.Add(item);
            m_inventoryWindow.Add(item);
        }
        // add event listeners
        inventoryController.onInventoryChanged += inventoryController_onInventoryChanged;  // add event listener so our UI will change when items change
        m_ghostIcon.RegisterCallback<PointerMoveEvent>(onPointerMove);
        m_ghostIcon.RegisterCallback<PointerUpEvent>(onPointerUp);
    }

    // adds UI representation of items into our UIInventory
    private void inventoryController_onInventoryChanged(int id, ItemDetails itemDetails, InventoryChangeType inventoryChangeType)
    {
        Debug.Log("calling OnInventoryChanged");
        if(inventoryChangeType == InventoryChangeType.Pickup)
        {
            InventorySlot inventorySlot = inventorySlotsById[id];
            if(inventorySlot != null)
            {
                inventorySlot.holdItem(itemDetails);
            }
            else
            {
                Debug.Log("Could not find a valid inventory slot with ID " + id + " : unable to store item");
            }
        }
    }

    // Drag + Drop methods
    // starts dragging
    public static void StartDrag(Vector2 pos, InventorySlot originalSlot)
    {
        isDragging = true;
        m_originalSlot = originalSlot;
        // center ghost icon on the mouse
        m_ghostIcon.style.top = pos.y - m_ghostIcon.layout.height / 2;
        m_ghostIcon.style.left = pos.x - m_ghostIcon.layout.width / 2;
        // set icon to be icon of original slot
        m_ghostIcon.style.backgroundImage = Background.FromSprite(inventoryController.getItemByLocation(m_originalSlot.locID).itemData.icon);
        m_ghostIcon.style.visibility = Visibility.Visible;
    }

    // move ghost icon while cursor is being moved
    private void onPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging)
        {
            return;
        }
        m_ghostIcon.style.top = evt.position.y - m_ghostIcon.layout.height / 2;
        m_ghostIcon.style.left = evt.position.x - m_ghostIcon.layout.width / 2;
    }

    private void onPointerUp(PointerUpEvent evt)
    {
        if (!isDragging)
        {
            return;
        }

        // check if we're overlapping anything
        IEnumerable<InventorySlot> slots = inventorySlots.Where(x => x.worldBound.Overlaps(m_ghostIcon.worldBound));
        if(slots.Count() != 0)
        {
            InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance(x.worldBound.position, m_ghostIcon.worldBound.position)).First(); 
            
        }
    }
}
