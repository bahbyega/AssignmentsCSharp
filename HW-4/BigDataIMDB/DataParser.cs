using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BigDataIMDB
{
    /// <summary>
    /// Class for Parsing data from csv, tsv, txt IMDB datasets
    /// </summary>
    class DataParser
    {
        // Paths to datasets
        private const string PATH_TO_MOVIE_CODES = @"ml-latest/MovieCodes_IMDB.tsv";
        private const string PATH_TO_ACTORS_DIRECTORS_NAMES_TXT = @"ml-latest/ActorsDirectorsNames_IMDB.txt";
        private const string PATH_TO_ACTORS_DIRECTORS_CODES_TSV = @"ml-latest/ActorsDirectorsCodes_IMDB.tsv";
        private const string PATH_TO_RATINGS = @"ml-latest/Ratings_IMDB.tsv";
        private const string PATH_TO_TAG_CODES_MOVIE_LENS = @"ml-latest/TagCodes_MovieLens.csv";
        private const string PATH_TO_TAG_SCORES_MOVIE_LENS = @"ml-latest/TagScores_MovieLens.csv";
        private const string PATH_TO_LINKS_FROM_MOVIELENS_TO_IMDB = @"ml-latest/links_IMDB_MovieLens.csv";

        // dictionaries of data
        public static Dictionary<int, Movie> Movie_Codes_dict = new Dictionary<int, Movie>();
        public static Dictionary<string, int> Movie_Name_Id_dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public static ConcurrentDictionary<int, Movie> Movie_Codes_dict_Conc = new ConcurrentDictionary<int, Movie>();

        public static Dictionary<int, Staff> Staff_dict = new Dictionary<int, Staff>();

        public static Dictionary<int, Tag> Tags_dict = new Dictionary<int, Tag>();
        public static Dictionary<int, int> LinksFromMovieLensToImdbIds_dict = new Dictionary<int, int>();

        public DataParser() { }

        // probably gonna be only one version left -> the best working one (linq for now)
        #region Different versions of the one function
        
        /// <summary>
        /// Parses file contating info about movie -> id, name, language.
        /// </summary>
        public void ParseMovieCodes_BlockColl()
        {
            if (File.Exists(PATH_TO_MOVIE_CODES))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_MOVIE_CODES))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    // read file string by string
                    string line;

                    var blockingCollection = new BlockingCollection<string>();

                    Task firstCoreTask = RunTasksWithBlockingCollection(blockingCollection);
                    Task secondCoreTask = RunTasksWithBlockingCollection(blockingCollection);
                    Task thirdCoreTask = RunTasksWithBlockingCollection(blockingCollection);
                    
                    streamReader.ReadLine(); // skip line
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        blockingCollection.Add(line);
                    }
                    blockingCollection.CompleteAdding();
                    Task.WhenAll(firstCoreTask, secondCoreTask, thirdCoreTask);
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
        }

        public void ParseMovieCodesNoParallel()
        {
            if (File.Exists(PATH_TO_MOVIE_CODES))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_MOVIE_CODES))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    // read file string by string
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine(); // skip first line
                    while ((line = streamReader.ReadLine().AsSpan()) != null)
                    {
                        (int id, Movie movie) = TsvLineParser.ParseLineForMovies(line);

                        if (!Movie_Codes_dict.ContainsKey(id))
                        {
                            Movie_Codes_dict.Add(id, movie);
                        }

                        // add reference from movie name to its id
                        if (!Movie_Name_Id_dict.ContainsKey(movie.Title))
                        {
                            Movie_Name_Id_dict.Add(movie.Title, id);
                        }
                    }
                }
                CollectRatingForEachMovie(); // parse file with rating
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
        }
        public void ParseMovieCodes_Split()
        {
            if (File.Exists(PATH_TO_MOVIE_CODES))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_MOVIE_CODES))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    // read file string by string
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] lineParsed = line.Split("\t");

                        // Only parse certain languages
                        string language = lineParsed[3].ToLower();

                        if (language == "en" || language == "ru" || language == "us")
                        {
                            int id = int.Parse(lineParsed[0].Substring(2));
                            Movie movie = new Movie(lineParsed[2], language);

                            // collect data in a dict
                            if (!Movie_Codes_dict.ContainsKey(id))
                            {
                                Movie_Codes_dict.Add(id, movie);
                            }

                            // add reference from movie name to its id
                            if (!Movie_Name_Id_dict.ContainsKey(movie.Title))
                            {
                                Movie_Name_Id_dict.Add(movie.Title, id);
                            }
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
        }
        public void ParseMovieCodesLinq()
        {
            if (File.Exists(PATH_TO_MOVIE_CODES))
            {
                var tempstrings = File
                    .ReadLines(PATH_TO_MOVIE_CODES)
                    .AsParallel()
                    .Skip(1)
                    .Select(line => line.Split("\t"));
                foreach (var line in tempstrings)
                {
                    if (line[3].ToLower() == "ru" || line[3].ToLower() == "en" || line[3].ToLower() == "us")
                    {
                        int id = int.Parse(line[0].Substring(2));
                        Movie movie = new Movie(line[2], line[4]);

                        if (!Movie_Codes_dict.ContainsKey(id))
                        {
                            Movie_Codes_dict.Add(id, movie);
                        }
                        // add reference from movie name to its id
                        if (!Movie_Name_Id_dict.ContainsKey(movie.Title))
                        {
                            Movie_Name_Id_dict.Add(movie.Title, id);
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
        }
        public Task RunTasksWithBlockingCollection(BlockingCollection<string> input)
        {
            return Task.Factory.StartNew(() =>
            {
                Regex match = new Regex("\t(EN|RU|US)\t", RegexOptions.IgnoreCase |
                    RegexOptions.Compiled);
                foreach (var str in input.GetConsumingEnumerable())
                {
                    if (match.IsMatch(str))
                    {
                        (int id, Movie movie) = TsvLineParser.ParseLineForMovies(str.AsSpan());

                        // collect data in a dict
                        if (!Movie_Codes_dict_Conc.ContainsKey(id))
                        {
                            Movie_Codes_dict_Conc.TryAdd(id, movie);
                        }
                        // add reference from movie name to its id
                        if (!Movie_Name_Id_dict.ContainsKey(movie.Title))
                        {
                            Movie_Name_Id_dict.Add(movie.Title, id);
                        }
                    }
                }

            }, TaskCreationOptions.LongRunning);
        }
        #endregion
        /// <summary>
        /// Parses file with information of actors and its ids
        /// </summary>
        public void ParseActorsDirectorsNames()
        {
            if (File.Exists(PATH_TO_ACTORS_DIRECTORS_NAMES_TXT))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_ACTORS_DIRECTORS_NAMES_TXT))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    // read file string by string
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine();
                    while ((line = streamReader.ReadLine().AsSpan()) != null)
                    {
                        (int id, Staff cast) = TsvLineParser.ParseLineForActor(line);

                        if (!Staff_dict.ContainsKey(id))
                        {
                            Staff_dict.Add(id, cast);
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_ACTORS_DIRECTORS_NAMES_TXT);
        }
        /// <summary>
        /// Parses file containing information in which movies actors take part
        /// </summary>
        public void ParseActorsDirectorsMovieInfo()
        {
            if (File.Exists(PATH_TO_ACTORS_DIRECTORS_CODES_TSV))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_ACTORS_DIRECTORS_CODES_TSV))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine();
                    while ((line = streamReader.ReadLine().AsSpan()) != null)
                    {
                        // get data
                        (int movieID, int staffID, bool isActor) = TsvLineParser.ParseLineForActorMovieInfo(line);

                        // staffID == 0 when ParseLineForActorMoreInfo didn't parse director or actor
                        if (staffID != 0)
                        {
                            ConnectStaffWithMovie(movieID, staffID, isActor);
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_ACTORS_DIRECTORS_CODES_TSV);
        }
        /// <summary>
        /// Connects staff with its movies and vice versa
        /// </summary>
        /// <param name="movieID"></param>
        /// <param name="staffID"></param>
        /// <param name="isActor"></param>
        public void ConnectStaffWithMovie(int movieID, int staffID, bool isActor)
        {
            if (Movie_Codes_dict.ContainsKey(movieID) && Staff_dict.ContainsKey(staffID))
            {
                Movie_Codes_dict.TryGetValue(movieID, out Movie movie);
                Staff_dict.TryGetValue(staffID, out Staff staff);

                if (isActor) staff.isActor.Add(movie);
                else staff.isDirector.Add(movie);

                movie.Staff.Add(staff);
            }
        }

        /// <summary>
        /// Parses file containing rating for each movie and connects them
        /// </summary>
        public void CollectRatingForEachMovie()
        {
            if (File.Exists(PATH_TO_RATINGS))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_RATINGS))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine();
                    while ((line = streamReader.ReadLine().AsSpan()) != null)
                    {
                        // get data
                        (int movieID, float Rating, int numVotes) = TsvLineParser.ParseLineForMovieRating(line);

                        // connect rating with a movie
                        if (Movie_Codes_dict.ContainsKey(movieID))
                        {
                            Movie_Codes_dict.TryGetValue(movieID, out Movie movie);
                            movie.Rating = Rating;
                            Movie_Codes_dict[movieID] = movie; // update value ( is it necessary? )
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_RATINGS);
        }

        #region Tags and everything connected to it
        // Tags data is taken from MovieLens database. That means that movies' IDs from IMDB (which we use)
        // are not the same as in tag files (in tag files IDs are from MovieLens). That's why we have a file
        // of movies' IDs correspondence and we connect tags using that file.

        /// <summary>
        /// 'Tag codes movielens' file contains information about tags in a csv: "tagId, tag"
        /// Parses that file to get dictionary of tags and its IDs
        /// </summary>
        public void ParseTagsAndItsIds()
        {
            if (File.Exists(PATH_TO_TAG_CODES_MOVIE_LENS))
            {

                using (FileStream fileStream = File.OpenRead(PATH_TO_TAG_CODES_MOVIE_LENS))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine();
                    while ((line = streamReader.ReadLine().AsSpan()) != null)
                    {
                        // get data
                        (int tagID, Tag tag) = CsvLineParser.ParseLineForTagIdAndTag(line);
                        if (!Tags_dict.ContainsKey(tagID))
                        {
                            Tags_dict.Add(tagID, tag);
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_TAG_CODES_MOVIE_LENS);
        }

        /// <summary>
        /// 'Tag scores movielens' file contains information about what movie to what tag has what
        /// relevance (tag score) in a csv: "movieId, tagId, relevance"
        /// Parses that file to get Tags scores 
        public void ParseTagsAndItsMovieScores()
        {
            ParseLinksFromMovieLensToImdbIds(); // we need to parse it to know how ids connect

            if (File.Exists(PATH_TO_TAG_SCORES_MOVIE_LENS))
            {

                using (FileStream fileStream = File.OpenRead(PATH_TO_TAG_SCORES_MOVIE_LENS))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine();
                    while ((line = streamReader.ReadLine().AsSpan()) != null)
                    {
                        // get data
                        (int movieLensID, int tagID, float tagScore) = CsvLineParser.ParseLineForTagScores(line);
                        if (tagScore > 0.5) // interested only in score more than 0.5
                        {
                            if (Tags_dict.ContainsKey(tagID))
                            {
                                ConnectTagWithMovie(movieLensID, tagID, tagScore);
                            }
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_TAG_SCORES_MOVIE_LENS);
        }

        /// <summary>
        /// 'Links from movielens to imdbs IDs' file contains information about 
        /// how to connect IDs from different databases
        /// Parses that file to get dictionary of links.
        /// </summary>
        private void ParseLinksFromMovieLensToImdbIds()
        {
            if (File.Exists(PATH_TO_LINKS_FROM_MOVIELENS_TO_IMDB))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_LINKS_FROM_MOVIELENS_TO_IMDB))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    ReadOnlySpan<char> line;
                    streamReader.ReadLine();
                    while ((line = streamReader.ReadLine().AsSpan()) != null )
                    {
                        // get data
                        (int movieLensID, int movieImdbID) = CsvLineParser.ParseLineForLinks(line);
                        if (!LinksFromMovieLensToImdbIds_dict.ContainsKey(movieLensID))
                        {
                            LinksFromMovieLensToImdbIds_dict.Add(movieLensID, movieImdbID);
                        }
                    }
                }
            }
            else
                Console.WriteLine("Couldn't find file {0}", PATH_TO_LINKS_FROM_MOVIELENS_TO_IMDB);
        }

        /// <summary>
        /// Method for connecting tag with id from MovieLens to IMDB
        /// </summary>
        /// <param name="movieLensID"></param>
        /// <param name="tagID"></param>
        /// <param name="tagScore"></param>
        private void ConnectTagWithMovie(int movieLensID, int tagID, float tagScore)
        {
            if (LinksFromMovieLensToImdbIds_dict.ContainsKey(movieLensID))
            {
                if (Tags_dict.ContainsKey(tagID))
                {
                    LinksFromMovieLensToImdbIds_dict.TryGetValue(movieLensID, out int movieImdbID);
                    Tags_dict.TryGetValue(tagID, out Tag tag);

                    if (Movie_Codes_dict.ContainsKey(movieImdbID))
                    {
                        Movie_Codes_dict.TryGetValue(movieImdbID, out Movie movie);
                        movie.Tags.Add(tag); // add tag to movie
                        tag.MoviesWithScores.Add(movie, tagScore); // add movie and its tag score to tag 
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Looks through staff and tags of a movie and searches for similar ones
        /// </summary>
        /// <param name="movie"></param>
        /// <returns>Hashset of similar to movie (from parameter) movies</returns>
        public static HashSet<Movie> FindSimilarMovies(Movie movie)
        {
            Dictionary<Movie, float> similarMoviesWithSimilarityScore = new Dictionary<Movie, float>();

            // min score of similarity we want
            float passScore = (float)0.5;

            // check movies of staff
            foreach(Staff staff in movie.Staff)
            {
                // movies where staff is director
                foreach(Movie movieIsDirector in staff.isDirector)
                {
                    if (movieIsDirector.Title != movie.Title && !similarMoviesWithSimilarityScore.ContainsKey(movieIsDirector))
                    {
                        float score = movieIsDirector.CompareTo(movie);
                        if (score > passScore)
                        {
                            similarMoviesWithSimilarityScore.Add(movieIsDirector, score);
                        }
                    }
                }
                // movies where staff is actor
                foreach (Movie movieIsActor in staff.isActor)
                {
                    if (movieIsActor.Title != movie.Title && !similarMoviesWithSimilarityScore.ContainsKey(movieIsActor))
                    {
                        float score = movieIsActor.CompareTo(movie);
                        if (score > passScore)
                        {
                            similarMoviesWithSimilarityScore.Add(movieIsActor, score);
                        }
                    }
                }
            }

            // sort dictionary by value into a set
            var sortedSimilarMoviesWithSimilarityScore = 
                (from entry in similarMoviesWithSimilarityScore 
                 orderby entry.Value ascending 
                 select entry.Key)
                .Take(10) // choose only top10
                .ToHashSet();

            // check tags of a movie
            /*foreach(Tag tag in movie.Tags)
            {
                foreach(Movie movieIsTag in tag.MoviesWithScores.TryGetValue()
            }*/

            return sortedSimilarMoviesWithSimilarityScore;
        }
    }
}
