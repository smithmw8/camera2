using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    public enum State
    {
        Main,
        ManagerPanel,
        UpgradePanel,
        WelcomeBackPanel
    }

    public Text CurrentBalanceText;
    public State CurrentState;

    public GameObject DialogPanels;

    void Awake()
    {
        // Singelton - There can be only one...
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameHandler.OnUpdateBalance += UpdateUI;
        PersistData.OnloadDataComplete += UpdateUI;
    }

    void OnDisable()
    {
        GameHandler.OnUpdateBalance -= UpdateUI;
        PersistData.OnloadDataComplete -= UpdateUI;
    }

    public void DialogClose()
    {
        CurrentState = State.Main;
        ShowDialog();
    }

    public void ManagerButtonClick()
    {
        CurrentState = State.ManagerPanel;
        ShowDialog();
    }

    public void UpgradeButtonClick()
    {
        CurrentState = State.UpgradePanel;
        ShowDialog();
    }

    public void WelcomeBack(double diffInSeconds, double idleAmount)
    {
        var time = TimeSpan.FromSeconds(diffInSeconds);
        var timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);

        var offlineText = DialogPanels.transform.Find("WelcomeBackPanel/Text").GetComponent<Text>();
        offlineText.text = "You earned\n" + MoneyFormat.Default(idleAmount) + "\nwhile you were offline for\n" + timeText;

        CurrentState = State.WelcomeBackPanel;
        ShowDialog();
    }

    private void ShowDialog()
    {
        foreach (Transform eachChild in DialogPanels.transform)
        {
            eachChild.gameObject.SetActive(false);
        }

        var overlayImage = DialogPanels.transform.Find("OverlayImage").GetComponent<Image>();
        overlayImage.transform.gameObject.SetActive(false);

        foreach (Transform eachChild in DialogPanels.transform)
        {
            if (eachChild.name == CurrentState.ToString())
            {

                eachChild.gameObject.SetActive(true);
                overlayImage.transform.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateUI()
    {
        CurrentBalanceText.text = MoneyFormat.Default(GameHandler.instance.CurrentBalance);
    }

    // Use this for initialization
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
