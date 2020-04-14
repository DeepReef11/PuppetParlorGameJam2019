using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerReference : MonoBehaviour
{
    public PuppetModificationBase modReference;

    public bool isQuickBuy;

    public void Substract()
    {
        EconomyController.instance.SubstractCoins(modReference, isQuickBuy);
    }
}
