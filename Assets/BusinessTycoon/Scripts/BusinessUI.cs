using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusinessUI : MonoBehaviour
{
    public Text StoreCountText;
    public ProgressBar ProgressBar;
    public Text BuyButtonText;
    public Text BuyButtonScaleText;
    public Button BuyButton;
    public Button BuyBusinessButton;
    public Text StoreProfitText;
    public Text TimeLeftText;

    public GameObject PurchasedPanel;
    public GameObject PurchasePanel;

    public GameObject Manager;
    public GameObject Upgrade;

    public BusinessHandler Business;

    void OnEnable()
    {
        GameHandler.OnUpdateBalance += UpdateUI;
        BusinessHandler.OnUpdateProfit += UpdateStoreProfit;
        PersistData.OnloadDataComplete += UpdateUI;
    }

    void OnDisable()
    {
        GameHandler.OnUpdateBalance -= UpdateUI;
        BusinessHandler.OnUpdateProfit -= UpdateStoreProfit;
        PersistData.OnloadDataComplete -= UpdateUI;
    }

    void Awake()
    {
        Business = transform.GetComponent<BusinessHandler>();
    }

    // Use this for initialization
    void Start()
    {
        UpdateStoreCount();
        UpdateBuyButton();
    }

    // Update is called once per frame
    void Update()
    {
        var progressValue = Business.BusinessInfo.TimerCurrent / Business.BusinessInfo.TimerInSeconds;
        if (progressValue > 0 && progressValue <= 0.010)
        {
            progressValue = 0.010f;
        }
        if (Business.BusinessInfo.TimerInSeconds <= 0.3)
        {
            ProgressBar.Progress = 1;
        }
        else
        {
            ProgressBar.Progress = progressValue;
        }

        UpdateTimeLeftText();
    }

    private void UpdateTimeLeftText()
    {
        var time = TimeSpan.FromSeconds(Business.BusinessInfo.TimerInSeconds - Business.BusinessInfo.TimerCurrent);
        TimeLeftText.text = string.Format("{0:D2}:{1:D2}:{2:D2}.{3}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds.ToString().Substring(0,1));
    }

    public void UpdateUI()
    {
        // Hide panel until you can afford the store
        if (Business.BusinessInfo.BusinessCount == 0 && GameHandler.instance.CanBuy(Business.NextStoreCost) == false)
        {
            if (BuyBusinessButton.interactable == true)
                BuyBusinessButton.interactable = false;
        }
        else
        {
            if (BuyBusinessButton.interactable == false)
                BuyBusinessButton.interactable = true;
        }

        // Disable button until you buy more stores
        if (Business.BusinessInfo.BusinessCount >= 1 && GameHandler.instance.CanBuy(Business.NextStoreCost) == true)
        {
            if (BuyButton.interactable == false)
                BuyButton.interactable = true;
        }
        else
        {
            if (BuyButton.interactable == true)
                BuyButton.interactable = false;
        }

        //Manager unlock button
        if (Business.BusinessInfo.ManagerUnlocked == false && GameHandler.instance.CanBuy(Business.Manager.Cost) == true)
        {
            var unlockManagerButton = Manager.transform.Find("UnlockButton").GetComponent<Button>();
            unlockManagerButton.interactable = true;
        }
        else
        {
            var unlockButton = Manager.transform.Find("UnlockButton").GetComponent<Button>();
            unlockButton.interactable = false;
            if (Business.BusinessInfo.ManagerUnlocked == true)
            {
                var unlockManagerButton = Manager.transform.Find("UnlockButton/Text").GetComponent<Text>();
                unlockManagerButton.text = "Hired";
            }
        }

        //Upgrade unlock button
        if (Business.BusinessInfo.UpgradeUnlocked == false && GameHandler.instance.CanBuy(Business.Upgrade.Cost) == true)
        {
            var unlockButton = Upgrade.transform.Find("UnlockButton").GetComponent<Button>();
            unlockButton.interactable = true;
        }
        else
        {
            var unlockButton = Upgrade.transform.Find("UnlockButton").GetComponent<Button>();
            unlockButton.interactable = false;
            if (Business.BusinessInfo.UpgradeUnlocked == true)
            {
                var unlockUpgradeButton = Upgrade.transform.Find("UnlockButton/Text").GetComponent<Text>();
                unlockUpgradeButton.text = "Unlocked";
            }
        }

        if (Business.BusinessInfo.BusinessCount >= 1 && PurchasePanel.activeSelf == true)
        {
            PurchasePanel.SetActive(false);
            PurchasedPanel.SetActive(true);
        }
    }

    public void BuyStoreClick()
    {
        if (GameHandler.instance.CanBuy(Business.NextStoreCost) == true)
        {
            if (Business.BusinessInfo.BusinessCount >= 1 && PurchasePanel.activeSelf == true)
            {
                PurchasePanel.SetActive(false);
                PurchasedPanel.SetActive(true);
            }
            Business.BuyStore();
            UpdateStoreCount();
            UpdateBuyButton();
            UpdateStoreProfit();
        }
    }

    private void UpdateStoreProfit()
    {
        StoreProfitText.text = MoneyFormat.Default(((Business.BusinessInfo.BaseProfit * Business.BusinessInfo.BusinessCount)* Business.BusinessInfo.ProfitMultiplier));
    }

    private void UpdateStoreCount()
    {
        StoreCountText.text = Business.BusinessInfo.BusinessCount.ToString();
    }

    private void UpdateBuyButton()
    {
        var money = MoneyFormat.GetMoney(Business.NextStoreCost);
        BuyButtonText.text = "$ " + money.FormattedNumber;
        BuyButtonScaleText.text = money.Scale;
    }

    public void StartTimerClick()
    {
        Business.StartTimer();
    }
}
