﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BigDataIMDB
{
    /// <summary>
    /// Class for optimizing line parsing
    /// </summary>
    class TsvLineParser
    {
        // define tabbing
        private const char Tab = '\t';

        /// <summary>
        /// Parses file contating information for movies.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Tuple of movie's and a Movie</returns>
        public (int, Movie) ParseLineForMovies(ReadOnlySpan<char> line)
        {
            var tabCount = 1;
            int id = 0;
            string title = "";
            string lang = "";

            // parse first 5 tabs
            while (tabCount <= 5)
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
                else if (tabCount == 5)
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
        /// Parses file containing names of actors and directors
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public (int, Staff) ParseLineForActor(ReadOnlySpan<char> line)
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
        /// Parses file contating information in which did an actor or director take part
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
        
    }
}
