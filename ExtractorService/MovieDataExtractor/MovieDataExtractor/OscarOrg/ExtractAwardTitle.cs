using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.OscarOrg
{
    public class ExtractAwardTitle
    {
        const string DOMAIN_URL = @"https://www.oscars.org/oscars/ceremonies/{0}";
        const int startyear = 2017;

        SeleniumService driver;
        string initialOscarAwardsUrl;

        List<Tuple<int, string, OscarOrgModel>> dictByTitle = new List<Tuple<int, string, OscarOrgModel>>();

        public ExtractAwardTitle(SeleniumService driver)
        {
            this.driver = driver;
        }

        public void Run()
        {
            for (int i = 1929; i <= 1931; i++)
            {
                initialOscarAwardsUrl = string.Format(DOMAIN_URL, i);
                driver.Navigate(initialOscarAwardsUrl);

                var groupingCategoryElements = driver.ByXpaths(@"//*[@id='quicktabs-tabpage-honorees-0']/div/div[2]/div[@class='view-grouping']");

                foreach (var category in groupingCategoryElements)
                {
                    var categoryName = category.ByXpath(@"./div[@class='view-grouping-header']/h2").Text;
                    var groupingContentSections = category.ByXpaths(@"./div[@class='view-grouping-content']/*");

                    bool isWinner = true;
                    var oscarModel = new OscarOrgModel();

                    foreach (var section in groupingContentSections)
                    {
                        if(section.TagName == "h3")
                        {
                            if (string.Compare(section.Text, "Winner", true) != 0) isWinner = false;
                        }
                        else
                        {
                            if(section.TagName == "div")
                            {
                                var fieldActorName = section.ByXpath("div[@class='views-field views-field-field-actor-name']").Text;
                                var fieldFieldTitle = section.ByXpath("div[@class='views-field views-field-title']").Text;
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
                        i, categoryName, oscarModel
                        ));
                }
            }

            // Write to file
            using (var textWriter = (TextWriter)new StreamWriter("output.csv"))
            {
                for (int i = 0; i < dictByTitle.Count; i++)
                {
                    var byTitleYear = dictByTitle[i];

                    for (int j = 0; j < byTitleYear.Item3.keySubkeyWinOders.Count; j++)
                    {
                        var winByYearAndTitle = byTitleYear.Item3.keySubkeyWinOders[j];
                        textWriter.WriteLine($"${byTitleYear.Item1},Winner, \"${byTitleYear.Item2}\",\"${winByYearAndTitle.Item1}\",\"${winByYearAndTitle.Item2}\",\"${winByYearAndTitle.Item3}\"");
                    }

                    for (int j = 0; j < byTitleYear.Item3.keySubkeyNomineesOders.Count; j++)
                    {
                        var winByYearAndTitle = byTitleYear.Item3.keySubkeyNomineesOders[j];
                        textWriter.WriteLine($"${byTitleYear.Item1},Nominees, \"${byTitleYear.Item2}\",\"${winByYearAndTitle.Item1}\",\"${winByYearAndTitle.Item2}\",\"${winByYearAndTitle.Item3}\"");
                    }
                }
            }
        }
    }
}
