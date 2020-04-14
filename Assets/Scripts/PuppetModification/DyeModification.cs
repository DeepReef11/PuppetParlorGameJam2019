using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyeModification : PuppetModificationBase
{
    protected override void Awake()
    {
        Category = ModificationCategory.Dye;
        base.Awake();
        
    }

}
