using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BigDataIMDB
{
    /// <summary>
    /// Class for Parsing data from csv, tsv, txt IMDB datasets
    /// </summary>
    class DataParser
    {
        // Paths to datasets
        private const string PATH_TO_MOVIE_CODES= @"ml-latest/MovieCodes_IMDB.tsv";
        private const string PATH_TO_ACTORS_DIRECTORS_NAMES_TXT = "ml-latest/ActorsDirectorsNames_IMDB.txt";
        private const string PATH_TO_ACTORS_DIRECTORS_NAMES_TSV = "ml-latest/ActorsDirectorsNames_IMDB.tsv";
        private const string PATH_TO_RATINGS = "ml-latest/Ratings_IMDB.tsv";
        private const string PATH_TO_TAG_CODES_MOVIE_LENS = "ml-latest/TagCodes_MovieLens.csv";
        private const string PATH_TO_TAG_SCORES_MOVIE_LENS = "ml-latest/TagScores_MovieLens.csv";

        // dictionary of data
        private  Dictionary<string, Movie> Movie_Codes_dict = new Dictionary<string, Movie>();
        public DataParser() { }

        /// <summary>
        /// Parses movie codes tsv file
        /// </summary>
        public void ParseMovieCodes()
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
                            if (!Movie_Codes_dict.ContainsKey(lineParsed[0]))
                            {
                                Movie_Codes_dict.Add(lineParsed[0], new Movie(lineParsed[2], language));
                            }
                        }
                    }
                }
            }
        }

    }
}
