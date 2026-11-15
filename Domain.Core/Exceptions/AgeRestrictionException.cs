namespace Domain.Core.Exceptions;

public class AgeRestrictionException : Exception
{
    private int _userAge;
    private int _requiredAge;

    public int UserAge
    {
        get => _userAge;
        private set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "User age cannot be negative.");
            }
            _userAge = value;
        }
    }

    public int RequiredAge
    {
        get => _requiredAge;
        private set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Required age restriction cannot be negative.");
            }
            _requiredAge = value;
        }
    }

    public AgeRestrictionException(int userAge, int requiredAge)
        : base($"Access denied due to age restriction! User age: {userAge}, required age: {requiredAge}+.")
    {
        UserAge = userAge;
        RequiredAge = requiredAge;
    }

    public AgeRestrictionException(string message, int userAge, int requiredAge)
        : base(message)
    {
        UserAge = userAge;
        RequiredAge = requiredAge;
    }
}