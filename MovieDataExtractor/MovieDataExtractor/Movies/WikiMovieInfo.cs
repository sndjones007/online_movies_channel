using HtmlAgilityPack;
using OpenQA.Selenium;
using SeleniumTest.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies
{
    class WikiMovieInfo
    {
        const string pageUrl = "https://en.wikipedia.org/wiki/The_Last_Command_(1928_film)";

        SeleniumService driver;
        WikiMovieItem MovieItem;

        public WikiMovieInfo(SeleniumService driver)
        {
            this.driver = driver;
        }

        public void Run()
        {
            driver.Navigate(pageUrl);

            MovieItem = new WikiMovieItem();
            ExtractMovieInfo();

            MovieItem.Castings = new List<WikiCast>();
            ExtractCastInfo();

            SaveCsv();
        }

        private void ExtractCastInfo()
        {
            var sectionElements = driver.ByXpaths("//h2[contains(string(),'Cast')]/following-sibling::ul[1]/li");

            foreach (var item in sectionElements)
            {
                var castItem = new WikiCast();
                var itemSplit = item.Text.Split(new string[] { "as" }, StringSplitOptions.None);
                MovieItem.Castings.Add(new WikiCast()
                {
                    Name = itemSplit[0].Trim(),
                    CharacterName = itemSplit[1].Trim()
                });
            }
        }

        private void ExtractMovieInfo()
        {
            var rows = driver.ByXpaths("//*[@id='mw-content-text']/div/table[1]/tbody/tr");

            // Expected movie name
            MovieItem.Name = rows[0].Text;
            MovieItem.PosterUrl = rows[1].ByXpath(".//img").GetAttribute("src");
            MovieItem.Properties = new Dictionary<string, List<WikiMoviePropertyItem>>();

            for (int i = 2; i < rows.Count; i++)
            {
                var propertyElement = rows[i];
                var keyTopic = propertyElement.ByXpath("th").Text.Replace("\r\n", " ");
                var propertyItems = new List<WikiMoviePropertyItem>();

                MovieItem.Properties.Add(keyTopic, propertyItems);
                var valueTopic = propertyElement.ByXpath("td");

                var innerHtml = valueTopic.GetAttribute("innerHTML");
                var elements = innerHtml.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim('\n', '\r'));
                
                foreach (var item in elements)
                {
                    var hap = new HtmlDocument();
                    hap.LoadHtml(item);
                    var ahref = hap.DocumentNode.SelectSingleNode("a");
                    var valref = hap.DocumentNode.SelectSingleNode(".");
                    if (ahref == null)
                        propertyItems.Add(new WikiMoviePropertyItem()
                        {
                            Value = System.Web.HttpUtility.HtmlDecode(valref.InnerText.Trim())
                        });
                    else
                    {
                        propertyItems.Add(new WikiMoviePropertyItem()
                        {
                            Url = new Uri(new Uri("https://en.wikipedia.org/"), ahref.GetAttributeValue("href", "")).AbsoluteUri,
                            Value = System.Web.HttpUtility.HtmlDecode(ahref.InnerText.Trim())
                        });
                    }
                }
            }
        }

        private void SaveCsv()
        {
            using (TextWriter writer = new StreamWriter("wikiMovie.csv"))
            {
                foreach (var awardItem in MovieItem.Properties)
                {
                    foreach (var item in awardItem.Value)
                    {
                        writer.WriteLine($"\"{MovieItem.Name}\",\"{MovieItem.PosterUrl}\",\"{awardItem.Key}\",\"{item.Value}\",\"{item.Url}\"");
                    }
                }
            }

            using (TextWriter writer = new StreamWriter("wikiMovieCast.csv"))
            {
                foreach (var awardItem in MovieItem.Castings)
                {
                    writer.WriteLine($"\"{MovieItem.Name}\",\"{awardItem.Name}\",\"{awardItem.CharacterName}\"");
                }
            }
        }
    }
}
