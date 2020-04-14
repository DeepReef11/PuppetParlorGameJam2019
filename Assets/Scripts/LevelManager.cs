using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public enum Month
{
    January,
    February,
    March,
    April,
    May,
    June,
    July,
    August,
    September,
    October,
    November,
    December

}

public static class CustomerQueueExtension
{
    public static void ClearGameObjects(this Queue<Customer> customers)
    {
        while(customers.Count>0)
        {
            Customer c = customers.Dequeue();
            c.RemoveCustomerFromGame();
        }
    }
}

public class LevelManager : MonoBehaviour
{

    /// <summary>
    /// This contain one list of each category of modifications that contain a list of all modification ever.
    /// Initialized in <see cref="Awake"/>
    /// </summary>
    public List<List<PuppetModificationBase>> AllModifications { get; private set; }
    /// <summary>
    /// This contain one list of each category of modifications that contain a list of all AVAILABLE modification ever.
    /// Initialized in <see cref="Awake"/>
    /// </summary>
    public List<List<PuppetModificationBase>> AvailableModifications { get; private set; }
    /// <summary>
    /// This is where the customers should be reparented when they enter the store
    /// </summary>
    public GameObject LineGO;

    private static System.Random random = new System.Random();

    /// <summary>
    /// Level should be equal to month*4 + week (+ number of time the game was completed * 48)
    /// </summary>
    [SerializeField]
    [Min(0)]
    private int level;
    public int Level { get { return level; } private set { level = value; } }
    /// <summary>
    /// Each month has 4 weeks that correspond to a level
    /// </summary>
    [Range(1, 4)]
    public int Week;
    /// <summary>
    /// Each month ends by an event. The player must have gotten a money goal to go to the next month, 
    /// otherwise the player goes back to first week of this month
    /// </summary>
    public Month month;

    //Customer related
    /// <summary>
    /// These customer are not actually in game, it is as if they were customer outside of the store.
    /// <see cref="LaneController"/> 
    /// </summary>
    [SerializeField]
    private Queue<Customer> CustomersInCurrentLevel;

    public UIManager UIManager;
    public AudioManager AudioManager;
    PuppetModificationBase trending;

    public AudioClip InGame;
    public AudioClip NightPhase;

    private void Awake()
    {
        AllModifications = new List<List<PuppetModificationBase>>();
        AvailableModifications = new List<List<PuppetModificationBase>>();

        InitializeGame();
        _currentLevel = 0;
    }

    private void Start()
    {
        //NextLevel();
    }

    void InitializeGame()
    {
        
        month = (Month)0;
        Level = 0;
        Week = 1;
        CustomersInCurrentLevel = new Queue<Customer>();
        for (int i = 0; i < System.Enum.GetNames(typeof(ModificationCategory)).Length; i++)
        {
            AllModifications.Add(new List<PuppetModificationBase>());
        }

        Debug.Log("levelManagement initialization done.");
    }

    float _sendCustomerWait = 0f;
    float _sendCustomerTimer = 0f;
    int _currentLevel;
    public bool InspectorChangeLevel = false;
    private void Update()
{
        _sendCustomerTimer += Time.deltaTime;
        if (InspectorChangeLevel == true)
        {
            ClearAllCustomersinGame();
            InspectorChangeLevel = false;
        }
        if (TotalRemainingCustomersInCurrentLevel() == 0 || IsNightPhase)
        {
            // Go to stocking Modifications and upgrades
            if (IsNightPhase == false)
            {
                IsNightPhase = true;
                // CHANGE AUDIO
                GameManager.audioManager.PlayOnTopOfMainTheme("NightPhaseTheme", .01f);
                NextLevel();        
            }   

            if (ContinueToNextLevelbtnclick == true)
            {
                IsNightPhase = false;
                GameManager.audioManager.PlayOnTopOfMainTheme("NightPhaseTheme", .1f);
                ContinueToNextLevelbtnclick = false;
                _sendCustomerTimer = 0f;
                _sendCustomerWait = 0f;
                BagOfPossibility.Clear();
                GenerateCurrentLevel();
                _currentLevel = Level;
                UIManager.UpdateRestock(IsNightPhase);
            }
        }
        if (CustomersInCurrentLevel.Count > 0 && _sendCustomerTimer > _sendCustomerWait)
        {
            GameManager.laneController.ACustomerEnterTheStore(CustomersInCurrentLevel.Dequeue());
            GenerateSendCustomerWait();
            _sendCustomerTimer = 0f;
        }
        
    }

    public bool IsNightPhase = false;
    public bool ContinueToNextLevelbtnclick = false;

    public void ContinueToNextLevel()
    {
        ContinueToNextLevelbtnclick = true;
    }

    public int TotalRemainingCustomersInCurrentLevel()
    {
        return GameManager.laneController.NumberOfCustomersCurrentlyInStore() + CustomersInCurrentLevel.Count;
    }

    List<float> BagOfPossibility = new List<float>();

    /// <summary>
    /// Generate the time to wait and store it in see cref="_sendCustomerWait"/>
    /// </summary>
    void GenerateSendCustomerWait()
    {
        if (BagOfPossibility.Count == 0)
        {
            for (int i = 0; i < TotalCustomerForThisLevel; i++)
            {
                int dif = Level > 24 ? 5 : 7;
                BagOfPossibility.Add(((i * (Level % dif == 0 ? dif + 6 : Level) * 17) % (Level > 24 ? 5 : 7)) + (i / TotalCustomerForThisLevel) * 2 + (4f / Mathf.Sqrt(Level)));
            }
        }

        int pick = UnityEngine.Random.Range(0, BagOfPossibility.Count);
        _sendCustomerWait = BagOfPossibility[pick];
        BagOfPossibility.RemoveAt(pick);

        if (GameManager.laneController.NumberOfWaitingCustomers() == 0 && _sendCustomerWait > 4f)
        {
            _sendCustomerWait = 3f;
        }
    }

    protected void NextLevel()
    {
   

        Debug.Log($"Level {Level} Completed.");
        if ((Level % 4 == 0 && AsReachedMonthlyGoal()) || Level % 4 != 0)
        {
            if(Level % 4 == 0)
                GetComponent<LaneController>().UnlockNewLane();

            Debug.Log($"Next Level");
            Level++;
        }
        else
        {           
            int previousMonth = Level - 4;
            while(!(previousMonth > 0) || !AsReachedMonthlyGoal(previousMonth))
            {
                previousMonth -= 4;
            }
            Level++;
            Debug.Log($"Didn't reach the monthly goal. Sent back to level {Level}.");
        }

        AsReachedMonthlyGoal();

        Week = ((Level - 1) % 4) + 1;
        UIManager.UpdateWeek(Week);
        month = (Month)((Mathf.FloorToInt((Level - 1f) / 4f)) % 12);
        UIManager.UpdateMonth(month);
        AvailableModifications = UpdateAvailableModifications();
        CreateTrending();
;
        UIManager.UpdateRestock(IsNightPhase);
        AudioManager.Play("NightTime");

    }

    [SerializeField]
    [Tooltip("Changing this property will not change the actual number of customers")]
    int TotalCustomerForThisLevel;

    /// <summary>
    /// All the difficulty algorithm and logic goes here
    /// </summary>
    void GenerateCurrentLevel()
    {
        
        foreach (List<PuppetModificationBase> lpmb in AllModifications)
        {
            foreach (PuppetModificationBase pmb in lpmb)
            {
                pmb.CheckIfModificationIsAvailableAndActivateOrDesactivate();
            }
        }

        //AvailableModifications = UpdateAvailableModifications(); // must stay here for inspector level changes
        GenerateCurrentLevelCustomers();

        
    }

    bool AsReachedMonthlyGoal(int lvl = 0)
    {
        if (lvl == 0) lvl = Level;

        if(CalculateMinPerfectAmountMoneyForLevel(Mathf.CeilToInt(lvl/4f)*4) <= EconomyController.GetCoins() || Level == 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gives the amount of minimum perfect sales for a month per customers.
    /// </summary>
    int CalculateMinPerfectAmountMoneyForLevel(int lvl)
    {
        int customers = 0;
        for (int i = 0; i < lvl; i++)
        {
            customers += Gen.GenerateNumberOfCustomersInLevel(lvl - i);
        }

        int value = customers * (PriceConstants.EyesCost + PriceConstants.DyeCost) - 200;

        if (value > 0)
            UIManager.UpdateGoal(value);

        return value;

    }


    /// <summary>
    /// Called from <see cref="GenerateCurrentLevel"/>
    /// </summary>
    void GenerateCurrentLevelCustomers()
    {

        ClearAllCustomersinGame(); // This is simply when changing level from inspector
        _sendCustomerTimer = 0f;
        //GenerateSendCustomerWait();
        TotalCustomerForThisLevel = Gen.GenerateNumberOfCustomersInLevel(Level);
        for (int i = 0; i < TotalCustomerForThisLevel; i++)
        {
            CustomersInCurrentLevel.Enqueue(GenerateALevelCustomer());
        }
        Debug.Log($"Number of customer in this level: {CustomersInCurrentLevel.Count}");
    }

    void ClearAllCustomersinGame()
    {
        CustomersInCurrentLevel.ClearGameObjects();
        GameManager.laneController.ClearCustomersBecauseLevelChanged();
    }

    /// <summary>
    /// Called from <see cref="GenerateCurrentLevelCustomers"/>. Generate a customers that correspond to the difficulty of the level
    /// </summary>
    /// <returns></returns>
    Customer GenerateALevelCustomer()
    {
        GameObject go = new GameObject();
        Customer c = go.AddComponent<Customer>();
        float minWaiting = 10f;
        float maxWaiting = (60f / Mathf.Sqrt((float)Level / 2f)) + minWaiting;
        float median = (minWaiting + maxWaiting) / 2f;
        c.WaitingInQueueMaximum = (Mathf.Lerp(minWaiting, maxWaiting,
            ((float)(TotalCustomerForThisLevel - CustomersInCurrentLevel.Count) / (float)TotalCustomerForThisLevel))
            + median) / 2f;

        c.WaitingAtVendorMaximum = c.WaitingInQueueMaximum * 1.25f;

        c.SetBubbleGO(GenerateBubble());


        return c;
    }

    /// <summary>
    /// Remove the customer from the queue and returns it. Note that once it is done, the Customer isn't part of the level manager anymore.
    /// The customer must be handled somewhere else (like give puppet to customer).
    /// </summary>
    /// <returns></returns>
    public Customer GetNextCustomer()
    {
        return CustomersInCurrentLevel.Dequeue();
    }

    /// <summary>
    /// Only keep one 
    /// </summary>
    /// <param name="pmb"></param>
    public void RegisterNewModification(PuppetModificationBase pmb)
    {

        //PuppetModificationBase isItRegistered = AllModifications[(int)pmb.Category].Find(x => x.GetType() == pmb.GetType());
        PuppetModificationBase isItRegistered = AllModifications[(int)pmb.Category].Find(x => x.Modification == pmb.Modification);
        if (isItRegistered == null)
        {
            Debug.Log("A new modification as been registered: " + pmb.name);
            AllModifications[(int)pmb.Category].Add(pmb);
        }
        else
        {
            Debug.LogError($"{pmb.name} has already been registered. From Gameobject: {GameManager.GetFullParentList(pmb.transform)}");
        }
    }

    /// <summary>
    /// Returns a list of lists with all currently available modifications of each category
    /// </summary>
    private List<List<PuppetModificationBase>> UpdateAvailableModifications()
    {
        var list = new List<List<PuppetModificationBase>>();

        foreach (ModificationCategory cat in Enum.GetValues(typeof(ModificationCategory)))
        {
            list.Add(new List<PuppetModificationBase>());
            foreach (PuppetModificationBase mod in AllModifications[(int)cat])
            {
                
                mod.CheckIfModificationIsAvailableAndActivateOrDesactivate();
                if (mod.IsAvailable)
                    list[(int)cat].Add(mod);
            }
        }

        return list;
    }

    private GameObject GenerateBubble()
    {
        int nModif = 0;
        int min = Level < 40 ? Mathf.FloorToInt((float)Level/20f) + 2: System.Enum.GetNames(typeof(ModificationCategory)).Length;
        int max = Level < 15 ? Mathf.FloorToInt((float)Level/5f) + 2 : System.Enum.GetNames(typeof(ModificationCategory)).Length;
        List<PuppetModificationBase> lpmb = new List<PuppetModificationBase>();
        for (int modificationCategory = 0; modificationCategory < System.Enum.GetNames(typeof(ModificationCategory)).Length; modificationCategory++)
        {
            lpmb.Add(null);

            if ((ModificationCategory)modificationCategory == ModificationCategory.Eyes)
            {
                //TODO Math

                var isTrending = CheckTrending((ModificationCategory)modificationCategory);
                if (isTrending != null)
                    lpmb[modificationCategory] = isTrending;
                else
                    lpmb[modificationCategory] = PickRandomFromList(AvailableModifications[modificationCategory]);
            }
            else if ((ModificationCategory)modificationCategory == ModificationCategory.Dye)
            {
                var isTrending = CheckTrending((ModificationCategory)modificationCategory);
                if (isTrending != null)
                    lpmb[modificationCategory] = isTrending;
                else
                    lpmb[modificationCategory] = PickRandomFromList(AvailableModifications[modificationCategory]);
            }
            else
            {
                if (nModif < max || nModif < min)
                {
                    Debug.Log($"min:{min} max:{max}");
                    if (random.Next(0, 2) == 1)
                    {
                        var isTrending = CheckTrending((ModificationCategory)modificationCategory);
                        if (isTrending != null)
                            lpmb[modificationCategory] = isTrending;
                        else
                            lpmb[modificationCategory] = PickRandomFromList(AvailableModifications[modificationCategory]);
                        
                    }
                }
            }
            if(lpmb[modificationCategory] != null)
            {
                Debug.Log("IN " + nModif);
                nModif++;
            }
            //Todo other categories
        }
        GameObject bubbleGO = new GameObject();
        Bubble b = bubbleGO.AddComponent<Bubble>();
        b.SetBubble(lpmb);
        return bubbleGO;
    }

    PuppetModificationBase PickRandomFromList(List<PuppetModificationBase> listOfmodif)
    {
        if (listOfmodif.Count > 0)
            return listOfmodif[UnityEngine.Random.Range(0, listOfmodif.Count)];
        else
            return null;
    }

    private void CreateTrending()
    {     
        if (Week == 4) {
            foreach(List<PuppetModificationBase> lpmb in AvailableModifications)
            {
                foreach(PuppetModificationBase pmb in lpmb)
                {
                    foreach(Month m in pmb.TrendOfTheseMonthsEvent)
                    {
                        if(m == month)
                        {
                            trending = pmb;
                            break;
                        }
                    }
                }
            }
            if(trending == null)
            {
                trending = PickRandomFromList(AvailableModifications[UnityEngine.Random.Range(0,Enum.GetNames(typeof(ModificationCategory)).Length)]);

            }
        }

        if (Week == 1)
            trending = null;

        UIManager.SetTrending(trending);
    }

    private PuppetModificationBase CheckTrending(ModificationCategory mod)
    {
        if(level % 4 == 0 && trending && mod == trending.Category){

            if(UnityEngine.Random.Range(0, 4) == 0)
            {
                return trending;
            }
        }

        return null;
    }

}
