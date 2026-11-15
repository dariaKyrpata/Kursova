namespace Domain.Core.Models;

public enum SubscriptionTier
{
    Free = 1,
    Standard = 2,
    Premium = 3
}

public class SubscriptionPlan
{
    private string _name = string.Empty;
    private decimal _price;
    private string _maxResolution = string.Empty;

    public SubscriptionTier Tier { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Plan name cannot be empty.", nameof(value));
            }
            _name = value.Trim();
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Subscription price cannot be negative.");
            }
            _price = value;
        }
    }

    public string MaxResolution
    {
        get => _maxResolution;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Max resolution cannot be empty.", nameof(value));
            }
            _maxResolution = value.Trim();
        }
    }

    public SubscriptionPlan()
    {
    }

    public SubscriptionPlan(SubscriptionTier tier, string name, decimal price, string maxResolution)
    {
        Tier = tier;
        Name = name;
        Price = price;
        MaxResolution = maxResolution;
    }

    public override string ToString()
    {
        return $"{Name} ({Tier}) - ${Price}/mo, Max: {MaxResolution}";
    }
}