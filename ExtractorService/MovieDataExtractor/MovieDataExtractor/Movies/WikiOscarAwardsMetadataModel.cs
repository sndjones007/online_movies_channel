using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies
{
    class WikiOscarAwardsMetadataModel
    {
        public string AwardName { get; set; }
        public List<WikiMovieItem> WinMovieItems { get; set; }
        public List<WikiMovieItem> NominationMovieItems { get; set; }
    }
}
