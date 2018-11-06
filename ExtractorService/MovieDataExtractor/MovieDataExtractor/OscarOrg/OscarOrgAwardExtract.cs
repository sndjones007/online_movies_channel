using log4net;
using MovieDataExtractor.Base;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace MovieDataExtractor.OscarOrg
{
    public class OscarOrgAwardExtract : ExtractDataFromWebsite
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The config for the extraction of the data
        /// </summary>
        OscarOrgExtractorSettings config;

        /// <summary>
        /// The cache to hold the temporary parsed data
        /// </summary>
        TempModelForExtract cacheExtract;

        /// <summary>
        /// The file which contains all the awards details of the oscar awards
        /// </summary>
        TextWriter awardWriter;

        /// <summary>
        /// The file which contains all the Memorable moments pics of the oscar awards
        /// </summary>
        TextWriter memorableMomentsWriter;

        /// <summary>
        /// The file which contains all the awards pics of the oscar awards
        /// </summary>
        TextWriter awardsPicsWriter;

        /// <summary>
        /// The file which contains all the awards metadata of the oscar awards
        /// </summary>
        TextWriter awardsMetadataWriter;

        /// <summary>
        /// The current url for the oscar year with awards informations, metadata, pictures
        /// </summary>
        string oscarAwardsUrl;

        /// <summary>
        /// The current url for moments webpage for the oscar year
        /// </summary>
        string momentsOfOscarUrl;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="config"></param>
        public OscarOrgAwardExtract(SeleniumService driver)
            : base(driver)
        {
            config = OscarOrgExtractorSettings.Default;
            cacheExtract = new TempModelForExtract();
        }

        /// <summary>
        /// The public method to run the extractor process
        /// </summary>
        public void Run()
        {
            // Write to file
            using (awardWriter = new StreamWriter(config.FileNameAwards))
            using (memorableMomentsWriter = new StreamWriter(config.FileNameMemorableMoments))
            using (awardsPicsWriter = new StreamWriter(config.FileNameAwardsPics))
            using (awardsMetadataWriter = new StreamWriter(config.FileNameMetadata))
            {
                awardWriter.WriteLine(config.ColumnNameAwards);
                memorableMomentsWriter.WriteLine(config.ColumNameMemorableMoments);
                awardsPicsWriter.WriteLine(config.ColumnNameAwardsPics);
                awardsMetadataWriter.WriteLine(config.ColumnNameMetadata);

                for (int year = config.ProcessStartYearOfOscar; year <= config.ProcessEndYearOfOscar; year++)
                {
                    logger.InfoFormat("Start processing for year {0}", year);
                    cacheExtract.InitYearModel();
                    cacheExtract.YearModel.Year = year;

                    ProcessYearToExtractAwardsAndWriteToFile();
                    ProcessYearToExtractHighlightPicturesAndWriteToFile();
                    ProcessYearToExtractMetadataAndWriteToFile();
                    ProcessYearToExtractMemorableMomentsAndWriteToFile();

                    logger.InfoFormat("End processing for year {0}", year);
                }
            }
        }

        /// <summary>
        /// Process the oscar award year data and write to file
        /// </summary>
        private void ProcessYearToExtractAwardsAndWriteToFile()
        {
            oscarAwardsUrl = string.Format(config.BaseUrl, cacheExtract.YearModel.Year);
            driver.Navigate(oscarAwardsUrl);

            // Xpath to extract the 'view-grouping-header' and 'view-grouping-content' pair of
            // div elements
            var groupDivElements = driver.ByXpaths(config.XpathAwardsCategoryYearSpecific);

            for (int i = 0; i < groupDivElements.Count; i++)
            {
                // Initialize each category storage model
                cacheExtract.YearModel.InitCategoryModel();

                var headerElement = groupDivElements[i++];
                var headerElementClass = headerElement.GetAttribute("class");
                if (headerElementClass != "view-grouping-header")
                {
                    logger.ErrorFormat("Expected to be in pair of div[class='view-grouping-header'], div[class='view-grouping-content'] so first element is div[class='view-grouping-header'] always",
                        headerElement);
                    logger.ErrorFormat("First element found is {0} with class {1}", headerElement.TagName, headerElementClass);
                    throw new Exception("Unknown ELement found. See log file");
                }

                cacheExtract.YearModel.CategoryHeader = headerElement.Text;

                ParseAwardCategoryItemsAndSave(groupDivElements[i]);
            }
        }

        /// <summary>
        /// Parse the Oscar awards each category nodes. The format as we see is like this:
        /// 1. h3 header with 'Winner' text
        /// 2. All Winners list in div element
        /// 3. h3 header with 'Nominations' text
        /// 4. All Winners list in div element
        /// </summary>
        /// <param name="webElement"></param>
        private void ParseAwardCategoryItemsAndSave(IWebElement webElement)
        {
            logger.InfoFormat("Parsing Award category {0}", cacheExtract.YearModel.CategoryHeader);

            var headerSecondElement = webElement.ByXpath(config.XpathNominationHeader);
            var winnersElements = webElement.ByXpaths(
                string.Format(config.XpathWinningElements, config.XpathNominationHeader));
            var nominationsElements = webElement.ByXpaths(
                string.Format(config.XpathNominationElements, config.XpathNominationHeader));

            ParseSpecificAwardCategoryItems(winnersElements, true, "winner");
            ParseSpecificAwardCategoryItems(nominationsElements, false, "nomination");

            WriteAwardsByCategoryBlock();
        }

        /// <summary>
        /// Parse the winners and nominations seperately
        /// </summary>
        /// <param name="categoryTypeElements"></param>
        /// <param name="isWinner"></param>
        /// <param name="categoryType"></param>
        private void ParseSpecificAwardCategoryItems(ReadOnlyCollection<IWebElement> categoryTypeElements,
            bool isWinner, string categoryType)
        {
            for (int i = 0; i < categoryTypeElements.Count; i++)
            {
                var elemDiv = categoryTypeElements[i];
                if (elemDiv.TagName == "h3") continue;
                else if (elemDiv.TagName != "div")
                {
                    logger.ErrorFormat("Expected find a div/h3 element but found {0}", elemDiv.TagName);
                    logger.Error(elemDiv.GetAttribute("outerHTML"));
                    throw new Exception("Unknown ELement found. See log file");
                }

                var keyItemElement = elemDiv.ByXpath(config.XpathFieldKeyName);
                var valueItemElement = elemDiv.ByXpath(config.XpathFieldKeyValue);

                cacheExtract.YearModel.AddCategoryItem(Helper_TrimData(keyItemElement.Text),
                    Helper_TrimData(valueItemElement.Text), isWinner);
            }

            if(!isWinner)
                logger.InfoFormat("Successfully finished parsing the {0}", categoryType);
        }

        /// <summary>
        /// Method to write category block
        /// </summary>
        private void WriteAwardsByCategoryBlock()
        {
            logger.InfoFormat("Update awards file for {0}", cacheExtract.YearModel.CategoryHeader);

            foreach (var categoryItem in cacheExtract.YearModel.CategoryItems)
                awardWriter.WriteLine($"{cacheExtract.YearModel.Year},\"{cacheExtract.YearModel.CategoryHeader}\",{categoryItem.Item1},\"{categoryItem.Item2}\",\"{categoryItem.Item3}\"");

            awardWriter.Flush();
        }

        /// <summary>
        /// Process to parse the awards picturws for a year and save to file
        /// </summary>
        private void ProcessYearToExtractHighlightPicturesAndWriteToFile()
        {
            // Xpath to extract the link elements to get the pictures link
            var divGroupLinkElements = driver.ByXpaths(config.XpathOscarHighlightPics);

            cacheExtract.YearModel.InitHighlightsPictureModel();
            for (int i = 0; i < divGroupLinkElements.Count; i++)
            {
                var divElement = divGroupLinkElements[i];

                var captionElement = divElement.ByXpath(config.XpathPicsCaption);
                var linkElement = divElement.ByXpath(config.XpathPicsLink);
                var captionTitle = "";
                if (captionElement != null) captionTitle = Helper_TrimDataWithNewLines(captionElement.Text);

                var title = linkElement.GetAttribute("title");
                if(string.IsNullOrWhiteSpace(title)) title = linkElement.GetAttribute("data-title");

                cacheExtract.YearModel.AddHighlightPic(
                    captionTitle,
                    Helper_TrimDataWithNewLines(title),
                    Helper_TrimDataWithNewLines(linkElement.GetAttribute("data-description")),
                    Helper_TrimDataWithNewLines(linkElement.GetAttribute("data-youtube")),
                    Helper_TrimDataWithNewLines(linkElement.GetAttribute("href")));
            }

            WriteHighlightPics();
        }

        /// <summary>
        /// Write to the highlights picture details
        /// </summary>
        private void WriteHighlightPics()
        {
            logger.InfoFormat("Write the pictures details for year {0}", cacheExtract.YearModel.Year);
            for (int i = 0; i < cacheExtract.YearModel.HighlightPictures.Count; i++)
            {
                var pictureItem = cacheExtract.YearModel.HighlightPictures[i];
                awardsPicsWriter.WriteLine($"{cacheExtract.YearModel.Year},\"{pictureItem.Item1}\",\"{pictureItem.Item2}\",\"{pictureItem.Item3}\",\"{pictureItem.Item4}\",\"{pictureItem.Item5}\"");
            }

            awardsPicsWriter.Flush();
        }

        /// <summary>
        /// Prcoess the metdata information for the location and dates and save
        /// </summary>
        private void ProcessYearToExtractMetadataAndWriteToFile()
        {
            logger.InfoFormat("Write the metadata details for year {0}", cacheExtract.YearModel.Year);

            awardsMetadataWriter.WriteLine($"{cacheExtract.YearModel.Year},\"{Helper_TrimDataWithNewLines(driver.ByXpath(config.XpathMetadataOscarTitle).Text)}\",\"{Helper_TrimDataWithNewLines(driver.ByXpath(config.XpathMetadataOscarDate).Text)}\",\"{Helper_TrimDataWithNewLines(driver.ByXpath(config.XpathMetadataOscarHonoringDate).Text)}\"");

            awardsMetadataWriter.Flush();
        }

        /// <summary>
        /// Process the moments of the oscar and save to file
        /// </summary>
        private void ProcessYearToExtractMemorableMomentsAndWriteToFile()
        {
            momentsOfOscarUrl = oscarAwardsUrl + config.MomentsRelativeUrl;
            driver.Navigate(momentsOfOscarUrl);

            ProcessMainPictureAndSave();
            ProcessMomentsAndSave();
        }

        /// <summary>
        /// The moments of oscar contaisna picture at the start. Save the picture
        /// </summary>
        private void ProcessMainPictureAndSave()
        {
            var imgElement = driver.ByXpath(config.XpathMomentPictureImg);

            if (imgElement == null) return;

            awardsPicsWriter.WriteLine($"{cacheExtract.YearModel.Year},,,,,,{imgElement.GetAttribute("src")}");
            awardsPicsWriter.Flush();
        }

        /// <summary>
        /// Process of extracting the moments of oscar for the year and save to file
        /// </summary>
        private void ProcessMomentsAndSave()
        {
            var momentsElements = driver.ByXpaths(config.XpathMomentDiv);

            cacheExtract.YearModel.InitMomentsModel();

            var paragraph = "";
            for (int i = 0; i < momentsElements.Count; i++)
            {
                var currentElement = momentsElements[i];
                if (currentElement.TagName == "hr")
                {
                    cacheExtract.YearModel.Moments.Add(paragraph);
                    paragraph = "";
                }
                else
                {
                    var trimData = Helper_TrimDataWithNewLines(currentElement.Text);
                    if (string.IsNullOrEmpty(paragraph)) paragraph = trimData;
                    else paragraph += "-" + trimData;
                }
            }

            if(!string.IsNullOrWhiteSpace(paragraph))
                cacheExtract.YearModel.Moments.Add(paragraph);

            WriteMomentsDataForYear();
        }

        /// <summary>
        /// Write the data into the file
        /// </summary>
        private void WriteMomentsDataForYear()
        {
            for (int i = 0; i < cacheExtract.YearModel.Moments.Count; i++)
                memorableMomentsWriter.WriteLine($"{cacheExtract.YearModel.Year},\"{cacheExtract.YearModel.Moments[i]}\"");

            memorableMomentsWriter.Flush();
        }
    }
}
