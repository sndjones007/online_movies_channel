using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies
{
    class WikiMovieItem
    {
        public string Name { get; set; }
        public string PosterUrl { get; set; }
        public Dictionary<string, List<WikiMoviePropertyItem>> Properties { get; set; }
        public List<WikiCast> Castings { get; set; }
    }

    class WikiMoviePropertyItem
    {
        public string Url { get; set; }
        public string Value { get; set; }
    }

    class WikiCast
    {
        public string Name { get; set; }
        public string CharacterName { get; set; }
    }
}
