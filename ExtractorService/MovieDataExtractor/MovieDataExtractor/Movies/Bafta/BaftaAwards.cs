using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies.Bafta
{
    
    class BaftaAward
    {
        public int Year;
        public List<BaftaCategory> baftaCategories;
    }

    internal class BaftaCategory
    {
        public string Category;
        public List<BaftaAwardItem> Wins;
        public List<BaftaAwardItem> Nominations;
    }

    public class BaftaAwardItem
    {
        public string Key;
        public List<string> Value;
    }
}
