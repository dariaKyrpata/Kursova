using Domain.Core.Models;

namespace Domain.Core.Comparers;

public class MovieRatingComparer : IComparer<Movie>
{
    private readonly bool _descending;

    public MovieRatingComparer(bool descending = true)
    {
        _descending = descending;
    }

    public int Compare(Movie? x, Movie? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return _descending ? 1 : -1;
        }

        if (y is null)
        {
            return _descending ? -1 : 1;
        }

        double ratingX = x.CalculateAverageRating();
        double ratingY = y.CalculateAverageRating();

        int result = ratingX.CompareTo(ratingY);

        return _descending ? -result : result;
    }
}