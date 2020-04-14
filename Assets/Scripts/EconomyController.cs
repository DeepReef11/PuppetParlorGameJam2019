using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyController : MonoBehaviour
{
    [SerializeField]
    private int coins;
    public static EconomyController instance;
    private UIManager _UIManager;
    private AudioManager _audioManager;

    private AudioSource BuyAudio;
    private AudioSource InvalidAudio;

    private void Start()
    {
        instance = this;
        _UIManager = FindObjectOfType<UIManager>();
        _audioManager = FindObjectOfType<AudioManager>();

        instance._UIManager.UpdateCoins(instance.coins);

    }

    public static int GetCoins()
    {
        return instance.coins;
    }


    public static void Addcoins(int value)
    {
        instance.coins += value;
        instance._UIManager.UpdateCoins(instance.coins);
    }

    public void SubstractCoins(PuppetModificationBase mod, bool isQuickBuy = false)
    {
        var value = isQuickBuy ? mod.cost * 2 : mod.cost;
        
        if (instance.Enough(value))
        {
            instance.coins -= value;
            mod.Amount++;
            instance._UIManager.UpdateCoins(instance.coins);
            instance._audioManager.Play("Buying");
        }
        else
        {
            instance._audioManager.Play("Invalid");
            Debug.Log("Not Enough Coins");
        }
    }

    public void SubstractCoins(int value)
    {
        if (instance.Enough(value))
        {
            instance.coins -= value;
            instance._UIManager.UpdateCoins(instance.coins);
            instance._audioManager.Play("Buying");
        }
        else
        {
            instance._audioManager.Play("Invalid");
            Debug.Log("Not Enough Coins");
        }
    }

    private bool Enough(int value)
    {
        return coins >= value;
    }
}
