using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using OpenQA.Selenium;

namespace SeleniumTest.Movies
{
    /// <summary>
    /// A class to extract oscar awards information from wikipedia
    /// </summary>
    public class WikiOscarAwards
    {
        /// <summary>
        /// The driver for browser
        /// </summary>
        SeleniumService driver;

        /// <summary>
        /// List of oscar awards urls
        /// </summary>
        List<string> PageUrls;

        /// <summary>
        /// The oscar model for a single urls
        /// </summary>
        OscarModel oscarModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public WikiOscarAwards(SeleniumService driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Execute
        /// </summary>
        public void Run()
        {
            ExtractPageUrls();

            for (int i = 0; i < PageUrls.Count; i++)
            {
                oscarModel = new OscarModel()
                {
                    Rank = i + 1,
                    WikiUrl = PageUrls[i]
                };

                Console.WriteLine(oscarModel);
                driver.Navigate(oscarModel.WikiUrl);

                ExtractMetadata();
                ExtractAwardsTable();

                SaveCsv();
            }
        }

        private void ExtractAwardsTable()
        {
            var tableRows = driver.ByXpaths("//*[@id='mw-content-text']/div/table[@class='wikitable'][1]//tr");

            for (int i = 0; i < tableRows.Count; i++)
            {
                var tablerow = tableRows[i];

                // check if contains header
                var possibleths = tablerow.ByXpaths("./th");

                if(possibleths == null || possibleths.Count <= 0)
                {
                    var possibletds = tablerow.ByXpaths("./td");
                    ExtractDataForOnlyDataRow(possibletds[0]);
                    if(possibletds.Count > 1) ExtractDataForOnlyDataRow(possibletds[1]);
                }
                else
                {
                    i++;
                    tablerow = tableRows[i];
                    var possibletds = tablerow.ByXpaths("./td");
                    ExtractDataForHeaderAndDataRow(possibleths[0], possibletds[0]);
                    if (possibleths.Count > 1) ExtractDataForHeaderAndDataRow(possibleths[1], possibletds[1]);
                }
            }
        }

        private void SaveCsv()
        {
            using (TextWriter writer = new StreamWriter($"OscarMetadata.csv", true))
            {
                int index = 0;
                foreach (var item in oscarModel.Metadata)
                {
                    foreach (var mItem in item.Value)
                    {
                        writer.WriteLine($"{oscarModel.Rank},\"{item.Key}\",{index},\"{mItem.Key}\",\"{mItem.Value}\"");
                    }
                    index++;
                }
            }

            using (TextWriter winwriter = new StreamWriter($"OscarAwardsWin.csv", true))
            using (TextWriter nominationwriter = new StreamWriter($"OscarAwardsNominations.csv", true))
            {
                foreach (var item in oscarModel.Awards)
                {
                    foreach (var mItem in item.Value.Wins)
                    {
                        for (int i = 0; i < mItem.Count; i++)
                            winwriter.WriteLine($"{oscarModel.Rank},\"{item.Key}\",{i},{mItem[i].IsKeyItem},\"{mItem[i].Key}\",\"{mItem[i].Value}\"");
                    }
                    foreach (var mItem in item.Value.Nominations)
                    {
                        for (int i = 0; i < mItem.Count; i++)
                            nominationwriter.WriteLine($"{oscarModel.Rank},\"{item.Key}\",{i},{mItem[i].IsKeyItem},\"{mItem[i].Key}\",\"{mItem[i].Value}\"");
                    }
                }
            }
        }
        
        private void ExtractDataForHeaderAndDataRow(IWebElement awardHeader,
            IWebElement awardElement)
        {
            var topicModel = new OscarAwardModel();

            if (string.IsNullOrWhiteSpace(awardHeader.Text)) return;

            oscarModel.Awards.Add(awardHeader.Text, topicModel);

            var namesListElement = awardElement.ByXpath("ul/li").GetAttribute("innerHTML");
            var hap = new HtmlDocument();
            hap.LoadHtml(namesListElement);

            topicModel.Wins.Add(new List<NameItem>());
            ExtractWinOrNomination(hap.DocumentNode.ChildNodes, topicModel.Wins[topicModel.Wins.Count - 1]);

            var nominationElements = awardElement.ByXpaths("ul/li/ul/li");
            ExtractNominations(nominationElements, topicModel);
        }

        private void ExtractMetadata()
        {
            var metaElements = driver.ByXpaths("//*[@id='mw-content-text']/div/table[contains(@class, 'infobox') and contains(@class, 'vevent')]//tr");

            oscarModel.Metadata = new Dictionary<string, List<NameItem>>();
            bool isHighlights = false;

            foreach (var metaEl in metaElements)
            {
                var th = metaEl.ByXpath("th");
                var td = metaEl.ByXpath("td");

                if (th != null && th.Text == "Highlights")
                {
                    isHighlights = true;
                    continue;
                }
                else
                    isHighlights = false;

                if (isHighlights) continue;

                if (th == null)
                {
                    var image = td.ByXpath("a/img");

                    if(image != null)
                    {
                        oscarModel.Metadata.Add("Poster", new List<NameItem>() {
                        new NameItem(){
                            Value = AbsoluteUrl(image.GetAttribute("src"))
                            }
                        });
                    }
                }
                else if(td != null)
                {
                    var tdChildsText = td.GetAttribute("innerHTML");
                    var hap = new HtmlDocument();
                    hap.LoadHtml(tdChildsText);
                    oscarModel.Metadata[th.Text] = ExtractMetadata(hap.DocumentNode.ChildNodes);
                }
                else 
                {
                    if (th.Text == "Highlights") isHighlights = true;
                    else isHighlights = false;
                }
            }
        }

        private string AbsoluteUrl(string url)
        {
            return new Uri(new Uri("https://en.wikipedia.org/"), url).AbsoluteUri;
        }

        private List<NameItem> ExtractMetadata(HtmlNodeCollection tdChilds)
        {
            var keyValuePairs = new List<NameItem>();
            ExtractMetadataPairs(tdChilds, keyValuePairs);
            return keyValuePairs;
        }

        private void ExtractMetadataPairs(HtmlNodeCollection tdChilds, List<NameItem> keyValuePairs)
        {
            List<HtmlNode> htmlNodes = ExtractNodeFilterBy(tdChilds.ToList());

            foreach (var item in htmlNodes)
            {
                if (item.NodeType == HtmlNodeType.Element && item.Name == "br")
                    continue;
                else if (item.NodeType == HtmlNodeType.Element && item.Name == "sup" &&
                    item.InnerText.Contains("[") && item.InnerText.Contains("]")) continue;
                else if (CheckIfNeedToSkip(item.InnerText)) continue;
                else if (item.NodeType == HtmlNodeType.Text)
                {
                    keyValuePairs.Add(new NameItem()
                    {
                        Key = "",
                        Value = Norm(item.InnerText)
                    });
                }
                else if (item.NodeType == HtmlNodeType.Element && item.Name == "a")
                {
                    keyValuePairs.Add(new NameItem()
                    {
                        Key = AbsoluteUrl(item.GetAttributeValue("href", "")),
                        Value = Norm(item.InnerText)
                    });
                }
                else if (item.NodeType == HtmlNodeType.Element && item.Name == "span")
                {
                    keyValuePairs.Add(new NameItem()
                    {
                        Key = "",
                        Value = Norm(item.InnerText)
                    });
                }
                else
                    throw new Exception("Unknwon node " + item.OuterHtml);
            }
        }

        private bool CheckIfNeedToSkip(string innerText)
        {
            var norm = HttpUtility.HtmlDecode(innerText);
            norm = norm.Trim();
            norm = norm.Replace(",", "").Replace(";", "").Replace(":", "").Replace("(", " ").Replace(")", " ");

            var regex = new Regex(@"^\(\d+\)$", RegexOptions.IgnoreCase);

            return string.IsNullOrWhiteSpace(norm) || regex.IsMatch(norm);
        }

        private string Norm(string innerText)
        {
            var norm = HttpUtility.HtmlDecode(innerText);
            norm = norm.Replace("\r", " ").Replace("\n", " ").Replace("  ", " ")
                .TrimEnd(',', ' ').TrimStart(',', ' ').Trim();
            return norm;
        }

        private void ExtractDataForOnlyDataRow(IWebElement awardElement)
        {
            var innerHtml = awardElement.GetAttribute("innerHTML");

            var topicEl = awardElement.ByXpath("div[1]");
            if (topicEl == null) return;

            var topicModel = new OscarAwardModel();
            topicModel.Topic = topicEl.Text;

            oscarModel.Awards.Add(topicModel.Topic, topicModel);

            var namesListElement = awardElement.ByXpath("ul/li").GetAttribute("innerHTML");
            var hap = new HtmlDocument();
            hap.LoadHtml(namesListElement);

            topicModel.Wins = new List<List<NameItem>>();
            topicModel.Wins.Add(new List<NameItem>());
            ExtractWinOrNomination(hap.DocumentNode.ChildNodes, topicModel.Wins[topicModel.Wins.Count - 1]);

            var nominationElements = awardElement.ByXpaths("ul/li/ul/li");
            topicModel.Nominations = new List<List<NameItem>>();
            ExtractNominations(nominationElements, topicModel);
        }

        private void ExtractNominations(ReadOnlyCollection<IWebElement> nominationElements, OscarAwardModel topicModel)
        {
            foreach (var nominationEl in nominationElements)
            {
                var hap = new HtmlDocument();
                hap.LoadHtml(nominationEl.GetAttribute("innerHTML"));
                topicModel.Nominations.Add(new List<NameItem>());
                ExtractWinOrNomination(hap.DocumentNode.ChildNodes, topicModel.Nominations[topicModel.Nominations.Count - 1]);
            }
        }
    
        private void ExtractWinOrNomination(HtmlNodeCollection nodes, List<NameItem> keyValuePairs)
        {
            var winNodes = new List<Tuple<string, string>>();
            foreach (var node in nodes)
            {
                if (node.Name == "ul")
                    break;
                else if (node.NodeType == HtmlNodeType.Text)
                    winNodes.Add(new Tuple<string, string>("", node.InnerText));
                else
                {
                    var filteredNodes = new List<HtmlNode>();
                    filteredNodes.Add(node);
                    var nodeFilters = ExtractNodeFilterBy(filteredNodes);

                    foreach (var item in nodeFilters)
                    {
                        if (item.NodeType == HtmlNodeType.Element && item.Name == "sup") continue;
                        else if (item.NodeType == HtmlNodeType.Element && (item.Name == "a"))
                        {
                            winNodes.Add(new Tuple<string, string>(AbsoluteUrl(item.GetAttributeValue("href", "")),
                                Norm(item.InnerText)));
                        }
                        else if (item.NodeType == HtmlNodeType.Element && (item.Name == "span"))
                        {
                            winNodes.Add(new Tuple<string, string>("", Norm(item.InnerText)));
                        }
                        else if (item.NodeType == HtmlNodeType.Element && item.Name == "img" &&
                            item.OuterHtml.Contains("double-dagger"))
                            ;
                        else if (item.NodeType == HtmlNodeType.Text)
                            winNodes.Add(new Tuple<string, string>("", Norm(item.InnerText)));
                        else
                            throw new Exception("Unknown element");
                    }
                }
            }

            int index = 0;
            var newWinNodes = new List<Tuple<string, string>>();
            bool isKeyItem = true;
            for (; index < winNodes.Count; ++index )
            {
                var item = winNodes[index];
                if (item.Item2.Contains("–"))
                {
                    var splits = item.Item2.Split(new string[] { "–", "–" }, StringSplitOptions.None);

                    if (splits != null && splits.Length > 0)
                    {
                        if (!CheckIfNeedToSkip(splits[0]))
                            keyValuePairs.Add(new NameItem()
                            {
                                Key = item.Item1,
                                Value = Norm(splits[0]),
                                IsKeyItem = isKeyItem
                            });

                        int i = 1, indexbck = index;
                        isKeyItem = false;
                        for (; i < splits.Length; i++)
                        {
                            if (!CheckIfNeedToSkip(splits[i]))
                            {
                                keyValuePairs.Add(new NameItem()
                                {
                                    Key = item.Item1,
                                    Value = Norm(splits[i]),
                                    IsKeyItem = isKeyItem
                                });
                                winNodes.Insert(index + i, new Tuple<string, string>(item.Item1, splits[i]));
                            }
                        }
                    }
                    else
                    {
                        if (!CheckIfNeedToSkip(item.Item2))
                        {
                            keyValuePairs.Add(new NameItem()
                            {
                                Key = item.Item1,
                                Value = Norm(item.Item2),
                                IsKeyItem = isKeyItem
                            });
                        }
                    }
                    isKeyItem = false;
                }
                else
                {
                    if (!CheckIfNeedToSkip(item.Item2))
                    {
                        keyValuePairs.Add(new NameItem()
                        {
                            Key = item.Item1,
                            Value = item.Item2,
                            IsKeyItem = isKeyItem
                        });
                    }
                }
            }

            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                var nameItem = keyValuePairs[i];
                var regexSplits = Regex.Split(nameItem.Value, @"(?<=\sfor\s|\sand\s)");

                keyValuePairs.RemoveAt(i);
                for (int j = 0; j < regexSplits.Length; j++)
                {
                    var normData = Norm(regexSplits[j]);

                    if(!CheckIfNeedToSkip(normData))
                    keyValuePairs.Insert(i + j, new NameItem()
                    {
                        IsKeyItem = nameItem.IsKeyItem,
                        Key = nameItem.Key,
                        Value = normData
                    });
                }
            }
        }

        private List<HtmlNode> ExtractNodeFilterBy(List<HtmlNode> filteredNodes)
        {
            var newNodes = new List<HtmlNode>();
            bool modified = false;

            for (int i = 0; i < filteredNodes.Count; i++)
            {
                if (filteredNodes[i].Name == "b" || filteredNodes[i].Name == "i" || filteredNodes[i].Name == "s"
                    || filteredNodes[i].Name == "small" || filteredNodes[i].Name == "span" || filteredNodes[i].Name == "div"
                    || filteredNodes[i].Name == "ul" || filteredNodes[i].Name == "li")
                {
                    var chNodes = filteredNodes[i].ChildNodes;

                    foreach (var item in chNodes) newNodes.Add(item);
                    modified = true;
                }
                else
                {
                    newNodes.Add(filteredNodes[i]);
                }
            }

            if (modified) return ExtractNodeFilterBy(newNodes);
            else return newNodes;
        }

        private void ExtractPageUrls()
        {
            PageUrls = new List<string>();

            for (int i = 1; i < (2019 - 1929); i++)
            {
                var rankSuffix = "th";

                if (i % 10 == 1 && i != 11) rankSuffix = "st";
                else if (i % 10 == 2 && i != 12) rankSuffix = "nd";
                else if (i % 10 == 3 && i != 13) rankSuffix = "rd";

                PageUrls.Add($"https://en.wikipedia.org/wiki/{i}{rankSuffix}_Academy_Awards");
            }
        }
    }
}
