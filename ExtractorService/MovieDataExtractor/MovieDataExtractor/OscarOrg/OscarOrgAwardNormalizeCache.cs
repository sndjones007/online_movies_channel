using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataExtractor.OscarOrg
{
    public class OscarOrgAwardNormalizeCache
    {
        /// <summary>
        /// Temporarily save persons
        /// </summary>
        public List<string> Persons;

        /// <summary>
        /// Type of awards (name)
        /// </summary>
        public List<string> AwardTypes;

        /// <summary>
        /// Type of awards (name)
        /// </summary>
        public List<string> JobTypes;

        public List<Tuple<int, int>> PersonJobs;

        /// <summary>
        /// A single year cache
        /// </summary>
        public AwardYearCache YearCache;

        /// <summary>
        /// Save the last movie id
        /// </summary>
        public int LastMovieId = 0;

        /// <summary>
        /// Save the last movie id
        /// </summary>
        public int LastSongId = 0;

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            Persons = new List<string>();
            AwardTypes = new List<string>();
            JobTypes = new List<string>();
            PersonJobs = new List<Tuple<int, int>>();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void InitializeYear()
        {
            YearCache = new AwardYearCache();
        }

        /// <summary>
        /// Save the movie name in the temporary list
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public static int SaveAndGetItemId(string value, List<string> list)
        {
            var valueData = value.Trim('"', ' ', '–');
            valueData = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(valueData.ToLower());

            var index = list.IndexOf(valueData);

            if (index >= 0) return index;
            else list.Add(valueData);

            return list.Count - 1;
        }

        /// <summary>
        /// Save the movie name in the temporary list
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public int SaveAndGetPersonId(string person)
        {
            return SaveAndGetItemId(person, Persons);
        }

        /// <summary>
        /// Save the movie name in the temporary list
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public int SaveAndGetAwardTypeId(string awardType)
        {
            return SaveAndGetItemId(awardType, AwardTypes);
        }

        /// <summary>
        /// Save the movie name in the temporary list
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public int SaveAndGetJobTypeId(string jobType)
        {
            return SaveAndGetItemId(jobType, JobTypes);
        }

        public void SavePersonJob(int personId, int jobTypeId)
        {
            PersonJobs.Add(new Tuple<int, int>(personId, jobTypeId));
        }

        public bool ContainsOr(string data, params string[] values)
        {
            var isContain = false;

            for (int i = 0; i < values.Length; i++)
            {
                isContain |= data.ToUpper().Contains(values[i].ToUpper());
            }
            return isContain;
        }
    }

    public class AwardYearCache
    {
        public int YearIndex = 0;

        /// <summary>
        /// Temporarily save movies
        /// </summary>
        public List<string> Movies;

        /// <summary>
        /// Temporary list of songs
        /// </summary>
        public List<Tuple<int,int,string>> Songs;

        /// <summary>
        /// The list of rows
        /// </summary>
        public List<Tuple<int, int, bool, int, int>> awardRows;

        public int _AwardTypeId;
        public bool _IsWinner;

        /// <summary>
        /// Constructor
        /// </summary>
        public AwardYearCache()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            Movies = new List<string>();
            Songs = new List<Tuple<int, int, string>>();
            awardRows = new List<Tuple<int, int, bool, int, int>>();
        }

        /// <summary>
        /// Save the movie name in the temporary list
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public int SaveAndGetMovieId(string movie)
        {
            return OscarOrgAwardNormalizeCache.SaveAndGetItemId(movie, Movies);
        }

        /// <summary>
        /// Save the song in the temporary list
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public void SaveSong(string song, int movieId, int awardTypeId)
        {
            Songs.Add(new Tuple<int, int, string>(movieId, awardTypeId, song));
        }

        /// <summary>
        /// Save a row
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="movieId"></param>
        /// <param name="index"></param>
        internal void SaveRow(int movieId, int personId)
        {
            awardRows.Add(new Tuple<int, int, bool, int, int>(
                YearIndex, _AwardTypeId, _IsWinner, movieId, personId));
        }
    }
}
