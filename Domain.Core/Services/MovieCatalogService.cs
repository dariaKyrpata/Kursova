using Domain.Core.Comparers;
using Domain.Core.Exceptions;
using Domain.Core.Interfaces;
using Domain.Core.Models;

namespace Domain.Core.Services;

public class MovieCatalogService : IMovieCatalogService
{
    private readonly IRepository<Movie> _repository;
    private readonly JsonFileService<Movie> _fileService;

    public MovieCatalogService(IRepository<Movie> repository, JsonFileService<Movie> fileService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }

    public IReadOnlyList<Movie> GetAllMovies()
    {
        return _repository.GetAll();
    }

    public void AddMovie(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        bool exists = _repository.GetAll().Any(m =>
            string.Equals(m.Title, movie.Title, StringComparison.OrdinalIgnoreCase) &&
            m.ReleaseYear == movie.ReleaseYear);

        if (exists)
        {
            throw new InvalidOperationException($"Movie '{movie.Title}' ({movie.ReleaseYear}) already exists in catalog.");
        }

        _repository.Add(movie);
    }

    public bool RemoveMovie(Guid movieId)
    {
        Movie? movie = GetMovieById(movieId);
        if (movie is null)
        {
            return false;
        }

        return _repository.Remove(movie);
    }

    public Movie? GetMovieById(Guid movieId)
    {
        return _repository.GetAll().FirstOrDefault(m => m.Id == movieId);
    }

    public void AddReview(Guid movieId, UserReview review)
    {
        ArgumentNullException.ThrowIfNull(review);

        Movie movie = GetMovieById(movieId)
            ?? throw new KeyNotFoundException($"Movie with ID '{movieId}' was not found.");

        movie.AddReview(review);
    }

    public double CalculateAverageRating(Guid movieId)
    {
        Movie movie = GetMovieById(movieId)
            ?? throw new KeyNotFoundException($"Movie with ID '{movieId}' was not found.");

        return movie.CalculateAverageRating();
    }

    public List<Movie> SortMoviesByRating(bool descending = true)
    {
        List<Movie> movies = _repository.GetAll();
        movies.Sort(new MovieRatingComparer(descending));
        return movies;
    }

    public List<Movie> SortMoviesByYear(bool descending = true)
    {
        List<Movie> movies = _repository.GetAll();
        movies.Sort(new MovieYearComparer(descending));
        return movies;
    }

    public List<Movie> FilterByAge(int maxAge)
    {
        if (maxAge < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAge), "Age filter value cannot be negative.");
        }

        return _repository.Find(m => m.MinAge <= maxAge);
    }

    public void WatchMovie(User user, Guid movieId)
    {
        ArgumentNullException.ThrowIfNull(user);

        Movie movie = GetMovieById(movieId)
            ?? throw new KeyNotFoundException($"Movie with ID '{movieId}' was not found.");

        if (user.Age < movie.MinAge)
        {
            throw new AgeRestrictionException(user.Age, movie.MinAge);
        }

        if (user.Subscription < movie.RequiredPlan)
        {
            throw new PremiumContentException(user.Subscription.ToString(), movie.RequiredPlan.ToString());
        }
    }

    public void SaveData()
    {
        _fileService.Save(_repository.GetAll());
    }

    public void LoadData()
    {
        List<Movie> loadedMovies = _fileService.Load();
        _repository.Clear();

        foreach (Movie movie in loadedMovies)
        {
            _repository.Add(movie);
        }

        if (_repository.GetAll().Count == 0)
        {
            SeedInitialData();
        }
    }

    private void SeedInitialData()
    {
        Genre sciFi = new("Sci-Fi", "Science fiction and futuristic themes");
        Genre drama = new("Drama", "In-depth character development and emotional themes");
        Genre animation = new("Animation", "Animated cartoons and family entertainment");

        Movie inception = new("Inception", "Christopher Nolan", 2010, 148, 16, sciFi, SubscriptionTier.Standard);
        inception.AddReview(new UserReview("Alex", 9, "Masterpiece!"));
        inception.AddReview(new UserReview("Sarah", 10, "Mind blowing concept!"));

        Movie interstellar = new("Interstellar", "Christopher Nolan", 2014, 169, 12, sciFi, SubscriptionTier.Premium);
        interstellar.AddReview(new UserReview("John", 10, "Best soundtrack and visuals."));

        Movie lionKing = new("The Lion King", "Roger Allers", 1994, 88, 0, animation, SubscriptionTier.Free);
        lionKing.AddReview(new UserReview("Emma", 9, "Childhood classic."));

        Movie oppenheimer = new("Oppenheimer", "Christopher Nolan", 2023, 180, 18, drama, SubscriptionTier.Premium);
        oppenheimer.AddReview(new UserReview("David", 9, "Incredible acting and tension."));

        _repository.Add(inception);
        _repository.Add(interstellar);
        _repository.Add(lionKing);
        _repository.Add(oppenheimer);
    }
}