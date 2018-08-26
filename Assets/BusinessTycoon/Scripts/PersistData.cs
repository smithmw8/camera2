using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PersistData : MonoBehaviour
{
    private string GameFilePath;
    private string GameFileName;
    private bool gamePaused;

    public delegate void LoadDataComplete();

    public static event LoadDataComplete OnloadDataComplete;

    public static PersistData instance;

    public GameDataModel GameData;

    public GameObject BusinessPrefab;
    public GameObject BusinessPanel;

    public GameObject ManagerPanel;
    public GameObject ManagerPrefab;

    public GameObject UpgradePanel;
    public GameObject UpgradePrefab;

    // Use this for initialization
    void Awake()
    {
        GameFilePath = Application.persistentDataPath + "/userData";
        GameFileName = "/default.dat";

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

    void Start()
    {

    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (gamePaused && focusStatus)
        {
            gamePaused = false;
            OnEnable();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (gamePaused == false && pauseStatus)
        {
            gamePaused = true;
            OnDisable();
        }
    }

    void OnDisable()
    {
        instance.Save();
    }

    void OnEnable()
    {
            instance.Load();
    }

    void OnGUI()
    {
        
    }

    public void Save()
    {
        CollectDataToSave();

        if (!Directory.Exists(GameFilePath))
        {
            Directory.CreateDirectory(GameFilePath);
        }

        var bf = new BinaryFormatter();
        var file = File.Create(GameFilePath + "/" + GameFileName);

        bf.Serialize(file, this.GameData);
        file.Close();
    }

    private void CollectDataToSave()
    {
        GameData.TimeStamp = DateTime.Now;
        GameData.CurrentBalance = GameHandler.instance.CurrentBalance;
    }

    public void Load()
    {
        GameData = new GameDataModel();

        if (File.Exists(GameFilePath + "/" + GameFileName))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(GameFilePath + "/" + GameFileName, FileMode.Open);
            var gameData = (GameDataModel)bf.Deserialize(file);
            file.Close();

            if (string.IsNullOrEmpty(gameData.WorldName))
            {
                CreateDefaultGameData();
            }
            else
            {
                GameData = gameData;
            }
        }
        else
        {
            CreateDefaultGameData();
        }

        var diffInSeconds = (DateTime.Now - GameData.TimeStamp).TotalSeconds;

        var idleAmount = LoadBusinesses(diffInSeconds);

        UpdateGameData(idleAmount);

        if (diffInSeconds >= 180)
        {
            // Show Idle Profit Dialog
            GameUI.instance.WelcomeBack(diffInSeconds, idleAmount);
        }

        if (OnloadDataComplete != null)
        {
            OnloadDataComplete();
        }
    }

    private double LoadBusinesses(double idleTime)
    {
        var idleAmount = 0d;

        foreach (Transform eachChild in BusinessPanel.transform)
        {
            if (eachChild.name == "BusinessPrefab")
            {
                Destroy(eachChild.gameObject);
            }
        }

        foreach (Transform eachChild in ManagerPanel.transform)
        {
            if (eachChild.name == "ManagerPrefab")
            {
                Destroy(eachChild.gameObject);
            }
        }

        foreach (Transform eachChild in UpgradePanel.transform)
        {
            if (eachChild.name == "UpgradePrefab")
            {
                Destroy(eachChild.gameObject);
            }
        }

        foreach (var element in GameData.Businesses)
        {
            var newBusiness = Instantiate(BusinessPrefab);
            newBusiness.name = "BusinessPrefab";
            var currentBusiness = newBusiness.GetComponent<BusinessHandler>();

            if (element.TimerCurrent > 0 || element.ManagerUnlocked == true)
            {
                if (element.TimerCurrent + idleTime >= element.TimerInSeconds)
                {
                    var businessStoreProfit = ((element.BaseProfit * element.BusinessCount) * element.ProfitMultiplier) * element.CostMultiplier;
                    if (element.ManagerUnlocked)
                    {
                        idleAmount = businessStoreProfit * (idleTime / element.TimerInSeconds);
                        element.TimerCurrent = (float)(idleTime % (element.TimerInSeconds - element.TimerCurrent)) + element.TimerCurrent;

                    }
                    else
                    {
                        idleAmount = businessStoreProfit;
                        element.TimerCurrent = 0;
                    }
                }
                else
                {
                    var result = (float)idleTime;
                    if (float.IsPositiveInfinity(result))
                    {
                        result = float.MaxValue;
                    }

                    element.TimerCurrent = (element.TimerCurrent + result);
                }
            }

            currentBusiness.BusinessInfo = element;

            LoadManagers(currentBusiness);
            LoadUpgrades(currentBusiness);

            var nameText = newBusiness.transform.Find("PurchasedPanel/StoreNameText").GetComponent<Text>();
            nameText.text = element.Name;

            nameText = newBusiness.transform.Find("PurchasePanel/BuyBusinessButton/StoreNameText").GetComponent<Text>();
            nameText.text = "Unlock " + "\"" + element.Name + "\"";

            var newSprite = Resources.Load<Sprite>(element.ImageName);
            var image = newBusiness.transform.Find("PurchasedPanel/StoreBackImageButton/StoreImage").GetComponent<Image>();
            image.sprite = newSprite;

            var storeCostText = newBusiness.transform.Find("PurchasePanel/BuyBusinessButton/StoreCostText").GetComponent<Text>();
            storeCostText.text = MoneyFormat.Default((element.BaseCost * Mathf.Pow(element.CostMultiplier, element.BusinessCount)));

            var nextProfitText = currentBusiness.transform.Find("PurchasedPanel/NextProfitText").GetComponent<Text>();
            nextProfitText.text = MoneyFormat.Default((currentBusiness.BusinessInfo.BaseProfit * currentBusiness.BusinessInfo.BusinessCount));

            newBusiness.transform.SetParent(BusinessPanel.transform, false);
        }

        return idleAmount;
    }

    private void LoadManagers(BusinessHandler business)
    {
        foreach (var manager in GameData.Managers.Where(t => t.BusinessId == business.BusinessInfo.Id))
        {
            var newManager = Instantiate(ManagerPrefab);
            newManager.name = "ManagerPrefab";
            var nameText = newManager.transform.Find("NameText").GetComponent<Text>();
            nameText.text = manager.Name;

            var descriptionText = newManager.transform.Find("DescriptionText").GetComponent<Text>();
            descriptionText.text = manager.Description.Replace("{0}", business.BusinessInfo.Name);

            var newSprite = Resources.Load<Sprite>(manager.ImageName);
            var image = newManager.transform.Find("Image").GetComponent<Image>();
            image.sprite = newSprite;

            var costText = newManager.transform.Find("CostText").GetComponent<Text>();
            costText.text = MoneyFormat.Default(manager.Cost);

            newManager.transform.SetParent(ManagerPanel.transform, false);

            Button unlockManagerButton = newManager.transform.Find("UnlockButton").GetComponent<Button>();
            unlockManagerButton.onClick.AddListener(business.UnlockManager);

            BusinessUI businessUI = business.GetComponent<BusinessUI>();
            businessUI.Manager = newManager;
        }
    }

    private void LoadUpgrades(BusinessHandler business)
    {
        foreach (var upgrade in GameData.Upgrades.Where(t => t.BusinessId == business.BusinessInfo.Id))
        {
            var newUpgrade = Instantiate(UpgradePrefab);
            newUpgrade.name = "UpgradePrefab";
            var nameText = newUpgrade.transform.Find("NameText").GetComponent<Text>();
            nameText.text = upgrade.Name;

            var descriptionText = newUpgrade.transform.Find("DescriptionText").GetComponent<Text>();
            descriptionText.text = upgrade.Description.Replace("{0}", business.BusinessInfo.Name);

            var newSprite = Resources.Load<Sprite>(upgrade.ImageName);
            var image = newUpgrade.transform.Find("Image").GetComponent<Image>();
            image.sprite = newSprite;

            var costText = newUpgrade.transform.Find("CostText").GetComponent<Text>();
            costText.text = MoneyFormat.Default(upgrade.Cost);

            newUpgrade.transform.SetParent(UpgradePanel.transform, false);

            Button unlockUpgradeButton = newUpgrade.transform.Find("UnlockButton").GetComponent<Button>();
            unlockUpgradeButton.onClick.AddListener(business.UnlockUpgrade);

            BusinessUI businessUI = business.GetComponent<BusinessUI>();
            businessUI.Upgrade = newUpgrade;
        }
    }

    private void UpdateGameData(double idleAmount)
    {
        GameHandler.instance.SetBalance(GameData.CurrentBalance);
        GameHandler.instance.AddToBalance(idleAmount);
    }

    private void CreateDefaultGameData()
    {
        GameData = new GameDataModel
        {
            WorldName = "Default",
            CurrencyName = "Dollars",
            CurrentBalance = 3,
            TotalBalance = 0,
            TimeStamp = DateTime.Now,
            GameSettings = new GameSettingModel()
            {
                Music = true,
                SoundFx = true,
                Notifications = true
            },
            Businesses = new List<BusinessModel>()
            {
                new BusinessModel
                {
                    Id = 1,
                    Name = "Paint shop",
                    ImageName = "Paint",
                    BusinessCount = 1,
                    BaseCost = 4,
                    BaseProfit = 1,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 2,
                    TimerCurrent = 0,
                    CostMultiplier = 1.07f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 2,
                    Name = "Lego shop",
                    ImageName = "Lego",
                    BusinessCount = 0,
                    BaseCost = 60,
                    BaseProfit = 60,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 3,
                    TimerCurrent = 0,
                    CostMultiplier = 1.15f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 3,
                    Name = "Pet shop",
                    ImageName = "Pet",
                    BusinessCount = 0,
                    BaseCost = 720,
                    BaseProfit = 540,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 6,
                    TimerCurrent = 0,
                    CostMultiplier = 1.14f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 4,
                    Name = "Toy shop",
                    ImageName = "Toy",
                    BusinessCount = 0,
                    BaseCost = 8640,
                    BaseProfit = 4320,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 12,
                    TimerCurrent = 0,
                    CostMultiplier = 1.13f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 5,
                    Name = "Phone shop",
                    ImageName = "Phone",
                    BusinessCount = 0,
                    BaseCost = 103680,
                    BaseProfit = 51840,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 24,
                    TimerCurrent = 0,
                    CostMultiplier = 1.12f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 6,
                    Name = "Candy shop",
                    ImageName = "Candy",
                    BusinessCount = 0,
                    BaseCost = 1244160,
                    BaseProfit = 622080,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 96,
                    TimerCurrent = 0,
                    CostMultiplier = 1.11f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 7,
                    Name = "Sport shop",
                    ImageName = "Sport",
                    BusinessCount = 0,
                    BaseCost = 14929920,
                    BaseProfit = 7464000,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 384,
                    TimerCurrent = 0,
                    CostMultiplier = 1.10f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 8,
                    Name = "Dress shop",
                    ImageName = "Dress",
                    BusinessCount = 0,
                    BaseCost = 179159040,
                    BaseProfit = 89579000,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 1525,
                    TimerCurrent = 0,
                    CostMultiplier = 1.09f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 9,
                    Name = "Costume shop",
                    ImageName = "Costume",
                    BusinessCount = 0,
                    BaseCost = 2149908480,
                    BaseProfit = 1074000000,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 6144,
                    TimerCurrent = 0,
                    CostMultiplier = 1.08f,
                    TimerDivision = 25
                },
                new BusinessModel
                {
                    Id = 10,
                    Name = "Computer shop",
                    ImageName = "Computer",
                    BusinessCount = 0,
                    BaseCost = 25798901760,
                    BaseProfit = 29668000000,
                    ProfitMultiplier = 1,
                    ManagerUnlocked = false,
                    UpgradeUnlocked = false,
                    TimerInSeconds = 36840,
                    TimerCurrent = 0,
                    CostMultiplier = 1.07f,
                    TimerDivision = 25
                }
            },
            Managers = new List<ManagerModel>()
            {
                new ManagerModel
                {
                    ManagerId = 1,
                    BusinessId = 1,
                    Name = "Billy",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Billy",
                    Cost = 1000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 2,
                    BusinessId = 2,
                    Name = "Mike",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Mike",
                    Cost = 15000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 3,
                    BusinessId = 3,
                    Name = "Kelly",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Kelly",
                    Cost = 100000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 4,
                    BusinessId = 4,
                    Name = "Wendy",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Wendy",
                    Cost = 500000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 5,
                    BusinessId = 5,
                    Name = "Sean",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Sean",
                    Cost = 1200000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 6,
                    BusinessId = 6,
                    Name = "Sara",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Sara",
                    Cost = 10000000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 7,
                    BusinessId = 7,
                    Name = "Carlos",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Carlos",
                    Cost = 111111111,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 8,
                    BusinessId = 8,
                    Name = "Lara",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Lara",
                    Cost = 555555555,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 9,
                    BusinessId = 9,
                    Name = "Spike",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Spike",
                    Cost = 10000000000,
                    Unlocked = false
                },
                new ManagerModel
                {
                    ManagerId = 10,
                    BusinessId = 10,
                    Name = "Floyd",
                    Description = "Hire manager to run your {0} for you.",
                    ImageName = "Floyd",
                    Cost = 100000000000,
                    Unlocked = false
                },
            },
            Upgrades = new List<UpgradeModel>()
            {
                new UpgradeModel()
                {
                    UpgradeId = 1,
                    BusinessId = 1,
                    Name = "Paint shop",
                    Description = "{0} profit x3.",
                    ImageName = "Paint",
                    Cost = 250000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 2,
                    BusinessId = 2,
                    Name = "Lego shop",
                    Description = "{0} profit x3.",
                    ImageName = "Lego",
                    Cost = 500000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 3,
                    BusinessId = 3,
                    Name = "Pet shop",
                    Description = "{0} profit x3.",
                    ImageName = "Pet",
                    Cost = 1000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 4,
                    BusinessId = 4,
                    Name = "Toy shop",
                    Description = "{0} profit x3.",
                    ImageName = "Toy",
                    Cost = 5000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 5,
                    BusinessId = 5,
                    Name = "Phone shop",
                    Description = "{0} profit x3.",
                    ImageName = "Phone",
                    Cost = 10000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 6,
                    BusinessId = 6,
                    Name = "Candy shop",
                    Description = "{0} profit x3.",
                    ImageName = "Candy",
                    Cost = 25000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 7,
                    BusinessId = 7,
                    Name = "Sport shop",
                    Description = "{0} profit x3.",
                    ImageName = "Sport",
                    Cost = 500000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 8,
                    BusinessId = 8,
                    Name = "Dress shop",
                    Description = "{0} profit x3.",
                    ImageName = "Dress",
                    Cost = 1000000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 9,
                    BusinessId = 9,
                    Name = "Costume shop",
                    Description = "{0} profit x3.",
                    ImageName = "Costume",
                    Cost = 50000000000,
                    Unlocked = false
                },
                new UpgradeModel()
                {
                    UpgradeId = 10,
                    BusinessId = 10,
                    Name = "Computer shop",
                    Description = "{0} profit x3.",
                    ImageName = "Computer",
                    Cost = 250000000000,
                    Unlocked = false
                }
            }
        };
    }
}