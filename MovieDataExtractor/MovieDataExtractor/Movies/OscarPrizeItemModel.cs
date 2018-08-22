using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies
{
    /// <summary>
    /// Group by movie
    /// </summary>
    public class OscarPrizeItemModel
    {
        public string MovieName { get; set; }
        public int WinsCount { get; set; }
        public int NominationsCount { get; set; }
        public List<MoviePropertyItemModel> Wins { get; set; }
        public List<MoviePropertyItemModel> Nominations { get; set; }
    }
}
