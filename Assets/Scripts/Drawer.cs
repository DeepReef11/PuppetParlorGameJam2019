using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drawer : MonoBehaviour, IDragHandler
{
    public Sprite EmptySprite;
    public Sprite ModificationSprite;

    public Text txtAmount;

    public PuppetModificationBase Modification;
    public bool canPick = true;
    public bool isRestock;

    public GameObject PickItem()
    {
        if (canPick && Modification && InventoryController.Enough(Modification))
        {
            GameObject go = Instantiate(Modification.DraggableGO, transform.position, Quaternion.identity, transform);
            //go.GetComponent<DragAndDrop>().ModificationReference = Modification;
            return go;
        }

        if (InventoryController.Enough(Modification) == false)
        {
            Debug.Log("Not enough");
        }
      
        return null;     
    }

    public void OnDrag(PointerEventData eventData)
    {
        eventData.pointerDrag = PickItem();
    }

    public void SetAvailable(bool value)
    {
        if (value)
        {
            GetComponent<Image>().sprite = ModificationSprite;
            if(isRestock == false)
                canPick = true;
        }
        else
            GetComponent<Image>().sprite = EmptySprite;

        if(txtAmount)
            txtAmount.gameObject.SetActive(value);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }

    private void Update()
    {
        if (txtAmount)
        {
            txtAmount.text = Modification.Amount.ToString();
        }

        SetAvailable(Modification.IsAvailable);
    }
}
