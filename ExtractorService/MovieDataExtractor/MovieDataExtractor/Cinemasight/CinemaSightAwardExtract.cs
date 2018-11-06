using HtmlAgilityPack;
using log4net;
using MovieDataExtractor.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MovieDataExtractor.Cinemasight
{
    /// <summary>
    /// Download and save data from the cinemasight websites
    /// </summary>
    public class CinemaSightAwardExtract : ExtractDataFromWebsite
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The list of unwanted strings
        /// </summary>
        private string[] fixed_unwantedStrings = new string[] { "to", "and", "," };

        /// <summary>
        /// The list of unwanted strings
        /// </summary>
        private string[] fixed_keyElements = new string[] { "em", "span", "strong" };

        /// <summary>
        /// The config for the extraction of the data
        /// </summary>
        CinemaSightExtractorSettings config;

        /// <summary>
        /// The cache to hold the temporary parsed data
        /// </summary>
        TempModelForExtract cacheExtract;

        /// <summary>
        /// The file which contains all the awards details of the oscar awards
        /// </summary>
        TextWriter awardWriter;

        /// <summary>
        /// The file which contains all the ceremony details of the oscar awards
        /// </summary>
        TextWriter ceremonyWriter;

        /// <summary>
        /// The file which contains all the moments details of the oscar awards
        /// </summary>
        TextWriter momentsWriter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="config"></param>
        public CinemaSightAwardExtract(SeleniumService driver)
            :base(driver)
        {
            config = CinemaSightExtractorSettings.Default;
            cacheExtract = new TempModelForExtract();
        }

        /// <summary>
        /// Execute the data extract
        /// </summary>
        public void Run()
        {
            if (config.SkipProcessing) return;

            // Navigate to the award history index url to gather the index data (url, name)
            logger.Info("Navigate to the award history index url to gather the index data (url, name)");
            driver.Navigate(config.BaseUrl);

            // Read the indices for oscar awards (year and name)
            ExtractAwardHistoryIndex();

            // Process the index data one by one and extract data for each year
            ProcessEachIndexToExtractDataAndWriteToFile();
        }

        /// <summary>
        /// Extract the indices for every year of oscar awards
        /// </summary>
        private void ExtractAwardHistoryIndex()
        {
            logger.InfoFormat("Extract the index data (url, name). Uses the Xpath {0}",
                config.XpathAwardIndexExtract);

            // Get all the categories
            var sd_academyChronologicalLinkElements = driver.ByXpaths(config.XpathAwardIndexExtract);

            for (int i = 0; i < sd_academyChronologicalLinkElements.Count; i++)
            {
                var hrefElement = sd_academyChronologicalLinkElements[i];
                cacheExtract.AddIndex(
                    hrefElement.GetAttribute("href"), 
                    Convert.ToInt32(hrefElement.Text.Remove(hrefElement.Text.Length - 2)));
            }
        }

        /// <summary>
        /// Process the index data one by one and extract data for each year
        /// Save the data for every year into the file.
        /// </summary>
        private void ProcessEachIndexToExtractDataAndWriteToFile()
        {
            using (awardWriter = new StreamWriter(config.FileNameAwards))
            using (ceremonyWriter = new StreamWriter(config.FileNameCeremonyDetails))
            using (momentsWriter = new StreamWriter(config.FileNameMoments))
            {
                awardWriter.WriteLine(config.ColumnNamesAwards);
                ceremonyWriter.WriteLine(config.ColumnNamesCeremony);
                momentsWriter.WriteLine(config.ColumnNamesMoments);

                logger.Info("Created the output files for saving awards, ceremony details");

                if (string.IsNullOrWhiteSpace(config.ProcessSpecificOscarYear))
                    cacheExtract.AwardHistoryIndices.ForEach(ProcessEachHistoryIndex);
                else
                {
                    var awardConfigIndices = CreateAwardIndexRange(config.ProcessSpecificOscarYear);

                    for (int i = 0; i < awardConfigIndices.Count; i++)
                    {
                        var awardConfigIndex = awardConfigIndices[i];
                        var awardIndex = cacheExtract.AwardHistoryIndices.Where(
                        x => x.Item2 == awardConfigIndex);
                        if (awardIndex != null && awardIndex.Count() > 0) ProcessEachHistoryIndex(awardIndex.First());
                    }
                }
            }
        }

        /// <summary>
        /// Create a range of year indices
        /// </summary>
        /// <param name="yearIndex"></param>
        /// <returns></returns>
        private List<int> CreateAwardIndexRange(string yearIndex)
        {
            var yearIndices = yearIndex.Split(',');
            var result = new List<int>();

            for (int i = 0; i < yearIndices.Length; i++)
            {
                if(yearIndices[i].Contains("-"))
                {
                    var splitRange = yearIndices[i].Split('-');
                    var start = Convert.ToInt32(splitRange[0]);
                    var end = Convert.ToInt32(splitRange[1]);
                    result.AddRange(Enumerable.Range(start, end - start).ToList());
                }
                else
                {
                    result.Add(Convert.ToInt32(yearIndices[i]));
                }
            }

            return result;
        }

        /// <summary>
        /// The method which process a award year at a time.
        /// It extracts data and saves into files
        /// </summary>
        private void ProcessEachHistoryIndex(Tuple<string, int> awardIndex)
        {
            cacheExtract.InitYearModel();

            cacheExtract.YearModel.OscarIndex = awardIndex.Item2;
            driver.Navigate(awardIndex.Item1);

            var awardYearIndexForAwardsAndCeremonies = driver.ByXpaths(config.XpathYearSpecificIndices);

            var urlAwardsWinners = awardYearIndexForAwardsAndCeremonies[0].GetAttribute("href");
            var urlCeremony = awardYearIndexForAwardsAndCeremonies[1].GetAttribute("href");

            logger.InfoFormat("Awards Url: {0}, Ceremony Url: {1} for year {2}", urlAwardsWinners,
                urlCeremony, cacheExtract.YearModel.OscarIndex);

            ProcessYearSpecificDataAndSave(urlAwardsWinners, WriteAwardsByCategoryBlock, 
                config.XpathAwardsYearSpecificCategoriesGroup, "h2", "award");
            ProcessYearSpecificDataAndSave(urlCeremony, WriteCeremonyByCategoryBlock, 
                config.XpathCeremoniesYearSpecific, "h1", "ceremony");
        }

        /// <summary>
        /// Process the year specific oscar awards url and save to file
        /// </summary>
        /// <param name="urlAwardsWinners"></param>
        private void ProcessYearSpecificDataAndSave(string urlAwardsWinners, Action writeToFile,
            string xpathGroup, string headerElement, string typeOfPage)
        {
            logger.InfoFormat("Start parsing {0} categories", typeOfPage);
            driver.Navigate(urlAwardsWinners);

            // Extracts all the child elements
            var contentsSections = driver.ByXpaths(xpathGroup);

            // Expected to be in pair of h2,p, so first element is h2 always
            for (int i = 0; i < contentsSections.Count; i++)
            {
                // Initialize each category storage model
                cacheExtract.YearModel.InitCategoryModel();

                if (contentsSections[i].TagName != headerElement)
                {
                    logger.ErrorFormat("Expected to be in pair of {0},p, so first element is {0} always",
                        headerElement);
                    logger.ErrorFormat("Element found is {0}", contentsSections[i].GetAttribute("outerHTML"));

                    continue;
                }

                cacheExtract.YearModel.CategoryHeader = contentsSections[i++].Text;
                var categoryContents = contentsSections[i];

                if(categoryContents.TagName == "p")
                {
                    Scan(categoryContents.GetAttribute("innerHTML"));
                    writeToFile();
                }
            }

            ProcessNotesIfPresentAndSave();

            logger.InfoFormat("End parsing {0} categories", typeOfPage);
        }

        /// <summary>
        /// The Contents of the paragraph element of a awards category contains a series of
        /// text and elements
        /// </summary>
        /// <param name="header"></param>
        /// <param name="innerHTML"></param>
        private void Scan(string innerHTML)
        {
            logger.InfoFormat("Start parsing sepcific award category {0}", 
                cacheExtract.YearModel.CategoryHeader);

            var categoryItemsGroup = Helper_SplitNoEmptyEntries(innerHTML, "<br>");

            if (cacheExtract.YearModel.CategoryHeader.Contains("HONORARY") ||
                    cacheExtract.YearModel.CategoryHeader.Contains("SCIENTIFIC"))
            {
                foreach (var item in categoryItemsGroup)
                {
                    var nodeText = Helper_TrimDataWithNewLines(item);
                    nodeText = Regex.Replace(nodeText, @"\s{2,}", " ");
                    cacheExtract.YearModel.AddCategoryItem("", nodeText, true);
                }
            }
            else
            {
                for (int i = 0; i < categoryItemsGroup.Count; i++)
                {
                    var nodeText = Helper_TrimDataWithNewLines(categoryItemsGroup[i]);
                    var rootNode = Helper_HtmlParse(nodeText);
                    var isWinner = false;

                    // If node contains Note then remove
                    if (rootNode.ChildNodes.Count > 0) RemoveNoteChildNode(rootNode);

                    // If it contains span element then it is winner node
                    if (rootNode.ChildNodes.Count == 1 && rootNode.ChildNodes[0].Name == "span")
                    {
                        isWinner = true;
                        rootNode = rootNode.ChildNodes[0];
                    }

                    var keyName = "";
                    var keyValue = "";

                    foreach (var node in rootNode.ChildNodes)
                    {
                        var nodeValue = Helper_TrimData(node.InnerText);
                        if (node.Name == "#text")
                        {
                            if (fixed_unwantedStrings.Contains(nodeValue, StringComparer.OrdinalIgnoreCase)) continue;
                            if (string.IsNullOrEmpty(keyValue)) keyValue = nodeValue;
                            else keyValue += "," + nodeValue;
                        }
                        else if (fixed_keyElements.Contains(node.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(keyName)) keyName = nodeValue;
                            else keyName += "," + nodeValue;
                        }
                    }

                    if(!string.IsNullOrWhiteSpace(keyName) && !string.IsNullOrWhiteSpace(keyValue))
                        cacheExtract.YearModel.AddCategoryItem(keyName, keyValue, isWinner);
                }
            }

            logger.Info("End parsing sepcific award category");
        }

        /// <summary>
        /// Remove any node like <a href="#Notes"><span class="NominationNote">[1]</span></a>
        /// </summary>
        /// <param name="rootNode"></param>
        private void RemoveNoteChildNode(HtmlNode rootNode)
        {
            for (int i = 0; i < rootNode.ChildNodes.Count(); i++)
            {
                var childNode = rootNode.ChildNodes[i];

                if (childNode.Name == "a" || (childNode.Name == "#text" && string.IsNullOrWhiteSpace(childNode.InnerText)))
                {
                    rootNode.ChildNodes.Remove(childNode);
                    i = i - 1;
                }
            }
        }

        /// <summary>
        /// Method to write category block
        /// </summary>
        private void WriteAwardsByCategoryBlock()
        {
            logger.InfoFormat("Update awards file for {0}", cacheExtract.YearModel.CategoryHeader);

            foreach (var categoryItem in cacheExtract.YearModel.CategoryItems)
                awardWriter.WriteLine($"{cacheExtract.YearModel.OscarIndex},\"{cacheExtract.YearModel.CategoryHeader}\",{categoryItem.Item1},\"{categoryItem.Item2}\",\"{categoryItem.Item3}\"");

            awardWriter.Flush();
        }

        /// <summary>
        /// Write to the ceremony file
        /// </summary>
        private void WriteCeremonyByCategoryBlock()
        {
            logger.InfoFormat("Update ceremony file for {0}", cacheExtract.YearModel.CategoryHeader);

            foreach (var categoryItem in cacheExtract.YearModel.CategoryItems)
                ceremonyWriter.WriteLine($"{cacheExtract.YearModel.OscarIndex},\"{cacheExtract.YearModel.CategoryHeader}\",\"{categoryItem.Item2}\",\"{categoryItem.Item3}\"");

            ceremonyWriter.Flush();
        }

        /// <summary>
        /// Process the notes if present
        /// </summary>
        private void ProcessNotesIfPresentAndSave()
        {
            var notesElement = driver.ByXpaths(config.XpathAwardsNotes);

            if (notesElement != null)
            {
                foreach (var noteElement in notesElement)
                {
                    // Initialize each category storage model
                    cacheExtract.YearModel.InitCategoryModel();

                    cacheExtract.YearModel.CategoryHeader = "NOTE";

                    Scan(noteElement.GetAttribute("innerHTML"));
                    writeNoteToFile();
                }
            }
        }

        /// <summary>
        /// Write to notes
        /// </summary>
        private void writeNoteToFile()
        {
            logger.InfoFormat("Update Notes file for {0}", cacheExtract.YearModel.CategoryHeader);

            foreach (var categoryItem in cacheExtract.YearModel.CategoryItems)
                momentsWriter.WriteLine($"{cacheExtract.YearModel.OscarIndex},\"{cacheExtract.YearModel.CategoryHeader}\",\"{categoryItem.Item2}\",\"{categoryItem.Item3}\"");

            momentsWriter.Flush();
        }
    }
}
