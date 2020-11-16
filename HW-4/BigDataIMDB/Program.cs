using System;

namespace BigDataIMDB
{
    class Program
    {
        static void UxShowWelcomePage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Welcome to the movie advisor.");

            while (true)
            {
                Console.WriteLine("What film do you want to watch ?");
                Console.Write("-> ");
                Console.ForegroundColor = ConsoleColor.White;

                string movieName;
                movieName = Console.ReadLine();
                if (movieName.ToLower() == "exit")
                {
                    System.Environment.Exit(1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("--------------------------------------------------");
                    Movie? movie = FindMovieByName(movieName);
                    if (movie != null)
                    {
                        UxShowMovieInfo((Movie)movie);
                    }

                    Console.WriteLine("Write 'exit' if you want to quit.\n");
                }
            }
        }

        static Movie? FindMovieByName(string name)
        {
            if (DataParser.Movie_Name_Id_dict.TryGetValue(name, out int id)) 
            {
                DataParser.Movie_Codes_dict.TryGetValue(id, out Movie movie);
                return movie;
            }
            else
            {
                Console.WriteLine("No such movie in our data.");
                return null;
            }
        }

        static void UxShowMovieInfo(Movie movie)
        {
            // output title
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"| {movie.Title}");
            Console.ForegroundColor = ConsoleColor.White;

            // output director
            Console.Write("| Director: ");
            foreach (var person in movie.Staff)
            {
                if (person.isDirector.Contains(movie))
                {
                    Console.Write($"{person.Name}");
                }
            }
            Console.WriteLine();

            // output rating
            Console.WriteLine($"| Rating: {movie.Rating}");

            // output actors
            Console.Write("| Actors: ");
            foreach (var person in movie.Staff)
            {
                if (person.isActor.Contains(movie))
                {
                    Console.Write($" {person.Name}, ");
                }
            }
            Console.Write("\b\b\b"); // remove last three characters
            Console.WriteLine();

            // output tags
            Console.Write("| Tags: ");
            foreach (var tag in movie.Tags)
            {
                Console.Write($"{tag.Name}, ");
            }
            Console.Write("\b\b\b"); // remove last three characters
            Console.WriteLine();

            // output similar movies
            Console.Write("| Similar movies: ");
            foreach (var similarMovie in DataParser.FindSimilarMovies(movie))
            {
                Console.Write($"{similarMovie.Title}, ");
            }
            Console.Write("\b\b\b"); // remove last three characters
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--------------------------------------------------");
        }

        static void Main(string[] args)
        {
            DataParser dataParser = new DataParser();
            dataParser.ParseMovieCodesLinq();
            dataParser.CollectRatingForEachMovie();
            dataParser.ParseActorsDirectorsNames();
            dataParser.ParseActorsDirectorsMovieInfo();
            dataParser.ParseTagsAndItsIds();
            dataParser.ParseTagsAndItsMovieScores();
            UxShowWelcomePage();
        }
    }
}
