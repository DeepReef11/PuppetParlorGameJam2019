using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class is basicly one place for a customer to ask for a puppet. So if there can be 3 customers asking for a puppet at the same time,
/// there should be 3 GameObject containing this component.
/// </summary>
public class Lane : MonoBehaviour, IDropHandler
{

    public Customer CurrentCustomer;
    /// <summary>
    /// Is the lane unlocked in game
    /// </summary>
    public bool ThisLaneIsAvailableToThePlayer = false;
    public bool isFree { get; private set; }

    private AudioManager _audioManager;
    private object customerLock;

    // Start is called before the first frame update
    void Start()
    {
        isFree = true;
        gameObject.SetActive(ThisLaneIsAvailableToThePlayer);
        GameManager.laneController.AddALane(gameObject);

        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendACustomerToTheLane(Customer customer)
    {
        if (isFree && CurrentCustomer == null)
        {
            
            CurrentCustomer = customer;
            isFree = false;
            _audioManager.Play("SpeechBubble");
            if (CurrentCustomer) CurrentCustomer.CustomerIsOrdering();
            if (CurrentCustomer) CurrentCustomer.transform.SetParent(gameObject.transform);

            if (CurrentCustomer) CurrentCustomer.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
            if (CurrentCustomer) CurrentCustomer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            if (CurrentCustomer) CurrentCustomer.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
            if (CurrentCustomer) CurrentCustomer.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);

            if (CurrentCustomer) CurrentCustomer.bubbleComponent.GetComponent<Image>().enabled = true;

            if (CurrentCustomer)
            {
                foreach (PuppetModificationBase mod in CurrentCustomer.bubbleComponent.GetPuppet())
                {
                    if (mod != null/* && CurrentCustomer*/)
                        Instantiate(mod.OnHandleGO, CurrentCustomer.bubbleGO.transform);
                }
            }

            if (CurrentCustomer)
                Destroy(CurrentCustomer.GetComponent<Image>());
                     
            

        }
        else
        {
            Debug.LogError("Shouldn't reach here.");
        }
    }
    public void CustomerLeft()
    {
        CurrentCustomer = null;
        isFree = true;
    }

    public void ClearCustomerBecauseLevelChanged()
    {
        if(CurrentCustomer != null)
            Destroy(CurrentCustomer.gameObject);
        isFree = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Save it because DragAndDrop deletes the item
        var itemDropped = DragAndDrop.itemBeingDragged;

        if (itemDropped && itemDropped.GetComponent<DragAndDrop>() && itemDropped.GetComponent<DragAndDrop>().isFromHandle && CurrentCustomer != null)
        {
            foreach(Transform child in DragAndDrop.itemBeingDragged.transform)
                Destroy(child.gameObject);

            DragAndDrop.itemBeingDragged = null;
        }

 
        Debug.Log(itemDropped.name + " placed on lane " + gameObject.name);
        if(itemDropped.GetComponentInParent<Handle>() != null && CurrentCustomer != null)
            CurrentCustomer.CustomerReceivedItsOrder(itemDropped);
    }
}
