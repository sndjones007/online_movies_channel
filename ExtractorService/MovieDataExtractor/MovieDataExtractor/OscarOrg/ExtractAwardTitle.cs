using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SeleniumTest.OscarOrg
{
    public class ExtractAwardTitle
    {
        const string DOMAIN_URL = @"https://www.oscars.org/oscars/ceremonies/{0}";
        const int startyear = 2017;

        SeleniumService driver;
        string oscarAwardsUrl, memorableMomentsOscarAwardsUrl;

        List<Tuple<int, string, OscarOrgModel>> dictByTitle = new List<Tuple<int, string, OscarOrgModel>>();
        List<string> memorableMomentsOfAYear = new List<string>();

        // year, content, href, title, youtubeData, titleData, descData, dataFacebook, dataTwitterUrl,
        List<List<string>> picsAwards = new List<List<string>>();
        List<string> metadataAwards = new List<string>();

        public ExtractAwardTitle(SeleniumService driver)
        {
            this.driver = driver;
        }

        public void Run()
        {
            // Write to file
            using (var csvAwardsWriter = (TextWriter)new StreamWriter("outputAwards.csv"))
            using (var csvMemorableWriter = (TextWriter)new StreamWriter("outputMemorable.csv"))
            using (var csvAwardsPicsWriter = (TextWriter)new StreamWriter("outputAwardsPics.csv"))
            using (var csvAwardsMetadataWriter = (TextWriter)new StreamWriter("outputAwardsMetadata.csv"))
            {
                csvAwardsWriter.WriteLine($"Year,WinOrLose,Type,Key,Item");
                csvMemorableWriter.WriteLine("Year,Moments");
                csvAwardsPicsWriter.WriteLine("Year,Type,Content,Url,Title,Youtube,DataTitle,Description,OriginalUrl");
                csvAwardsMetadataWriter.WriteLine("Year,Title,Location,Date,Honoring");

                for (int year = 1929; year <= 2018; year++)
                {
                    ReadAYear(year);
                    WriteAwardsInfo(csvAwardsWriter);

                    ReadPics();
                    WritePicsInfo(csvAwardsPicsWriter, year);

                    ReadMemorablePics();
                    WriteMemorablePicsInfo(csvAwardsPicsWriter, year);

                    ReadMetadata();
                    WriteMetadataInfo(csvAwardsMetadataWriter, year);

                    ReadMemorableMomemnts();
                    WriteMemorableInfo(csvMemorableWriter, year);
                }
            }
        }

        private void WriteMetadataInfo(TextWriter csvAwardsMetadataWriter, int year)
        {
            csvAwardsMetadataWriter.Write($"{year},");

            for (int i = 0; i < metadataAwards.Count; i++)
            {
                var item = metadataAwards[i];

                if (!string.IsNullOrEmpty(item))
                {
                    csvAwardsMetadataWriter.Write($"\"{item.Trim()}\",");
                }
                else
                {
                    csvAwardsMetadataWriter.Write(",");
                }
            }
            csvAwardsMetadataWriter.WriteLine();
            metadataAwards.Clear();
        }

        private void ReadMetadata()
        {
            var contentsSection = driver.ByXpaths(@"//div[contains(@class,'view-id-awards_ceremonies')]/div[2]/div/div");

            metadataAwards.Add(contentsSection[0].Text.Trim());
            metadataAwards.Add(contentsSection[1].Text.Trim());
            metadataAwards.Add(contentsSection[2].Text.Trim());
            metadataAwards.Add(contentsSection[3].Text.Trim());
        }

        private void WriteMemorablePicsInfo(TextWriter csvAwardsPicsWriter, int year)
        {
            for (int i = 0; i < picsAwards.Count; i++)
            {
                csvAwardsPicsWriter.Write($"{year},Memorable,");
                var itemList = picsAwards[i];

                for (int j = 0; j < itemList.Count; j++)
                {
                    var item = itemList[j];
                    if (!string.IsNullOrEmpty(item))
                    {
                        csvAwardsPicsWriter.Write($"\"{item.Trim()}\",");
                    }
                    else
                    {
                        csvAwardsPicsWriter.Write(",");
                    }
                }
                csvAwardsPicsWriter.WriteLine();
            }
            picsAwards.Clear();
        }

        private void ReadMemorablePics()
        {
            // Highlights Pics
            var pictureBoxItems = driver.ByXpaths(@".//div[contains(@class,'views-field-field-memorable-moments')]/div/ul/li");

            foreach (var pictureBox in pictureBoxItems)
            {
                var pictureItem = new List<string>();

                var divPictureBox = pictureBox.ByXpath(@".//div[contains(@class,'field-collection-item-field-memorable-moments')]/div");
                pictureItem.Add("");
                var hrefElement = divPictureBox.ByXpath(@"./div[1]//a");

                pictureItem.Add(hrefElement.GetAttribute("href"));
                pictureItem.Add(hrefElement.GetAttribute("title"));
                pictureItem.Add(hrefElement.GetAttribute("data-youtube"));
                pictureItem.Add(hrefElement.GetAttribute("data-title"));
                pictureItem.Add(hrefElement.GetAttribute("data-description"));
                //var hrefDataFacebookUrlText = hrefElement.GetAttribute("data-facebook-url");
                //var hrefDataTwitterUrlText = hrefElement.GetAttribute("data-twitter-url");

                var imgBoxElement = hrefElement.ByXpath(".//img");

                pictureItem.Add(imgBoxElement.GetAttribute("data-original")); // actual url

                picsAwards.Add(pictureItem);
            }
        }

        private void WritePicsInfo(TextWriter csvAwardsPicsWriter, int year)
        {
            for (int i = 0; i < picsAwards.Count; i++)
            {
                csvAwardsPicsWriter.Write($"{year},Highlights,");
                var itemList = picsAwards[i];

                for (int j = 0; j < itemList.Count; j++)
                {
                    var item = itemList[j];
                    if (!string.IsNullOrEmpty(item))
                    {
                        csvAwardsPicsWriter.Write($"\"{item.Trim()}\",");
                    }
                    else
                    {
                        csvAwardsPicsWriter.Write(",");
                    }
                }
                csvAwardsPicsWriter.WriteLine();
            }
            
            picsAwards.Clear();
        }

        private void ReadPics()
        {
            // Highlights Pics
            var pictureBoxItems = driver.ByXpaths(@".//div[contains(@class,'views-field-field-highlights')]/div/ul/li");

            foreach (var pictureBox in pictureBoxItems)
            {
                var pictureItem = new List<string>();
                var divPictureBox = pictureBox.ByXpath(@".//div[contains(@class,'field-collection-item-field-highlights')]/div");
                pictureItem.Add(divPictureBox.ByXpath(@"div[1]").Text.Trim());
                var hrefElement = divPictureBox.ByXpath(@"div[2]//a");

                pictureItem.Add(hrefElement.GetAttribute("href"));
                pictureItem.Add(hrefElement.GetAttribute("title"));
                pictureItem.Add(hrefElement.GetAttribute("data-youtube"));
                pictureItem.Add(hrefElement.GetAttribute("data-title"));
                pictureItem.Add(hrefElement.GetAttribute("data-description"));
                //var hrefDataFacebookUrlText = hrefElement.GetAttribute("data-facebook-url");
                //var hrefDataTwitterUrlText = hrefElement.GetAttribute("data-twitter-url");

                var imgBoxElement = hrefElement.ByXpath(".//img");

                pictureItem.Add(imgBoxElement.GetAttribute("data-original")); // actual url
                picsAwards.Add(pictureItem);
            }
        }

        private void WriteMemorableInfo(TextWriter csvMemorableWriter, int year)
        {
            for (int i = 0; i < memorableMomentsOfAYear.Count; i++)
            {
                var item = memorableMomentsOfAYear[i];
                csvMemorableWriter.WriteLine($"{year},\"{item}\"");
            }
            memorableMomentsOfAYear.Clear();
        }

        private void WriteAwardsInfo(TextWriter csvAwardsWriter)
        {
            for (int i = 0; i < dictByTitle.Count; i++)
            {
                var byTitleYear = dictByTitle[i];

                for (int j = 0; j < byTitleYear.Item3.keySubkeyWinOders.Count; j++)
                {
                    var winByYearAndTitle = byTitleYear.Item3.keySubkeyWinOders[j];
                    csvAwardsWriter.WriteLine($"{byTitleYear.Item1},Winner,\"{byTitleYear.Item2}\",\"{winByYearAndTitle.Item1}\",\"{winByYearAndTitle.Item2}\"");
                }

                for (int j = 0; j < byTitleYear.Item3.keySubkeyNomineesOders.Count; j++)
                {
                    var winByYearAndTitle = byTitleYear.Item3.keySubkeyNomineesOders[j];
                    csvAwardsWriter.WriteLine($"{byTitleYear.Item1},Nominees,\"{byTitleYear.Item2}\",\"{winByYearAndTitle.Item1}\",\"{winByYearAndTitle.Item2}\"");
                }
            }
            dictByTitle.Clear();
        }

        private void ReadMemorableMomemnts()
        {
            memorableMomentsOscarAwardsUrl = oscarAwardsUrl + "/memorable-moments";
            driver.Navigate(memorableMomentsOscarAwardsUrl);

            var momentsSections = driver.ByXpaths(@"//div[contains(@class,'field--type-text-with-summary') and contains(@class,'field--label-hidden')]//div[contains(@class,'field-item') and contains(@class,'even')]/p");

            foreach (var item in momentsSections)
            {
                memorableMomentsOfAYear.Add(item.Text);
            }
        }

        private void ReadAYear(int year)
        {
            oscarAwardsUrl = string.Format(DOMAIN_URL, year);
            driver.Navigate(oscarAwardsUrl);

            var groupingCategoryElements = driver.ByXpaths(@"//*[@id='quicktabs-tabpage-honorees-0']/div/div[2]/div[@class='view-grouping']");

            foreach (var category in groupingCategoryElements)
            {
                var categoryName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    category.ByXpath(@"./div[@class='view-grouping-header']/h2").Text.ToLower());
                var groupingContentSections = category.ByXpaths(@"./div[@class='view-grouping-content']/*");

                bool isWinner = true;
                var oscarModel = new OscarOrgModel();

                foreach (var section in groupingContentSections)
                {
                    if (section.TagName == "h3")
                    {
                        if (string.Compare(section.Text, "Winner", true) != 0) isWinner = false;
                    }
                    else
                    {
                        if (section.TagName == "div")
                        {
                            var fieldActorName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                section.ByXpath("div[@class='views-field views-field-field-actor-name']").Text.Trim().ToLower());
                            var fieldFieldTitle = section.ByXpath("div[@class='views-field views-field-title']").Text.Trim();
                            var fieldEditMode = section.ByXpath("div[@class='views-field views-field-edit-node']").Text;

                            if (isWinner)
                                oscarModel.keySubkeyWinOders.Add(new Tuple<string, string, string>(
                                    fieldActorName, fieldFieldTitle, fieldEditMode
                                    ));
                            else
                                oscarModel.keySubkeyNomineesOders.Add(new Tuple<string, string, string>(
                                    fieldActorName, fieldFieldTitle, fieldEditMode
                                    ));
                        }
                    }
                }

                dictByTitle.Add(new Tuple<int, string, OscarOrgModel>(
                    year, categoryName, oscarModel
                    ));
            }
        }
    }
}
