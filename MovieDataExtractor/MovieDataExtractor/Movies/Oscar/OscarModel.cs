using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies
{
    class OscarModel
    {
        public OscarModel()
        {
            Awards = new Dictionary<string, OscarAwardModel>();
            Metadata = new Dictionary<string, List<NameItem>>();
        }

        public int Rank { get; set; }
        public string WikiUrl { get; set; }
        public Dictionary<string, List<NameItem>> Metadata { get; set; }
        public Dictionary<string, OscarAwardModel> Awards { get; set; }
        public override string ToString()
        {
            return WikiUrl;
        }
    }

    class OscarAwardModel
    {
        public OscarAwardModel()
        {
            Wins = new List<List<NameItem>>();
            Nominations = new List<List<NameItem>>();
        }

        public string Topic { get; set; }
        public List<List<NameItem>> Wins { get; set; }
        public List<List<NameItem>> Nominations { get; set; }
    }

    class NameItem
    {
        public NameItem()
        {
            IsKeyItem = true;
        }

        public bool IsKeyItem { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as NameItem);
        }

        public bool Equals(NameItem obj)
        {
            return obj != null && obj.Key == this.Key;
        }
    }
}
