using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataExtractor.Cinemasight
{
    /// <summary>
    /// A temporary model cache to store the extracted data in memory
    /// </summary>
    public class TempModelForExtract
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Award history index parse from the start page
        /// Item1 : The Url
        /// Item2 : The name
        /// </summary>
        public List<Tuple<string, int>> AwardHistoryIndices;

        /// <summary>
        /// Store a year's worth of data
        /// </summary>
        public OscarHistoryYearModel YearModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public TempModelForExtract()
        {
            AwardHistoryIndices = new List<Tuple<string, int>>();
        }

        /// <summary>
        /// To Add a single (url, name) pair for oscar award index
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        public void AddIndex(string url, int index)
        {
            AwardHistoryIndices.Add(new Tuple<string, int>(url, index));

            logger.DebugFormat("Added to Award Index ({0},{1})", url, index);
        }

        /// <summary>
        /// Initialize the year model cache data every start of new oscar year parsing
        /// </summary>
        public void InitYearModel()
        {
            YearModel = new OscarHistoryYearModel();
        }
    }

    /// <summary>
    /// A class to hold a single year of data extracted from the url.
    /// It contains both ceremony and awards data
    /// </summary>
    public class OscarHistoryYearModel
    {
        /// <summary>
        /// The index value of the oscar year, like 1929 is '1st'
        /// </summary>
        public int OscarIndex { get; set; }

        /// <summary>
        /// The category name
        /// </summary>
        public string CategoryHeader { get; set; }

        /// <summary>
        /// The Winner, Key name, key value combination
        /// </summary>
        public List<Tuple<bool, string, string>> CategoryItems { get; set; }

        /// <summary>
        /// Initialize the year model cache data every start of new oscar year parsing
        /// </summary>
        public void InitCategoryModel()
        {
            CategoryHeader = "";
            CategoryItems = new List<Tuple<bool, string, string>>();
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
    }
}
