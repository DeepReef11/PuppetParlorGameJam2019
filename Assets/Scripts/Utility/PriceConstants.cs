using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceConstants : MonoBehaviour
{
    public const int DyeCost = 20;
    public const int AccessoryCost = 15;
    public const int HairCost = 10;
    public const int EyesCost = 10;

    public static int GetCost(ModificationCategory cat)
    {
        var cost = 0; 

        switch ((int)cat)
        {
            case 0:
                cost = DyeCost;
                break;
            case 1:
                cost = HairCost;
                break;
            case 2:
                cost = EyesCost;
                break;
            case 3:
                cost = AccessoryCost;
                break;
            default:
                break;
        }

        return cost;
    }
}
