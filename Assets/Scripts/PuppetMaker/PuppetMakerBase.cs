using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuppetMakerBase : MonoBehaviour
{
    /// <summary>
    /// Contains one <see cref="PuppetModificationBase"/> for each <see cref="ModificationCategory"/>
    /// </summary>
    protected List<PuppetModificationBase> _ModificationSlot;

    protected virtual void Awake()
    {
        InitializeModList();
    }

    public void InitializeModList()
    {
        _ModificationSlot = new List<PuppetModificationBase>();
        for (int i = 0; i < System.Enum.GetNames(typeof(ModificationCategory)).Length; i++)
        {
            _ModificationSlot.Add(null);
        }
    }

    public virtual List<PuppetModificationBase> GetPuppet()
    {
        return _ModificationSlot;
    }
}
