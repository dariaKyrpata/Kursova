namespace Domain.Core.Models;

public class Movie : IComparable<Movie>
{
    private string _title = string.Empty;
    private string _director = string.Empty;
    private int _releaseYear;
    private int _durationMinutes;
    private int _minAge;
    private Genre _genre = new();
    private List<UserReview> _reviews = new();

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Movie title cannot be empty.", nameof(value));
            }
            _title = value.Trim();
        }
    }

    public string Director
    {
        get => _director;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Director name cannot be empty.", nameof(value));
            }
            _director = value.Trim();
        }
    }

    public int ReleaseYear
    {
        get => _releaseYear;
        set
        {
            if (value < 1895 || value > DateTime.UtcNow.Year + 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Release year must be between 1895 and {DateTime.UtcNow.Year + 2}.");
            }
            _releaseYear = value;
        }
    }

    public int DurationMinutes
    {
        get => _durationMinutes;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Duration must be greater than 0 minutes.");
            }
            _durationMinutes = value;
        }
    }

    public int MinAge
    {
        get => _minAge;
        set
        {
            if (value < 0 || value > 21)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Minimum age restriction must be between 0 and 21.");
            }
            _minAge = value;
        }
    }

    public Genre Genre
    {
        get => _genre;
        set => _genre = value ?? throw new ArgumentNullException(nameof(value), "Genre cannot be null.");
    }

    public SubscriptionTier RequiredPlan { get; set; } = SubscriptionTier.Free;

    public List<UserReview> Reviews
    {
        get => _reviews;
        set => _reviews = value ?? new List<UserReview>();
    }

    public Movie()
    {
    }

    public Movie(
        string title,
        string director,
        int releaseYear,
        int durationMinutes,
        int minAge,
        Genre genre,
        SubscriptionTier requiredPlan = SubscriptionTier.Free)
    {
        Id = Guid.NewGuid();
        Title = title;
        Director = director;
        ReleaseYear = releaseYear;
        DurationMinutes = durationMinutes;
        MinAge = minAge;
        Genre = genre;
        RequiredPlan = requiredPlan;
        Reviews = new List<UserReview>();
    }

    public void AddReview(UserReview review)
    {
        ArgumentNullException.ThrowIfNull(review);
        _reviews.Add(review);
    }

    public double CalculateAverageRating()
    {
        if (_reviews.Count == 0)
        {
            return 0.0;
        }

        return Math.Round(_reviews.Average(r => r.Rating), 1);
    }

    public int CompareTo(Movie? other)
    {
        if (other is null)
        {
            return 1;
        }

        return string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        double rating = CalculateAverageRating();
        string ratingText = rating > 0 ? $"{rating:F1}/10 ({Reviews.Count} reviews)" : "No reviews";
        return $"\"{Title}\" ({ReleaseYear}) | Genre: {Genre.Name} | {DurationMinutes} min | Age: {MinAge}+ | Access: {RequiredPlan} | Rating: {ratingText}";
    }
}