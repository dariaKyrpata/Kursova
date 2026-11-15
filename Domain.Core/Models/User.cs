namespace Domain.Core.Models;

public class User
{
    private string _username = string.Empty;
    private int _age;

    public string Username
    {
        get => _username;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Username cannot be empty.", nameof(value));
            }
            _username = value.Trim();
        }
    }

    public int Age
    {
        get => _age;
        set
        {
            if (value < 0 || value > 130)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 0 and 130.");
            }
            _age = value;
        }
    }

    public SubscriptionTier Subscription { get; set; } = SubscriptionTier.Free;

    public User()
    {
    }

    public User(string username, int age, SubscriptionTier subscription = SubscriptionTier.Free)
    {
        Username = username;
        Age = age;
        Subscription = subscription;
    }

    public override string ToString()
    {
        return $"{Username} (Age: {Age}, Plan: {Subscription})";
    }
}