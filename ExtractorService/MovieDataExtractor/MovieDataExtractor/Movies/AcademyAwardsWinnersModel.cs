using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies
{
    public class AcademyAwardsWinnersModel
    {
        public string PageUrl { get; set; }
        public int Rank { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime HonoringReleaseStartDate { get; set; }
        public DateTime HonoringReleaseEndDate { get; set; }
        public List<OscarPrizeItemModel> PrizeItemModels { get; set; }
    }
}
