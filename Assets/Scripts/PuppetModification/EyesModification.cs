using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesModification : PuppetModificationBase
{
    protected override void Awake()
    {
        Category = ModificationCategory.Eyes;
        base.Awake();
        
    }
}
