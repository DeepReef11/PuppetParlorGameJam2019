using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : PuppetMakerBase
{

    protected override void Awake()
    {
        base.Awake();
        gameObject.name = "Bubble Puppet";
        gameObject.AddComponent<RectTransform>();
    }

    /// <summary>
    /// Method to be called by Level manager on customer bubble generation
    /// </summary>
    /// <param name="puppet"></param>
    public void SetBubble(List<PuppetModificationBase> puppet)
    {
        if(puppet[(int)ModificationCategory.Eyes] ==null || puppet[(int)ModificationCategory.Dye])
        {
            Debug.LogWarning("Missing Eyes or Puppet in bubble puppet.");
            //Debug.LogError("Missing Eyes or Puppet in bubble puppet.");
        }
        _ModificationSlot = puppet;
    }

    

}
