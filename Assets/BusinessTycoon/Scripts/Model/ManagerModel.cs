using System;

[Serializable]
public class ManagerModel
{
    public int ManagerId { get; set; }
    public int BusinessId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Cost { get; set; }
    public bool Unlocked { get; set; }
    public string ImageName { get; set; }
}
