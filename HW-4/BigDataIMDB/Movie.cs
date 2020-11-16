using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataIMDB
{
    public struct Movie
    {
        public string Title;
        public HashSet<Staff> Staff;
        public HashSet<Tag> Tags;
        public float Rating;
        private string Language;

        public Movie(string movieTitle, string language)
        {
            Title = movieTitle;
            Staff = new HashSet<Staff>();
            Tags = new HashSet<Tag>();
            Rating = 0;
            Language = language;
        }
        /// <summary>
        /// Compares parameter movie with this movie.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns>float - similarity score from 0 to 1</returns>
        public float CompareTo(Movie movie)
        {
            int countOfSameStaff = 0;
            int countOfSameTags = 0;

            foreach (Staff staff in movie.Staff)
            {
                if (this.Staff.Contains(staff))
                {
                    countOfSameStaff++;
                }
            }

            foreach (Tag tag in movie.Tags)
            {
                if (this.Tags.Contains(tag))
                {
                    countOfSameTags++;
                }
            }

            // Similarity score is built like this:
            // Min score = 0.0, max score = 1.0.
            // 0 - 0.5 is intersection between movies actors and tags sets:
            //      percent of same staff in a movie (from parameter) * 0.5 
            //      percent of same tags in a movie (from parameter) * 0.5 
            //to get 0.5 - 1.0 we add Rating (0.0 <= IMDB rating <= 10.0) * 0.05 
            float similarityScore = ((movie.Staff.Count == 0 ? 0 : ((float)countOfSameStaff / movie.Staff.Count) / 2)
                                    + (movie.Tags.Count == 0 ? 0 : ((float)countOfSameTags / movie.Tags.Count) / 2 )) / 2
                                    + this.Rating * (float)0.05;
            return similarityScore;
        }
            
    }
}
