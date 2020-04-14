using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryController
{
    public static void AModificationGoesBackToTheDrawer(PuppetModificationBase mod, int value = 1)
    {
        mod.Amount += value;
    }

    public static void AModificationHasBeenTakenFromDrawer(PuppetModificationBase mod, int value = 1)
    {
        if (mod.Amount > 0)
        {
            mod.Amount -= value;
        }
        else
        {
            Debug.LogError($"(Negative amount of {mod.gameObject.name}) InventoryController isn't in control.");
        }
    }

    public static bool Enough(PuppetModificationBase mod, int value = 1)
    {
        return mod.Amount >= value;
    }
}
