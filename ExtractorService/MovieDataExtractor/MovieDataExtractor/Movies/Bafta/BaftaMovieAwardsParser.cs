using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest.Movies.Bafta
{
    class BaftaMovieAwardsParser
    {
        SeleniumService driver;
        List<string> PageUrls;
        List<string> AwardTypeButtons;
        string currentUrl;

        BaftaAward baftaAward;

        public BaftaMovieAwardsParser(SeleniumService driver)
        {
            this.driver = driver;
        }

        public void Run()
        {
            ExtractPageUrls();

            for (int i = 0; i < PageUrls.Count; i++)
            {
                currentUrl = PageUrls[i];

                driver.Navigate(currentUrl);

                baftaAward = new BaftaAward();
                ExtractAwardTypeButtons();
                NavigateAwardTypes();
            }
        }

        private void NavigateAwardTypes()
        {
            for (int i = 0; i < AwardTypeButtons.Count; i++)
            {
                driver.Navigate(currentUrl);
                var buttonElement = driver.ByXpath(@"//*[@id='explore-page-awards-type-form']/button[@value='{AwardTypeButtons[i]}'");
                buttonElement.Click();
                var goButton = driver.ByXpath(@"//*[@id='explore-page-awards-type-submit']/input");
                goButton.Click();

                ExtractCategoryData();
            }
        }

        private void ExtractCategoryData()
        {
            var listElements = driver.ByXpaths(@"//div[@class='view-content']/ul/li");

            foreach (var awardGroup in listElements)
            {
                var categoryBafta = new BaftaCategory();
                categoryBafta.Category = awardGroup.ByXpath(@"./div[@class='search-result-title']/h2/a").Text;
                categoryBafta.Category = categoryBafta.Category.Replace("Film | ", "").Trim();

                categoryBafta.Nominations = new List<BaftaAwardItem>();
                categoryBafta.Wins = new List<BaftaAwardItem>();
                var categoryResultGroupElements = 
                    awardGroup.ByXpaths(@"./div[@class='search-result-group']/div[@class='search-result-nomination']/div");
                for (int i = 0; i < categoryResultGroupElements.Count; i++)
                {
                    var resultElement = categoryResultGroupElements[i];
                    var awardItem = new BaftaAwardItem();

                    var expectedHeadline = resultElement.GetAttribute("class");
                    if (expectedHeadline.Contains("headline"))
                        awardItem.Key = resultElement.ByXpath("./p").Text.Trim();
                    bool isWinItem = expectedHeadline.Contains("winner");

                    i++;
                    resultElement = categoryResultGroupElements[i];
                    var expectedSubtitle = resultElement.GetAttribute("class");
                    if (expectedSubtitle.Contains("subtitle"))
                    {
                        var item = resultElement.ByXpath("./p");

                        if(item != null)
                            awardItem.Value = item.Text.Trim().Split(',').ToList();
                    }

                    if (isWinItem)
                        categoryBafta.Wins.Add(awardItem);
                    else
                        categoryBafta.Nominations.Add(awardItem);
                }
            }
        }

        private void ExtractAwardTypeButtons()
        {
            var formButtonsElements = driver.ByXpaths(@"//*[@id='explore-page-awards-type-form']/button");
            AwardTypeButtons = new List<string>();

            foreach (var buttonElement in formButtonsElements)
                AwardTypeButtons.Add(buttonElement.GetAttribute("value"));
        }

        private void ExtractPageUrls()
        {
            PageUrls = new List<string>();

            for (int i = 1949; i < 2019; i++)
                PageUrls.Add($"http://awards.bafta.org/explore?year={i}");
        }
    }
}
