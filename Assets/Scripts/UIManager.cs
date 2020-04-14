using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("QuickBuy Txt")]
    public Text puppetCostTxt;
    public Text eyesCostTxt;
    public Text hairCostTxt;
    public Text accessoryCostTxt;

    [Header("Restock Txt")]
    public Text puppetCostTxtR;
    public Text eyesCostTxtR;
    public Text hairCostTxtR;
    public Text accessoryCostTxtR;

    [Header ("Panels")]
    public GameObject pausePanel;
    public GameObject restockPanel;

    [Header ("InfoTxt")]
    public Text monthTxt;
    public Text coinsTxt;
    public Text weekTxt;
    public Text goalTxt;

    public Sprite noTrendSprite;
    public Image trendingImage;

    // Start is called before the first frame update
    void Start()
    {
        puppetCostTxt.text = "$" + (PriceConstants.DyeCost * 2).ToString();
        eyesCostTxt.text = "$" + (PriceConstants.EyesCost * 2).ToString();
        hairCostTxt.text = "$" + (PriceConstants.HairCost * 2).ToString();
        accessoryCostTxt.text = "$" + (PriceConstants.AccessoryCost * 2).ToString();

        puppetCostTxtR.text = "$" + PriceConstants.DyeCost.ToString();
        eyesCostTxtR.text = "$" + PriceConstants.EyesCost.ToString();
        hairCostTxtR.text = "$" + PriceConstants.HairCost.ToString();
        accessoryCostTxtR.text = "$" + PriceConstants.AccessoryCost.ToString();
    }

    public void UpdateCoins(int value)
    {
        coinsTxt.text = value.ToString();
    }

    public void UpdatePause(bool isPaused)
    {
        if (isPaused)
            pausePanel.SetActive(true);

        pausePanel.GetComponent<Animator>().SetTrigger("Trigger");
    }

    public void UpdateMonth(Month month)
    {
        monthTxt.text = month.ToString();
    }

    public void UpdateGoal(int value)
    {
        goalTxt.text = "Month's Goal: " + value;
    }

    public void UpdateWeek(int week)
    {
        weekTxt.text = "Week " + week;
    }

    public void SetTrending(PuppetModificationBase mod)
    {
        if (mod)
            trendingImage.sprite = mod.OnHandleGO.GetComponent<Image>().sprite;
        else
            trendingImage.sprite = noTrendSprite;
    }

    public void UpdateRestock(bool active)
    {
        if (active)
            restockPanel.SetActive(true);

        restockPanel.GetComponent<Animator>().SetTrigger("Trigger");
    }
}
