namespace Domain.Core.Models;

public class Genre
{
    private string _name = string.Empty;
    private string _description = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Genre name cannot be empty.", nameof(value));
            }
            _name = value.Trim();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Genre description cannot be empty.", nameof(value));
            }
            _description = value.Trim();
        }
    }

    public Genre()
    {
    }

    public Genre(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public override string ToString()
    {
        return Name;
    }
}