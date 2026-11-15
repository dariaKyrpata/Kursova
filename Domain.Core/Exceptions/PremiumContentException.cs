namespace Domain.Core.Exceptions;

public class PremiumContentException : Exception
{
    private string _currentPlan = string.Empty;
    private string _requiredPlan = string.Empty;

    public string CurrentPlan
    {
        get => _currentPlan;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Current subscription plan cannot be empty.", nameof(value));
            }
            _currentPlan = value;
        }
    }

    public string RequiredPlan
    {
        get => _requiredPlan;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Required subscription plan cannot be empty.", nameof(value));
            }
            _requiredPlan = value;
        }
    }

    public PremiumContentException(string currentPlan, string requiredPlan)
        : base($"Access denied! This content requires '{requiredPlan}' plan, but current plan is '{currentPlan}'.")
    {
        CurrentPlan = currentPlan;
        RequiredPlan = requiredPlan;
    }

    public PremiumContentException(string message, string currentPlan, string requiredPlan)
        : base(message)
    {
        CurrentPlan = currentPlan;
        RequiredPlan = requiredPlan;
    }
}