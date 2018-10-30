using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeleniumTest.Cinemasight
{
    public class CinemaSightOscarExtract
    {
        public static string BASE_URL = "http://www.cinemasight.com/awards-history/";

        SeleniumService driver;

        List<string> yearUrls = new List<string>();
        List<string> yearTitle = new List<string>();

        class DataBlock
        {
            public string year;
            public Dictionary<string, List<Tuple<bool, string, string>>> CategoryItems =
                new Dictionary<string, List<Tuple<bool, string, string>>>();
        }

        DataBlock dataBlock = new DataBlock();

        public CinemaSightOscarExtract(SeleniumService driver)
        {
            this.driver = driver;
        }

        public void Run()
        {
            driver.Navigate(BASE_URL);

            // Get all the categories
            var academyYearTables = driver.ByXpaths(@"//article[@id='post-52712']/div[contains(@class,'entry-content')]/table");

            for (int i = 0; i < 2; i++)
            {
                var academyYearTableaList = academyYearTables[i].ByXpaths(@".//tr/td//a");

                for (int j = 0; j < academyYearTableaList.Count; j++)
                {
                    var hrefElement = academyYearTableaList[j];
                    yearUrls.Add(hrefElement.GetAttribute("href"));
                    yearTitle.Add(academyYearTableaList[j].Text);
                }
            }

            using (var textWriter = (TextWriter)new StreamWriter("cinemasight.csv"))
            using (var ceremonyWriter = (TextWriter)new StreamWriter("cinemasight_ceremony.csv"))
            {
                textWriter.WriteLine("Year,Category,Winner,Key,Subkey");
                for (int i = 1; i < yearUrls.Count; i++)
                {
                    var url = yearUrls[i];
                    dataBlock.year = yearTitle[i];
                    driver.Navigate(url);

                    var yearSpecificCategories = driver.ByXpaths(@"//article[@id='post-95150']/div[contains(@class,'entry-content')]/table//tr/td[2]/p");

                    var urlAwardsWinners = yearSpecificCategories[0].ByXpath(".//a").GetAttribute("href");
                    var urlCeremony = yearSpecificCategories[1].ByXpath(".//a").GetAttribute("href");

                    driver.Navigate(urlAwardsWinners);
                    ParseAwardWinners(textWriter);

                    driver.Navigate(urlCeremony);
                    ParseCeremony(ceremonyWriter);
                }
            }
        }

        private void ParseCeremony(TextWriter ceremonyWriter)
        {
            var contentsSections = driver.ByXpaths(@"//article[@id='post-108535']/div[contains(@class,'entry-content')]/*");

            var header = "";
            for (int i = 1; i < contentsSections.Count; i++)
            {
                var item = contentsSections[i];

                if (item.TagName == "div") break;

                if (item.TagName == "h1")
                {
                    WriteCeremony(ceremonyWriter);
                    header = contentsSections[i++].Text;
                }
                                
                var categoryContents = contentsSections[i];

                if (categoryContents.TagName == "p")
                {
                    ScanCategory(header, categoryContents.GetAttribute("innerHTML"));
                }
            }
        }

        private void ScanCategory(string header, string innerHTML)
        {
            var categoryItemsGroup = innerHTML.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var categoryItemsList = new List<Tuple<bool, string, string>>();

            for (int i = 0; i < categoryItemsGroup.Count; i++)
            {
                var nodeText = categoryItemsGroup[i].Trim();
                var hap = new HtmlDocument();
                hap.LoadHtml(nodeText);

                categoryItemsList.Add(new Tuple<bool, string, string>(
                        true, hap.DocumentNode.ChildNodes[0].InnerText,
                        hap.DocumentNode.ChildNodes[1].InnerText
                        ));
            }

            dataBlock.CategoryItems.Add(header, categoryItemsList);
        }

        private void WriteCeremony(TextWriter ceremonyWriter)
        {
            foreach (var kv in dataBlock.CategoryItems)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    var item = kv.Value[i];
                    ceremonyWriter.WriteLine($"{dataBlock.year},{kv.Key},{item.Item1},\"{item.Item2}\",\"{item.Item3}\"");
                }
            }

            dataBlock.CategoryItems = new Dictionary<string, List<Tuple<bool, string, string>>>();
        }

        private void ParseAwardWinners(TextWriter textWriter)
        {
            var contentsSections = driver.ByXpaths(@"//article[@id='post-108458']/div[contains(@class,'entry-content')]/*");

            for (int i = 4; i < contentsSections.Count; i++)
            {
                if (contentsSections[i].TagName == "div") break;

                var header = contentsSections[i++].Text;
                var categoryContents = contentsSections[i];

                Scan(header, categoryContents.GetAttribute("innerHTML"));
            }

            WriteToFile(textWriter);
        }

        private void Scan(string header, string innerHTML)
        {
            var categoryItemsGroup = innerHTML.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var categoryItemsList = new List<Tuple<bool, string, string>>();
            for (int i = 0; i < categoryItemsGroup.Count; i++)
            {
                var nodeText = categoryItemsGroup[i].Trim();
                var hap = new HtmlDocument();
                hap.LoadHtml(nodeText);

                if (hap.DocumentNode.ChildNodes[0].Name == "span")
                {
                    var rootNode = hap.DocumentNode.ChildNodes[0];
                    categoryItemsList.Add(new Tuple<bool, string, string>(
                        true, rootNode.ChildNodes[0].InnerText,
                        rootNode.ChildNodes[1].InnerText
                        ));
                }
                else
                {
                    categoryItemsList.Add(new Tuple<bool, string, string>(
                        true, hap.DocumentNode.ChildNodes[0].InnerText,
                        hap.DocumentNode.ChildNodes[1].InnerText
                        ));
                }
            }

            dataBlock.CategoryItems.Add(header, categoryItemsList);
        }

        private void WriteToFile(TextWriter textWriter)
        {
            foreach (var kv in dataBlock.CategoryItems)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    var item = kv.Value[i];
                    textWriter.WriteLine($"{dataBlock.year},{kv.Key},{item.Item1},\"{item.Item2}\",\"{item.Item3}\"");
                }
            }

            dataBlock.CategoryItems = new Dictionary<string, List<Tuple<bool, string, string>>>();
        }
    }
}
