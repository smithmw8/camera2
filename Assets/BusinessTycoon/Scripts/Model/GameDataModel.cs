using System;
using System.Collections.Generic;


[Serializable]
public class GameDataModel
{
    public string WorldName { get; set; }
    public string CurrencyName { get; set; }
    public double CurrentBalance { get; set; }
    public double TotalBalance { get; set; }
    public DateTime TimeStamp { get; set; }
    public GameSettingModel GameSettings { get; set; }
    public List<BusinessModel> Businesses { get; set; }
    public List<ManagerModel> Managers { get; set; }
    public List<UpgradeModel> Upgrades { get; set; }
}
