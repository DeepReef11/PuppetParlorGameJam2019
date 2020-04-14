using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Handle : PuppetMakerBase, IDropHandler, IDragHandler
{
    private bool canPick = true;
    private AudioManager _audioManager;

    override protected void Awake()
    {
        base.Awake();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    /// <summary>
    /// Add or swap a modification
    /// </summary>
    /// <param name="pm"> null if the modification slot was empty</param>
    /// <returns></returns>
    public void ApplyModification(PuppetModificationBase pm)
    {
        int pos = (int)pm.Category;
        PuppetModificationBase swap = _ModificationSlot[pos];
        if (swap != null)
        {
            InventoryController.AModificationGoesBackToTheDrawer(swap);
            Destroy(transform.Find("Puppet").Find(((ModificationCategory)pos).ToString()).gameObject);
        }

        _audioManager.Play("Swap");

        InventoryController.AModificationHasBeenTakenFromDrawer(pm);
        _ModificationSlot[pos] = pm;
        pm.PutOnhandle(transform);
        
        //pm.changestate... or maybe this should be managed in inventory


        
        /* Obsolete

        // The following is not necessary.
        if (pm.Category == ModificationCategory.Dye)
        {
            if (dye != null /*&& dye != blank)
            {
                return pm;
            }
            else dye = (DyeModification)pm;
        }
        else if (pm.Category == ModificationCategory.Hair)
        {
            PuppetModificationBase h = hair;
            hair = (HairModification)pm;
            return h;
        }
        else if(pm.Category == ModificationCategory.Eyes)
        {

        }
        //Debug.LogError("Something went wrong while applying a modification. Check if the category is managed and if PuppetModification set its Category properly");
        return null; // if nothing been swapped
    */
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragAndDrop.itemBeingDragged && DragAndDrop.itemBeingDragged.GetComponent<DragAndDrop>() && DragAndDrop.itemBeingDragged.GetComponent<DragAndDrop>().isFromHandle)
            return;

        Debug.Log(DragAndDrop.itemBeingDragged + " placed in handle");

        if(DragAndDrop.itemBeingDragged && DragAndDrop.itemBeingDragged.GetComponentInParent<PuppetModificationBase>())
            ApplyModification(DragAndDrop.itemBeingDragged.GetComponentInParent<PuppetModificationBase>());
    }

    public void ClearPuppetStand()
    {
        if (DragAndDrop.itemBeingDragged != null)
            return;

        for (int pos = 0; pos < System.Enum.GetValues(typeof(ModificationCategory)).Length; pos++)
        {
            PuppetModificationBase swap = _ModificationSlot[pos];
            if (swap != null)
            {
                InventoryController.AModificationGoesBackToTheDrawer(swap);
                Destroy(transform.Find("Puppet").Find(((ModificationCategory)pos).ToString()).gameObject);
            }
            _ModificationSlot[pos] = null;
        }

        Destroy(transform.GetChild(0).GetComponent<DragAndDrop>());
    }

    /* This methods behave similar to Drawer methods */

    public GameObject PickItem()
    {
        /* Can only drag out puppets with color and eyes ? */
        if (transform.GetChild(0).childCount > 1 && canPick)
        {
            GameObject go = transform.GetChild(0).gameObject;
            go.AddComponent<CanvasGroup>();
            if (go.GetComponent<DragAndDrop>() == null)
            {
                go.AddComponent<DragAndDrop>();
                go.GetComponent<DragAndDrop>().isFromHandle = true;
            }

            return go;
        }

        return null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        eventData.pointerDrag = PickItem();
    }
}
