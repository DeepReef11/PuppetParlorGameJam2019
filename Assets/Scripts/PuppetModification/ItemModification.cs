using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModification : PuppetModificationBase
{
    protected override void Awake()
    {
        Category = ModificationCategory.Accessory;
        base.Awake();
        
    }

}
