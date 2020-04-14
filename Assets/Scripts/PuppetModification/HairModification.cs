using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairModification : PuppetModificationBase
{
    override protected void Awake()
    {
        Category = ModificationCategory.Hair;
        base.Awake();
       
    }
}
