using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BusinessHandler : MonoBehaviour
{
    public delegate void UpdateProfit();
    public static event UpdateProfit OnUpdateProfit;

    public BusinessModel BusinessInfo;
    public double NextStoreCost
    {
        get
        {
            return BusinessInfo.BaseCost * Mathf.Pow(BusinessInfo.CostMultiplier, BusinessInfo.BusinessCount);
        }
    }

    public ManagerModel Manager
    {
        get
        {
            var manager = PersistData.instance.GameData.Managers.FirstOrDefault(t => t.BusinessId == BusinessInfo.Id);
            if (manager != null)
            {
                return manager;
            }
            else
            {
                return new ManagerModel();
            }
        }
    }

    public UpgradeModel Upgrade
    {
        get
        {
            var upgrade = PersistData.instance.GameData.Upgrades.FirstOrDefault(t => t.BusinessId == BusinessInfo.Id);
            if (upgrade != null)
            {
                return upgrade;
            }
            else
            {
                return new UpgradeModel();
            }
        }
    }

    private bool _startTimer;

    public BusinessHandler()
    {
        BusinessInfo = new BusinessModel();
    }

    // Use this for initialization
    private void Start()
    {
        _startTimer = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_startTimer == true)
        {
            BusinessInfo.TimerCurrent += Time.deltaTime;
            if (BusinessInfo.TimerCurrent >= BusinessInfo.TimerInSeconds)
            {
                if (BusinessInfo.ManagerUnlocked == false)
                {
                    _startTimer = false;
                }
                BusinessInfo.TimerCurrent = 0;
                GameHandler.instance.AddToBalance((BusinessInfo.BaseProfit * BusinessInfo.BusinessCount) * BusinessInfo.ProfitMultiplier);
            }
        }
        else
        {
            if (BusinessInfo.BusinessCount > 0)
            {
                //If unlocked or loaded with started timer
                if (BusinessInfo.ManagerUnlocked == true || BusinessInfo.TimerCurrent > 0)
                {
                    _startTimer = true;
                }
            }
        }
    }

    public void StartTimer()
    {
        if (_startTimer == false && BusinessInfo.BusinessCount > 0)
        {
            _startTimer = true;
        }
    }

    public void UnlockManager()
    {
        if (BusinessInfo.ManagerUnlocked == false)
        {
            var amount = -(Manager.Cost);
            BusinessInfo.ManagerUnlocked = true;
            GameHandler.instance.AddToBalance(amount);
            StartTimer();
        }
    }

    public void UnlockUpgrade()
    {
        if (BusinessInfo.UpgradeUnlocked == false)
        {
            var amount = -(Upgrade.Cost);
            BusinessInfo.UpgradeUnlocked = true;
            BusinessInfo.ProfitMultiplier = BusinessInfo.ProfitMultiplier * 3;
            GameHandler.instance.AddToBalance(amount);
            if (OnUpdateProfit != null)
            {
                OnUpdateProfit();
            }
        }
    }

    public void BuyStore()
    {
        var amount = -this.NextStoreCost;

        BusinessInfo.BusinessCount++;

        GameHandler.instance.AddToBalance(amount);

        if (BusinessInfo.BusinessCount % BusinessInfo.TimerDivision == 0)
        {
            BusinessInfo.TimerInSeconds = BusinessInfo.TimerInSeconds / 2;
            if (BusinessInfo.TimerInSeconds <= 0.25)
            {
                BusinessInfo.TimerInSeconds = 0.25f;
            }

            if (BusinessInfo.TimerCurrent > 0)
            {
                BusinessInfo.TimerCurrent = BusinessInfo.TimerCurrent / 2;
            }
        }
    }
}
