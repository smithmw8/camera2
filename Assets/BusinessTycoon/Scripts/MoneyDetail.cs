public class MoneyDetail
{
    public double Number { get; private set; }
    public string FormattedNumber { get; private set; }
    public string Scale { get; private set; }

    public MoneyDetail(double number, string formattednumber, string scale)
    {
        this.Number = number;
        this.FormattedNumber = formattednumber;
        this.Scale = scale;
    }
}