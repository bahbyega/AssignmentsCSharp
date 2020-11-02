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

        // dictionaries of data
        private static Dictionary<int, Movie> Movie_Codes_dict = new Dictionary<int, Movie>();
        private static ConcurrentDictionary<int, Movie> Movie_Codes_dict_Conc = new ConcurrentDictionary<int, Movie>();

        private static Dictionary<int, Staff> Staff_dict = new Dictionary<int, Staff>();


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
            else Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
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
                        var parser = new TsvLineParser();
                        var parsedLine = parser.ParseLineForMovies(line);
                        var key = parsedLine.Item1;

                        if (!Movie_Codes_dict.ContainsKey(key))
                        {
                            Movie_Codes_dict.Add(key, parsedLine.Item2);
                        }
                    }
                }
            }
            else Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
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
                        string language = lineParsed[4].ToLower();

                        if (language == "en" || language == "ru")
                        {
                            int key = int.Parse(lineParsed[0].Substring(2));

                            // collect data in a dict
                            if (!Movie_Codes_dict.ContainsKey(key))
                            {
                                Movie_Codes_dict.Add(key, new Movie(lineParsed[2], language));
                            }
                        }
                    }
                }
            }
            else Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
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
                    if (line[4] == "ru" || line[4] == "en")
                    {
                        int key = int.Parse(line[0].Substring(2));
                        if (!Movie_Codes_dict.ContainsKey(key))
                        {
                            Movie_Codes_dict.Add(key, new Movie(line[2], line[4]));
                        }
                    }
                }
            }
            else Console.WriteLine("Couldn't find file {0}", PATH_TO_MOVIE_CODES);
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
                        var parser = new TsvLineParser();
                        (int id, Movie movie) = parser.ParseLineForMovies(str.AsSpan());

                        // collect data in a dict
                        if (!Movie_Codes_dict_Conc.ContainsKey(id))
                        {
                            Movie_Codes_dict_Conc.TryAdd(id, movie);
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
                        var lineParser = new TsvLineParser();
                        (int id, Staff cast) = lineParser.ParseLineForActor(line);

                        if (!Staff_dict.ContainsKey(id))
                        {
                            Staff_dict.Add(id, cast);
                        }
                    }
                }
            }
            else Console.WriteLine("Couldn't find file {0}", PATH_TO_ACTORS_DIRECTORS_NAMES_TXT);
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
                            FindMovieForStaff(movieID, staffID, isActor);
                        }
                    }
                }
            }
            else Console.WriteLine("Couldn't find file {0}", PATH_TO_ACTORS_DIRECTORS_CODES_TSV);
        }
        /// <summary>
        /// Connects staff with its movies and vice versa
        /// </summary>
        /// <param name="movieID"></param>
        /// <param name="staffID"></param>
        /// <param name="isActor"></param>
        public static void FindMovieForStaff(int movieID, int staffID, bool isActor)
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
    }
}
