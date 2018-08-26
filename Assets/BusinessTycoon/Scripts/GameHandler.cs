using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public delegate void UpdateBalance();
    public static event UpdateBalance OnUpdateBalance;

    public static GameHandler instance;
    public double CurrentBalance{ get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (OnUpdateBalance != null)
        {
            OnUpdateBalance();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBalance(double amount)
    {
        CurrentBalance = amount;
        if (OnUpdateBalance != null)
        {
            OnUpdateBalance();
        }
    }

    public void AddToBalance(double amount)
    {
        CurrentBalance += amount;
        if (OnUpdateBalance != null)
        {
            OnUpdateBalance();
        }
    }

    public bool CanBuy(double amount)
    {
        if (amount > CurrentBalance)
        {
            return false;
        }

        return true;
    }
}
