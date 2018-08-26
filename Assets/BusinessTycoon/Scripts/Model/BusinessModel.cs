using System;

[Serializable]
public class BusinessModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageName { get; set; }
    public int BusinessCount { get; set; }
    public double BaseCost { get; set; }
    public double BaseProfit { get; set; }
    public double ProfitMultiplier { get; set; }
    public bool ManagerUnlocked { get; set; }
    public bool UpgradeUnlocked { get; set; }
    public float TimerInSeconds { get; set; }
    public float TimerCurrent { get; set; }
    public float CostMultiplier { get; set; }
    public int TimerDivision { get; set; }
}
