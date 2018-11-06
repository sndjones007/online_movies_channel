using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MovieDataExtractor.OscarOrg
{
    public class TempModelForExtract
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Store a year's worth of data
        /// </summary>
        public OscarHistoryYearModel YearModel;

        /// <summary>
        /// Initialize the year model cache data every start of new oscar year parsing
        /// </summary>
        public void InitYearModel()
        {
            YearModel = new OscarHistoryYearModel();
        }

        public List<Tuple<string, string, string>> keySubkeyWinOders = new List<Tuple<string, string, string>>();
        public List<Tuple<string, string, string>> keySubkeyNomineesOders = new List<Tuple<string, string, string>>();
    }

    /// <summary>
    /// A class to hold a single year of data extracted from the url.
    /// It contains both ceremony and awards data
    /// </summary>
    public class OscarHistoryYearModel
    {
        /// <summary>
        /// The oscar year, like 1929
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The category name
        /// </summary>
        public string CategoryHeader { get; set; }

        /// <summary>
        /// The Winner, Key name, key value combination
        /// </summary>
        public List<Tuple<bool, string, string>> CategoryItems { get; set; }

        // year, content, href, title, youtubeData, titleData, descData, dataFacebook, dataTwitterUrl,
        public List<Tuple<string, string, string, string, string>> HighlightPictures { get; set; }

        /// <summary>
        /// The moments of the year
        /// </summary>
        public List<string> Moments { get; set; }

        /// <summary>
        /// Initialize the year model cache data every start of new oscar year parsing
        /// </summary>
        public void InitCategoryModel()
        {
            CategoryHeader = "";
            CategoryItems = new List<Tuple<bool, string, string>>();
        }

        /// <summary>
        /// Initialize the pictures model data
        /// </summary>
        public void InitHighlightsPictureModel()
        {
            HighlightPictures = new List<Tuple<string, string, string, string, string>>();
        }

        /// <summary>
        /// Initialize the moments model data
        /// </summary>
        public void InitMomentsModel()
        {
            Moments = new List<string>();
        }

        /// <summary>
        /// Add a single category item
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isWinner"></param>
        public void AddCategoryItem(string key, string value, bool isWinner = false)
        {
            CategoryItems.Add(new Tuple<bool, string, string>(isWinner, key, value));
        }

        /// <summary>
        /// Add an picture item
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="youtubeUrl"></param>
        /// <param name="href_url"></param>
        public void AddHighlightPic(string content, string title, string description, string youtubeUrl, string href_url)
        {
            HighlightPictures.Add(new Tuple<string, string, string, string, string>(
                content, title, description, youtubeUrl, href_url
                ));
        }
    }
}
