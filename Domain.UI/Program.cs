using System.Globalization;
using System.Text.Json;
using Domain.Core.Exceptions;
using Domain.Core.Interfaces;
using Domain.Core.Models;
using Domain.Core.Repositories;
using Domain.Core.Services;

namespace Domain.UI;

public static class Program
{
    private static User _currentUser = new("Viewer", 16, SubscriptionTier.Standard);

    public static void Main()
    {
        string dataFilePath = Path.Combine(AppContext.BaseDirectory, "movies.json");

        IRepository<Movie> repository = new Repository<Movie>();
        JsonFileService<Movie> fileService = new(dataFilePath);
        IMovieCatalogService catalogService = new MovieCatalogService(repository, fileService);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine();
        Console.WriteLine("              ONLINE MOVIE CATALOG               ");
        Console.WriteLine($"Data file: {dataFilePath}");

        try
        {
            catalogService.LoadData();
            Console.WriteLine("Movies loaded successfully from storage.");
        }
        catch (Exception ex) when (IsExpectedError(ex))
        {
            Console.WriteLine($"Warning during startup data loading: {ex.Message}");
        }

        bool running = true;
        while (running)
        {
            PrintMenu();
            string choice = ReadText("Select an option: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        DisplayAllMovies(catalogService.GetAllMovies());
                        break;
                    case "2":
                        ViewMovieDetails(catalogService);
                        break;
                    case "3":
                        AddNewMovie(catalogService);
                        break;
                    case "4":
                        AddMovieReview(catalogService);
                        break;
                    case "5":
                        SortMovies(catalogService);
                        break;
                    case "6":
                        FilterMoviesByAge(catalogService);
                        break;
                    case "7":
                        SimulateWatchMovie(catalogService);
                        break;
                    case "8":
                        RemoveMovie(catalogService);
                        break;
                    case "9":
                        ManageUserProfile();
                        break;
                    case "10":
                        catalogService.SaveData();
                        Console.WriteLine("Movies saved successfully to JSON file.");
                        break;
                    case "0":
                        catalogService.SaveData();
                        Console.WriteLine("Data saved automatically. Goodbye!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please enter a number between 0 and 10.");
                        break;
                }
            }
            catch (AgeRestrictionException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[AGE RESTRICTION ERROR] {ex.Message}");
                Console.ResetColor();
            }
            catch (PremiumContentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[SUBSCRIPTION ERROR] {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex) when (IsExpectedError(ex))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"[OPERATION ERROR] {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--------------------- MENU ----------------------");
        Console.WriteLine($"Current User: {_currentUser}");
        Console.WriteLine("1. Show all movies");
        Console.WriteLine("2. View movie details & reviews");
        Console.WriteLine("3. Add new movie");
        Console.WriteLine("4. Add review and rating to movie");
        Console.WriteLine("5. Sort movies (by rating / by release year)");
        Console.WriteLine("6. Filter movies by age restriction");
        Console.WriteLine("7. Watch movie (simulation & access check)");
        Console.WriteLine("8. Remove movie");
        Console.WriteLine("9. Change current user profile (age / plan)");
        Console.WriteLine("10. Save movies to file");
        Console.WriteLine("0. Exit (auto-save)");
        Console.WriteLine("-------------------------------------------------");
    }

    private static void DisplayAllMovies(IReadOnlyList<Movie> movies)
    {
        if (movies.Count == 0)
        {
            Console.WriteLine("Movie catalog is empty.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Catalog contains {movies.Count} movies:");
        for (int i = 0; i < movies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {movies[i]}");
        }
    }

    private static void ViewMovieDetails(IMovieCatalogService service)
    {
        Movie movie = SelectMovie(service);
        Console.WriteLine();
        Console.WriteLine("----------------- MOVIE DETAILS -----------------");
        Console.WriteLine($"ID:           {movie.Id}");
        Console.WriteLine($"Title:        {movie.Title}");
        Console.WriteLine($"Director:     {movie.Director}");
        Console.WriteLine($"Release Year: {movie.ReleaseYear}");
        Console.WriteLine($"Duration:     {movie.DurationMinutes} minutes");
        Console.WriteLine($"Genre:        {movie.Genre.Name} - {movie.Genre.Description}");
        Console.WriteLine($"Age Limit:    {movie.MinAge}+");
        Console.WriteLine($"Required Tier:{movie.RequiredPlan}");
        Console.WriteLine($"Avg Rating:   {movie.CalculateAverageRating():F1} / 10");
        Console.WriteLine("Reviews:");

        if (movie.Reviews.Count == 0)
        {
            Console.WriteLine("  (No reviews yet)");
        }
        else
        {
            foreach (UserReview review in movie.Reviews)
            {
                Console.WriteLine($"  * {review}");
            }
        }
        Console.WriteLine("-------------------------------------------------");
    }

    private static void AddNewMovie(IMovieCatalogService service)
    {
        Console.WriteLine();
        Console.WriteLine("--- Add New Movie ---");
        string title = ReadText("Title: ");
        string director = ReadText("Director: ");
        int year = ReadInteger("Release Year (1895 - 2028): ", 1895, 2028);
        int duration = ReadInteger("Duration in minutes (1 - 600): ", 1, 600);
        int minAge = ReadInteger("Age restriction (0, 6, 12, 16, 18, 21): ", 0, 21);
        string genreName = ReadText("Genre name: ");
        string genreDesc = ReadText("Genre description: ");
        SubscriptionTier tier = ReadSubscriptionTier("Required subscription tier (1-Free, 2-Standard, 3-Premium): ");

        Genre genre = new(genreName, genreDesc);
        Movie movie = new(title, director, year, duration, minAge, genre, tier);

        service.AddMovie(movie);
        Console.WriteLine($"Movie \"{title}\" added successfully.");
    }

    private static void AddMovieReview(IMovieCatalogService service)
    {
        Movie movie = SelectMovie(service);

        Console.WriteLine();
        Console.WriteLine($"Adding review for \"{movie.Title}\":");
        string author = ReadText("Your name: ");
        int rating = ReadInteger("Rating score (1 - 10): ", 1, 10);
        string comment = ReadText("Review comment: ");

        UserReview review = new(author, rating, comment);
        service.AddReview(movie.Id, review);

        Console.WriteLine($"Review added! New average rating: {service.CalculateAverageRating(movie.Id):F1}/10");
    }

    private static void SortMovies(IMovieCatalogService service)
    {
        Console.WriteLine();
        Console.WriteLine("1. Sort by average rating (highest first)");
        Console.WriteLine("2. Sort by average rating (lowest first)");
        Console.WriteLine("3. Sort by release year (newest first)");
        Console.WriteLine("4. Sort by release year (oldest first)");

        string option = ReadText("Choose sorting mode: ");
        List<Movie> sorted;

        switch (option)
        {
            case "1":
                sorted = service.SortMoviesByRating(descending: true);
                break;
            case "2":
                sorted = service.SortMoviesByRating(descending: false);
                break;
            case "3":
                sorted = service.SortMoviesByYear(descending: true);
                break;
            case "4":
                sorted = service.SortMoviesByYear(descending: false);
                break;
            default:
                Console.WriteLine("Invalid option.");
                return;
        }

        DisplayAllMovies(sorted);
    }

    private static void FilterMoviesByAge(IMovieCatalogService service)
    {
        int maxAge = ReadInteger("Enter your maximum age limit: ", 0, 130);
        List<Movie> filtered = service.FilterByAge(maxAge);

        Console.WriteLine($"\nMovies suitable for age <= {maxAge}:");
        DisplayAllMovies(filtered);
    }

    private static void SimulateWatchMovie(IMovieCatalogService service)
    {
        Movie movie = SelectMovie(service);

        Console.WriteLine($"\nAttempting to watch \"{movie.Title}\" as {_currentUser}...");
        service.WatchMovie(_currentUser, movie.Id);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Access granted! Enjoy watching \"{movie.Title}\"!");
        Console.ResetColor();
    }

    private static void RemoveMovie(IMovieCatalogService service)
    {
        Movie movie = SelectMovie(service);
        bool confirmed = ReadBoolean($"Are you sure you want to delete \"{movie.Title}\"?");

        if (confirmed)
        {
            service.RemoveMovie(movie.Id);
            Console.WriteLine("Movie deleted successfully.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }

    private static void ManageUserProfile()
    {
        Console.WriteLine();
        Console.WriteLine($"Current profile: {_currentUser}");
        string name = ReadText("Enter username: ");
        int age = ReadInteger("Enter age: ", 0, 130);
        SubscriptionTier tier = ReadSubscriptionTier("Select subscription (1-Free, 2-Standard, 3-Premium): ");

        _currentUser = new User(name, age, tier);
        Console.WriteLine($"User profile updated: {_currentUser}");
    }

    private static Movie SelectMovie(IMovieCatalogService service)
    {
        IReadOnlyList<Movie> movies = service.GetAllMovies();
        if (movies.Count == 0)
        {
            throw new InvalidOperationException("No movies available in the catalog.");
        }

        DisplayAllMovies(movies);
        int index = ReadInteger($"Select movie number (1 - {movies.Count}): ", 1, movies.Count);
        return movies[index - 1];
    }

    private static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    private static int ReadInteger(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) &&
                value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Error! Please enter a whole number between {min} and {max}.");
        }
    }

    private static SubscriptionTier ReadSubscriptionTier(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out int num) && Enum.IsDefined(typeof(SubscriptionTier), num))
            {
                return (SubscriptionTier)num;
            }

            if (Enum.TryParse<SubscriptionTier>(input, true, out var tier))
            {
                return tier;
            }

            Console.WriteLine("Invalid plan. Choose 1 for Free, 2 for Standard, 3 for Premium.");
        }
    }

    private static bool ReadBoolean(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (input is "y" or "yes")
            {
                return true;
            }

            if (input is "n" or "no")
            {
                return false;
            }

            Console.WriteLine("Please enter 'y' for yes or 'n' for no.");
        }
    }

    private static bool IsExpectedError(Exception exception)
    {
        return exception is ArgumentException
            or InvalidOperationException
            or KeyNotFoundException
            or IOException
            or JsonException
            or UnauthorizedAccessException;
    }
}