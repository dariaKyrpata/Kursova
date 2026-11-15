namespace Domain.Core.Models;

public class UserReview
{
    private string _author = string.Empty;
    private int _rating;
    private string _comment = string.Empty;

    public string Author
    {
        get => _author;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Author name cannot be empty.", nameof(value));
            }
            _author = value.Trim();
        }
    }

    public int Rating
    {
        get => _rating;
        set
        {
            if (value < 1 || value > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 1 and 10.");
            }
            _rating = value;
        }
    }

    public string Comment
    {
        get => _comment;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Review comment cannot be empty.", nameof(value));
            }
            _comment = value.Trim();
        }
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserReview()
    {
    }

    public UserReview(string author, int rating, string comment)
    {
        Author = author;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"[{Rating}/10] {Author}: \"{Comment}\" ({CreatedAt:yyyy-MM-dd})";
    }
}