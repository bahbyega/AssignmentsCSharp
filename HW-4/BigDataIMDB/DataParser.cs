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

        public DataParser() { }

        public Dictionary<int, Movie> ParseMovieCodes()
        {
            var resultDict = new Dictionary<int, Movie>();

            if (File.Exists(PATH_TO_MOVIE_CODES))
            {
                using (FileStream fileStream = File.OpenRead(PATH_TO_MOVIE_CODES))  
                using (StreamReader streamReader = new StreamReader(fileStream))    
                {
                    // I suppose reading file string 
                    // by string is faster than reading
                    // the entire file into one
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] lineParsed = line.Split("\t");

                        // Only parse certain languages
                        string lang = lineParsed[3].ToLower();

                        if (lang == "en" || lang == "ru")
                        {
                            int key = int.Parse(lineParsed[0].Substring(2));

                            if (!resultDict.ContainsKey(key))
                            {
                                resultDict.Add(key, new Movie(lineParsed[2], lineParsed[3]));
                            }
                        }
                    }
                }
            }
            return resultDict;
        }

    }
}
