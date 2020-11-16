using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace BigDataIMDB
{
    /// <summary>
    /// Classes for parsing lines.
    /// The point of parsing lines externally is to optimise time required for it.
    /// First of all, each method only parses columns that are needed to be parsed.
    /// Secondly, it uses Spans, which are very efficient in slicing and cutting 
    /// compared to string.Split.
    /// </summary>
    public struct TsvLineParser
    {
        // define tabbing
        private const char Tab = '\t';

        /// <summary>
        /// Parses line contating information for movies.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Tuple of movie's id and a Movie</returns>
        public static (int, Movie) ParseLineForMovies(ReadOnlySpan<char> line)
        {
            var tabCount = 1;
            int id = 0;
            string title = "";
            string lang = "";

            // parse first 4 tabs
            while (tabCount <= 4)
            {
                var tabAt = line.IndexOf(Tab);

                if (tabCount == 1)
                {
                    var value = int.Parse(line.Slice(2, tabAt - 2)); // don't need first 2 characters
                    id = value;
                }
                else if (tabCount == 3)
                {
                    var value = line.Slice(0, tabAt).ToString();
                    title = value;
                }
                else if (tabCount == 4)
                {
                    var value = line.Slice(0, tabAt).ToString();
                    lang = value;
                }

                // now we only care about next part of the string
                line = line.Slice(tabAt + 1);
                tabCount++;
            }

            return (id, new Movie(title, lang));
        }
        /// <summary>
        /// Parses line containing names of actors and directors
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static (int, Staff) ParseLineForActor(ReadOnlySpan<char> line)
        {
            var tabCount = 1;
            int id = 0;
            var cast = new Staff();

            // first 2 tabs
            while (tabCount <= 2)
            {
                var tabAt = line.IndexOf(Tab);

                if (tabCount == 1)
                {
                    var value = int.Parse(line.Slice(2, tabAt - 2));
                    id = value;
                }
                else if (tabCount == 2)
                {
                    var value = line.Slice(0, tabAt).ToString();
                    cast.Name = value;
                }

                line = line.Slice(tabAt + 1);
                tabCount++;
            }

            return (id, cast);
        }
        /// <summary>
        /// Parses line contating information in which did an actor or director take part
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static (int, int, bool) ParseLineForActorMovieInfo(ReadOnlySpan<char> line)
        {
            var tabCount = 1;
            int movieID = 0;
            int staffID = 0;
            bool isActor = true;

            // first 4 tabs
            while (tabCount <= 4)
            {
                var tabAt = line.IndexOf(Tab);

                if (tabCount == 1)
                {
                    var value = int.Parse(line.Slice(2, tabAt - 2)); // don't need first 2 characters
                    movieID = value;
                }
                else if (tabCount == 3)
                {
                    var value = int.Parse(line.Slice(2, tabAt - 2)); // don't need first 2 characters
                    staffID = value;
                }
                else if (tabCount == 4)
                {
                    var value = line.Slice(0, tabAt).ToString();
                    if (value == "actor" || value == "actress")
                    {
                        isActor = true;
                    }
                    else if (value == "director")
                    {
                        isActor = false;
                    }
                }

                line = line.Slice(tabAt + 1);
                tabCount++;
            }

            return (movieID, staffID, isActor);
        }

        public static (int, float, int) ParseLineForMovieRating(ReadOnlySpan<char> line)
        {
            var tabCount = 1;
            int movieID = 0;
            float averageRating = 0;
            int numOfVotes = 0;

            while (tabCount <= 3)
            {
                var tabAt = line.IndexOf(Tab);

                if (tabCount == 1) // id
                {
                    var value = int.Parse(line.Slice(2, tabAt - 2)); // don't need first 2 characters
                    movieID = value;
                }
                else if (tabCount == 2) // average rating
                {
                    // need this weird culture info because float is "0.012345" instead of "0,012345"
                    var value = float.Parse(line.Slice(0, tabAt).ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    averageRating = value;
                }
                else if (tabCount == 3) // number of votes
                {
                    var value = int.Parse(line.Slice(0)); // to the end
                    numOfVotes = value;
                }

                line = line.Slice(tabAt + 1);
                tabCount++;
            }

            return (movieID, averageRating, numOfVotes);
        }

    }
    class CsvLineParser
    {
        // define comma
        private const char Comma = ',';

        /// TODO: (nocheckin) need to think about the way to remove commaAt since it's csv not tsv
        public static (int, Tag) ParseLineForTagIdAndTag(ReadOnlySpan<char> line)
        {
            var commaCount = 0;

            int tagID = 0;
            Tag tag = new Tag();

            while (commaCount <= 1)
            {
                var commaAt = line.IndexOf(Comma);

                if (commaCount == 0) // tag id
                {
                    var value = int.Parse(line.Slice(0, commaAt));
                    tagID = value;
                }
                else if (commaCount == 1) // tag
                {
                    var value = line.Slice(0).ToString();
                    tag = new Tag(value);
                    break;
                }
                commaAt = line.IndexOf(Comma);
                line = line.Slice(commaAt + 1);
                commaCount++;
            }

            return (tagID, tag);
        }
        /// TODO: (nocheckin) need to think about the way to remove commaAt since it's csv not tsv
        public static (int, int, float) ParseLineForTagScores(ReadOnlySpan<char> line)
        {
            var commaCount = 0;

            int movieID = 0;
            int tagID = 0;
            float tagScore = 0;

            while (commaCount <= 2)
            {
                var commaAt = line.IndexOf(Comma);

                if (commaCount == 0) // movie id
                {
                    var value = int.Parse(line.Slice(0, commaAt));
                    movieID = value;
                }
                else if (commaCount == 1) // tag id
                {
                    var value = int.Parse(line.Slice(0, commaAt));
                    tagID = value;
                }
                else if (commaCount == 2) // tag score
                {
                    // need this weird culture info because float is "0.012345" instead of "0,012345"
                    var value = float.Parse(line.Slice(0).ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    tagScore = value;
                    break;
                }
                commaAt = line.IndexOf(Comma);
                line = line.Slice(commaAt + 1);
                commaCount++;
            }

            return (movieID, tagID, tagScore);
        }
        // (nocheckin)
        public static (int, int) ParseLineForLinks(ReadOnlySpan<char> line)
        {
            var commaCount = 0;
                       
            int movieImdbID = 0;
            int movieLensID = 0;

            while (commaCount <= 2)
            {
                var commaAt = line.IndexOf(Comma);

                if (commaCount == 1) // movieLens id
                {
                    var value = int.Parse(line.Slice(0, commaAt));
                    movieImdbID = value;
                }
                else if (commaCount == 2) // imdb id
                {
                    int value;
                    if (line.IsEmpty) // there are line like "1316,0115548," in the links file
                        value = 0;  // which means there's no movieLensID for that movie.
                    else            // that's why we need that check
                        value = int.Parse(line.Slice(0));
                    movieLensID = value;
                    break;
                }
                commaAt = line.IndexOf(Comma);
                line = line.Slice(commaAt + 1);
                commaCount++;
            }

            return (movieLensID, movieImdbID);
        }
    }
}
