using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PuppetModificationBase ModificationReference;
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    public bool isFromHandle;

    private void Awake()
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isFromHandle && itemBeingDragged && itemBeingDragged.GetComponentInParent<Handle>())
        {
            var mods = itemBeingDragged.GetComponentInParent<Handle>().GetPuppet();

            if(mods[(int)ModificationCategory.Eyes] && mods[(int)ModificationCategory.Dye])
                transform.position = eventData.position;
        }
        else
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(startParent.name);
        if (isFromHandle && transform.parent == startParent)
        {
            transform.position = startPosition;
            itemBeingDragged = null;
        }
        else
        {
            Destroy(itemBeingDragged);
            itemBeingDragged = null;
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

}