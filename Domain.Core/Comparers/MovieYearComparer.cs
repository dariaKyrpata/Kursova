using Domain.Core.Models;

namespace Domain.Core.Comparers;

public class MovieYearComparer : IComparer<Movie>
{
    private readonly bool _descending;

    public MovieYearComparer(bool descending = true)
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

        int result = x.ReleaseYear.CompareTo(y.ReleaseYear);

        return _descending ? -result : result;
    }
}