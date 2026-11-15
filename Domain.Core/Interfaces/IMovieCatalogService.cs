using Domain.Core.Models;

namespace Domain.Core.Interfaces;

public interface IMovieCatalogService
{
    IReadOnlyList<Movie> GetAllMovies();
    void AddMovie(Movie movie);
    bool RemoveMovie(Guid movieId);
    Movie? GetMovieById(Guid movieId);
    void AddReview(Guid movieId, UserReview review);
    double CalculateAverageRating(Guid movieId);
    List<Movie> SortMoviesByRating(bool descending = true);
    List<Movie> SortMoviesByYear(bool descending = true);
    List<Movie> FilterByAge(int maxAge);
    void WatchMovie(User user, Guid movieId);
    void SaveData();
    void LoadData();
}