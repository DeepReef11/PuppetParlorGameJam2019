using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// This class handle everything related to <see cref="Lane"/>. 
/// This is where lane game object should be stored so customers can be distributed
/// </summary>
public class LaneController : MonoBehaviour
{
    /// <summary>
    /// This can be used in the inspector to add lanes gameobject.
    /// </summary>
    [SerializeField]
    List<GameObject> _lanes = new List<GameObject>();

    [SerializeField]
    int _availableLanes = 6;

    /// <summary>
    /// This will display when customer enter the store and leave, etc.
    /// </summary>
    [SerializeField]
    bool _displayInformationInConsole = true;

    /// <summary>
    /// This is the queue of customers we should see in store. Once they are here, they have a timer 
    /// </summary>
    Queue<Customer> _waitingCustomers = new Queue<Customer>();

    private GameObject lanesGO;

    private void Awake()
    {
        lanesGO = GameObject.Find("Lanes");
        List<GameObject> lanes = _lanes;
        foreach(GameObject go in lanes)
        {
            if(!CheckIfLane(go))
            {
                _lanes.Remove(go);
            }
        }

    }

    private void Update()
    {
        ACustomerIsGoingToALane();
    }

    private void ACustomerIsGoingToALane()
    {
        foreach (GameObject go in _lanes)
        {
            Lane lane = go.GetComponent<Lane>();
            if (lane.isFree && _waitingCustomers.Count > 0)
            {
                lane.SendACustomerToTheLane(_waitingCustomers.Dequeue());
            }
        }     
    }

    /// <summary>
    /// Total customers in <see cref="_waitingCustomers"/> queue and in <see cref="_lanes"/>
    /// </summary>
    /// <returns></returns>
    public int NumberOfCustomersCurrentlyInStore()
    {
        return NumberOfWaitingCustomers() + NumberOfOrderingCustomers();
    }

    public int NumberOfWaitingCustomers()
    {
        return _waitingCustomers.Count();
    }

    public int NumberOfOrderingCustomers()
    {
        int customers = 0;
        foreach(GameObject go in _lanes)
        {
            Lane lane = go.GetComponent<Lane>();
            if (lane.ThisLaneIsAvailableToThePlayer && lane.CurrentCustomer != null)
            {
                customers++;
            }
        }
        return customers;
    }

    /// <summary>
    /// Programatically add a lane go if needed
    /// </summary>
    /// <param name="goLane"></param>
    public void AddALane(GameObject goLane)
    {
        if(CheckIfLane(goLane))
        {
            _lanes.Add(goLane);
        }
    }

    bool CheckIfLane(GameObject go)
    {
        if (go.GetComponent<Lane>() == null)
        {
            Debug.LogError($"Missing Lane component on the game object {gameObject.name}. It was removed from the list. Please verify.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Should be called from <see cref="LevelManager"/> and the customer should come from <see cref="LevelManager.CustomersInCurrentLevel"/>
    /// </summary>
    /// <param name="customer"></param>
    public void ACustomerEnterTheStore(Customer customer)
    {
        if (_displayInformationInConsole) Debug.Log($"The {customer.name} entered the store.");
        _waitingCustomers.Enqueue(customer);
        customer.transform.SetParent(GameManager.levelManager.LineGO.transform);
        Debug.Log(customer.transform.parent);
        customer.CustomerEnteredTheStore();
        
    }

    /// <summary>
    /// When the customer has reach its waiting limit (called from <see cref="Customer"/>)
    /// </summary>
    /// <param name="customer"></param>
    public void ACustomerIsLeaving(Customer customer)
    {
        if (_displayInformationInConsole) Debug.Log($"The customer {customer.name} leaves the store.");
        foreach (GameObject go in _lanes)
        {
            Lane lane = go.GetComponent<Lane>();
            if (lane.CurrentCustomer == customer)
            {
                
                lane.CustomerLeft();
                return;
            }
        }
        foreach (Customer c in _waitingCustomers)
        {
            if (c == customer)
            {
                _waitingCustomers = new Queue<Customer>(_waitingCustomers.Where(x => x != c));
                return;
            }
        }
        Debug.LogError("Shouldn't reach here (LaneController.TheCustomerIsLeaving)");
    }


    /// <summary>
    /// This method is only used by Level manager when changing levels from inspector.
    /// </summary>
    public void ClearCustomersBecauseLevelChanged()
    {
        _waitingCustomers.ClearGameObjects();
        foreach(GameObject go in _lanes)
        {
            go.GetComponent<Lane>().ClearCustomerBecauseLevelChanged();
        }
    }

    public void UnlockNewLane()
    {
        if (_lanes.Count == 6)
            return;

        GameObject newLane = lanesGO.transform.GetChild(_lanes.Count).gameObject;
        newLane.SetActive(true);
        newLane.GetComponent<Lane>().ThisLaneIsAvailableToThePlayer = true;
    }

}
