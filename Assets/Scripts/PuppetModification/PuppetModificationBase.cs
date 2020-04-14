using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ModificationCategory {Dye, Hair, Eyes, Accessory}

/// <summary>
/// Full list of everything possible
/// </summary>
public enum ModificationType 
{
    // Level 1 (January)
    BlankPuppet,
    SewedEyes,  // Infinite
    // level 2
    ButtonEyes,
    //level 3 NORMAL EYES MOVED HERE
    // Level 5 -> SWAPPED WITH FLOWER HAIR -> level 19
    LongHair,
    // Level 4 First trend Event
    // Long hair trend (introduction)
    // level 5 (February)
    OrangePuppet,
    // level 6
    PurplePuppet,
    // level 7
    // Nohing new
    // Level 8 Valentine Event
    // Trend on red and pink
    // Level 9 (march)
    CuteBlackEyes,
    // Level 10
    Shades,
    // Level 11
    // Level 12 (Geek Week event)
    // Trend on glasses
    // Level 13 (april)
    GreenPuppet,
    // Level 14
    YellowPuppet,
    // Level 15
    BluePuppet,
    // Level 16 Easter event
    // Trend on pink, blue, yellow, green
    // Level 17 (May)

    // Level 18
    RibbonHair, // just a ribbon that goes on head
    // Level 19 --> SAWWPPED WITH LONG HAIR --> level 5
    FlowerHair, // Hairs with a flower in it
    // Level 20 Mother's day event
    // Trend on pink puppet, FlowerHair and RibbonHair
    // Level 21 (June)

    // Level 22
    SunglassItem,
    
    CapItem, // a "Cool" cap that goes over the hair
             
             // Level 23

    // Level 24 (Party event)
    // Trend on cap and Sunglasses 
    // Level 25

    // level 26
    ShortHair, //(16 modif)
               // Level 27 
               // Level 28 Summer party event
               // Trend on cap, sunglass, short hair and flower hair
               // Level 29

    SnakeEyes,
    //3 MOVED
    NormalEyes,
    // 31

    // 32
    //33
    //34
    BlondeHair,
    //35
    Tie,
    //36
    //37
    Patch,
    //38
    NoseRing,
    //39
    ClownNose,
    None,
}

public abstract class PuppetModificationBase : MonoBehaviour
{

    public GameObject DraggableGO;
    public GameObject OnHandleGO;

    public int cost;

    public ModificationType Modification;
    public int AvailableAtLevelNumber;
    public List<Month> TrendOfTheseMonthsEvent = new List<Month>();
    [SerializeField]
    private bool _isAvailable;
    public bool IsAvailable { get { return _isAvailable; }
        protected set
        {
            _isAvailable = value;
            //Add Image related Here TODO
        }
    }

    private int amount;
    public int Amount
    {
        get
        {
            return this.amount;
        }
        set
        {
            this.amount = value;
        }
    }

    /// <summary>
    /// Must be set in awake
    /// </summary>
    public ModificationCategory Category;

    /// <summary>
    /// Must set the category of modification <see cref="Category"/>
    /// </summary>
    protected virtual void Awake()
    {    
        try
        {
            //DraggableGO;
            //OnHandleGO;
        }
        catch
        {
            Debug.LogWarning("Sprite game object uninitialized.");
        }

    }
    
    protected virtual void Start()
    {
        amount = 2;
        int i = 0;
        foreach(string s in System.Enum.GetNames(typeof(ModificationType)))
        {
            if(s != Modification.ToString() && gameObject.name.Replace(" ", "") == s)
            {
                Debug.LogWarning($"The PuppetModification {gameObject.name} was modified to fit its Modification enum with its name. Please, make the proper change to the prefab.");
                Modification = (ModificationType)i;
            }
            i++;
        }

        cost = PriceConstants.GetCost(Category);

        GameManager.levelManager.RegisterNewModification(this);
        CheckIfModificationIsAvailableAndActivateOrDesactivate();
    }

    /// <summary>
    /// Check if The modification is accessible to the current level and activate it or deactivate
    /// </summary>
    public void CheckIfModificationIsAvailableAndActivateOrDesactivate()
    {
        if (GameManager.levelManager.Level < AvailableAtLevelNumber)
        {
            IsAvailable = false;

        }
        else
        {
            IsAvailable = true;
            Debug.Log($"{name} is now Available.");
        }

       
    }

    /// <summary>
    /// Set the <see cref="InDrawerSprite"/> game object to false (visual)
    /// </summary>
    public void EmptyStock()
    {
        GetComponent<Image>().color = Color.black;
    }

    /// <summary>
    /// set the <see cref="InDrawerSprite"/> game object on (visual)
    /// </summary>
    public void AvailableStock()
    {
        GetComponent<Image>().color = Color.white;
    }

    /// <summary>
    /// Instiantiate the proper game object (visual) on top of the handle at right position
    /// 
    /// To delete these new instance, 
    ///  simply delete all the game object child in <see cref="Handle.PuppetGameObjectParent"/>. These are only visuals.
    /// </summary>
    /// <param name="handle"></param>
    public void PutOnhandle(Transform handle)
    {
        GameObject go = Instantiate(OnHandleGO, handle.Find("Puppet"));
        go.transform.SetSiblingIndex((int)Category);
        go.name = Category.ToString();

        //go.GetComponent<Identifier>().modReference = this;
        //go.GetComponentInParent<PuppetModificationBase>().transform.parent = handle;
        //Destroy(DragAndDrop.itemBeingDragged);
    }


}
