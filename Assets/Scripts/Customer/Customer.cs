using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    static int ID = 0;

    /// <summary>
    /// Set by level manager
    /// </summary>
    public float WaitingInQueueMaximum;
    /// <summary>
    /// Set by level manager
    /// </summary>
    public float WaitingAtVendorMaximum;
    /// <summary>
    /// Either aproximation or complete lenght of it to know when to delete the customer
    /// </summary>
    public float LeavingStoreAnimationLength = 2.5f;

    /// <summary>
    /// Actual script of bubble; use <see cref="SetBubbleGO"/>
    /// </summary>
    public Bubble bubbleComponent { get; protected set; }
    
    /// <summary>
    /// Should be a children of Customer <see cref="SetBubbleGO"/>
    /// </summary>
    public GameObject bubbleGO { get; protected set; }

    public GameObject smiley { get; protected set; }

    private AudioManager _audioManager;

    bool _isInQueue;
    bool _isOrdering;
    bool _isLeavingTheStore;
    float _timer;
    /// <summary>
    /// How much the player get when he fulfills exactly the order
    /// </summary>
    int _OrderWorth;

    Sprite[] customerSprites;
    Sprite[] customerFaceSprites;

    private void Awake()
    {
        gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<Image>();

        customerSprites = Resources.LoadAll<Sprite>("Sprites/Puppet Customers");
        customerFaceSprites = Resources.LoadAll<Sprite>("Sprites/Smileys");

        GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI");

        GameObject go = new GameObject();
        go.name = "CustomerImg";
        go.transform.SetParent(transform);
        go.AddComponent<Image>();
        go.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(15, 20);
        go.GetComponent<Image>().sprite = customerSprites[Random.Range(1, customerSprites.Length)];

        _audioManager = FindObjectOfType<AudioManager>();
        smiley = new GameObject();
        smiley.name = "Smiley";
        smiley.transform.SetParent(transform);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = $"Customer {ID++}";
        
        _timer = 0f;
        _isInQueue = false;
        _isOrdering = false;
        _isLeavingTheStore = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isInQueue && _timer > WaitingInQueueMaximum)
        {
            CustomerLeavesWithoutOrdering();
        }
        else if(_isOrdering && _timer > WaitingAtVendorMaximum)
        {
            CustomerLeavesWithoutOrdering();
        }
        else if(_isLeavingTheStore && _timer > LeavingStoreAnimationLength)
        {

            GameManager.laneController.ACustomerIsLeaving(this);
            RemoveCustomerFromGame();

        }
        _timer += Time.deltaTime;
    }

    /// <summary>
    /// Once bubble is generated, use this method to set it on customer
    /// </summary>
    /// <param name="bgo"></param>
    public void SetBubbleGO(GameObject bgo)
    {
        Bubble b = bgo.GetComponent<Bubble>();
        if (b != null)
        {           
            bubbleComponent = b;
            EvaluateOrderWorth(b);
            bgo.transform.SetParent(transform);
            bubbleGO = bgo;
            bubbleGO.AddComponent<Image>();
            bubbleGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Bubble");
            bubbleGO.GetComponent<Image>().enabled = false;
            bubbleGO.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
            bubbleGO.GetComponent<RectTransform>().position = new Vector2(-16, 40);
        }
    }

    void EvaluateOrderWorth(Bubble b)
    {
        int worth = 0;
        foreach(PuppetModificationBase pmb in b.GetPuppet())
        {
            if(pmb != null)
                worth += PriceConstants.GetCost(pmb.Category);
        }
        _OrderWorth =  Mathf.FloorToInt(worth * 2.25f);
    }

    /// <summary>
    /// From <see cref="LevelManager.CustomersInCurrentLevel"/> queue to <see cref="LaneController._waitingCustomers"/> queue
    /// </summary>
    public void CustomerEnteredTheStore()
    {
        _timer = 0f;
        _isInQueue = true;
        _isOrdering = false;
        _isLeavingTheStore = false;
    }


    /// <summary>
    /// From <see cref="LaneController._waitingCustomers"/> queue to <see cref="Lane.CurrentCustomer"/>
    /// </summary>
    public void CustomerIsOrdering()
    {
        _timer = 0f;
        _isInQueue = false;
        _isOrdering = true;
        _isLeavingTheStore = false;

        /* Hides angry */
        smiley.SetActive(false);

        bubbleGO.SetActive(true);
    }

    float _firstDamageToWorth = .45f;
    float _nextDamageToWorth = .15f;

    int EvaluateTheReceivedPuppet(Handle handle)
    {
        float worth = _OrderWorth;
        float damage = _firstDamageToWorth;
        List<PuppetModificationBase> PlayersPuppet =  handle.GetPuppet();
        List<PuppetModificationBase> CustomersPuppet = bubbleComponent.GetPuppet();
        for (int i = 0; i < PlayersPuppet.Count; i++)
        {
            if(
                (PlayersPuppet[i] != null && CustomersPuppet[i] != null
                    && PlayersPuppet[i].Modification != CustomersPuppet[i].Modification) 
                ||
                (PlayersPuppet[i] == null && CustomersPuppet[i] != null)
                ||
                (PlayersPuppet[i] != null && CustomersPuppet[i] == null)
                )
            {
                worth -= _OrderWorth * damage;
                if(damage == _firstDamageToWorth)
                {
                    damage = _nextDamageToWorth;
                }
            }
        }

        //GameObject go = new GameObject();
        AddSmiley(smiley);

        //Clear PlayersPuppet List
        handle.InitializeModList();

        int amountPaid = Mathf.FloorToInt(worth);

        if (amountPaid == _OrderWorth)
            smiley.GetComponent<Image>().sprite = customerFaceSprites[1];
        else if(amountPaid < _OrderWorth)
            smiley.GetComponent<Image>().sprite = customerFaceSprites[0];

        Debug.Log($"{gameObject.name} paid {amountPaid} for the puppet. Is order was worth {_OrderWorth}");
        return amountPaid;
    }

    /// <summary>
    /// This method evaluate the resulting puppet and may return money eventually (TODO)
    /// </summary>
    /// <param name="Puppet"></param>
    public void CustomerReceivedItsOrder(GameObject Puppet)
    {
        _isInQueue = false;
        _isOrdering = false;
        _isLeavingTheStore = true;
        _timer = 0f;
        // Moved to update to let smiley appear
        //GameManager.laneController.ACustomerIsLeaving(this);

        Handle handle = Puppet.GetComponentInParent<Handle>();
        if (handle != null)
        {
            EconomyController.Addcoins(EvaluateTheReceivedPuppet(handle));
            _audioManager.Play("Purchase");
        }
        else
        {
            // Should never get here.
            Debug.LogError($"The {gameObject.name} received a Game Object that doesn't contain handles. The pupet cannot be evaluated.");
        }

    }


    void CustomerLeavesWithoutOrdering()
    { 
        
        AddSmiley(smiley);
        smiley.GetComponent<Image>().sprite = customerFaceSprites[2];
      
        // Launch leaving animation
        _isInQueue = false;
        _isOrdering = false;
        _isLeavingTheStore = true;
        _timer = 0f;
        // Moved in update to let the customer smiley
        //GameManager.laneController.ACustomerIsLeaving(this);
    }

    private void AddSmiley(GameObject go)
    {
        if (go != null)
        {
            go.SetActive(true);
            go.name = "Smiley";
            go.transform.SetParent(this.transform);
            go.AddComponent<RectTransform>();

            go.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            go.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
            go.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);

            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 40);

            go.AddComponent<Image>();

            bubbleGO.SetActive(false);
        }
    }

    /// <summary>
    /// When customer exit the store or when changing level from inspector (from <see cref="CustomerQueueExtension.ClearGameObjects"/>
    /// </summary>
    public void RemoveCustomerFromGame()
    {
        Destroy(gameObject);
    }

}
