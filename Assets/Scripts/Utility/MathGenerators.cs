using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// MAthematic generators for difficulty increasement
/// </summary>
public static class Gen
{
    public static int GenerateNumberOfCustomersInLevel(int lvl)
    {
        return Mathf.CeilToInt((Mathf.Pow(lvl, .8f))) + 2;
    }
}

