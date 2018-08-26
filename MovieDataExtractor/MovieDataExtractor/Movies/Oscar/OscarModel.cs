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

    class AwardItem
    {
        public string UrlKey1 { get; set; }
        public string Key1 { get; set; }
        public string UrlKey2 { get; set; }
        public string Key2 { get; set; }
        public string Type1 { get; set; }
        public string Url1 { get; set; }
        public string Value1 { get; set; }
        public string Type2 { get; set; }
        public string Url2 { get; set; }
        public string Value2 { get; set; }
        public string Type3 { get; set; }
        public string Url3 { get; set; }
        public string Value3 { get; set; }
        public string Type4 { get; set; }
        public string Url4 { get; set; }
        public string Value4 { get; set; }
        public string Type5 { get; set; }
        public string Url5 { get; set; }
        public string Value5 { get; set; }
        public string Type6 { get; set; }
        public string Url6 { get; set; }
        public string Value6 { get; set; }
    }
}
