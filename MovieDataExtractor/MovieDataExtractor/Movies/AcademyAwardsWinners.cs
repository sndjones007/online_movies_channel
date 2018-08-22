using MySql.Data.Types;
using OpenQA.Selenium;
using SeleniumTest.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeleniumTest.Movies
{
    /// <summary>
    /// A class to extract oscars awards information from the official site.
    /// </summary>
    public class AcademyAwardsWinners
    {
        const string DOMAIN_URL = @"http://www.oscars.org";
        const int startyear = 2017;
        const string INITIAL_AWARDS_URL_FORMAT = @"{0}/oscars/ceremonies/2017";
        const string YEARS_LIST_XPATH = @"//*[@id='ceremonies-decade-scroller']/div[2]/ul/li/a";
        const string FILE_SAVE_FORMAT = @"Oscar{0}.xml";
        const string XPATH_GROUP_MOVIES_BY_ALPHA = @"//*[@id='quicktabs-tabpage-honorees-1']/div/div[@class='view-content']/div[@class='view-grouping']";
        const string XPATH_MOVIES_ITEM_GROUP = @"./div[@class='view-grouping-content']/div[@class='view-grouping']";

        SeleniumService driver;
        string initialOscarAwardsUrl;
        SortedDictionary<int, AcademyAwardsWinnersModel> yearOscarsUrl = 
                new SortedDictionary<int, AcademyAwardsWinnersModel>();
        MySqlConnector sqlConnector;
        int oscards_awards_id = 1;
        int film_id = 1;
        int award_film_id = 1;

        Dictionary<string, OscarPrizeItemModel> storeByMovies;

        public AcademyAwardsWinners(SeleniumService driver)
        {
            this.driver = driver;
            initialOscarAwardsUrl = string.Format(INITIAL_AWARDS_URL_FORMAT, DOMAIN_URL);
        }

        private void PopulateYearsAndUrls(ReadOnlyCollection<IWebElement> elements)
        {
            foreach (var item in elements)
            {
                int year = Convert.ToInt32(item.Text);

                if(startyear <= year)
                {
                    yearOscarsUrl.Add(year,
                    new AcademyAwardsWinnersModel()
                    {
                        PageUrl = $"{item.GetAttribute("href")}"
                    });
                }
            }
        }

        public void Run()
        {
            driver.Navigate(initialOscarAwardsUrl);

            PopulateYearsAndUrls(driver.ByXpaths(YEARS_LIST_XPATH));

            sqlConnector = new MySqlConnector();
            sqlConnector.Open();

            oscards_awards_id = sqlConnector.DMLCount("Select max(oscar_id) from oscar_awards");
            film_id = sqlConnector.DMLCount("Select max(film_id) from movie_type");
            award_film_id = sqlConnector.DMLCount("Select max(award_film_id) from award_type");

            oscards_awards_id++;
            film_id++;
            award_film_id++;

            foreach (var kv in yearOscarsUrl)
            {
                storeByMovies = new Dictionary<string, OscarPrizeItemModel>(
                    StringComparer.InvariantCultureIgnoreCase);
                Console.WriteLine($"Extracting for {kv.Value.PageUrl}");
                driver.Navigate(kv.Value.PageUrl);
                Extract(kv.Value);

                if(storeByMovies.Count > 0)
                {
                    foreach (var kv1 in storeByMovies)
                    {
                        kv.Value.PrizeItemModels.Add(kv1.Value);
                    }
                }
                
                SaveInDb(kv);
                }

            sqlConnector.Close();
        }

        private void SaveInDb(KeyValuePair<int, AcademyAwardsWinnersModel> kv)
        {
            var startDate = kv.Value.StartDate.ToString("yyyy-MM-dd");
            var honoringStartDate = kv.Value.HonoringReleaseStartDate.ToString("yyyy-MM-dd");
            var honoringEndDate = kv.Value.HonoringReleaseEndDate.ToString("yyyy-MM-dd");

            sqlConnector.DDL($"INSERT INTO oscar_awards VALUES({oscards_awards_id}, {kv.Value.Rank}, '{Norm(kv.Value.Location)}', '{startDate}', '{honoringStartDate}', '{honoringEndDate}')");

            foreach (var movie in kv.Value.PrizeItemModels)
            {
                sqlConnector.DDL($"INSERT INTO movie_type VALUES({film_id}, '{Norm(movie.MovieName)}', {movie.WinsCount}, {movie.NominationsCount}, {oscards_awards_id})");

                foreach (var winItem in movie.Wins)
                {
                    sqlConnector.DDL($"INSERT INTO award_type VALUES({award_film_id}, {film_id}, '{Norm(winItem.AwardName)}', '{Norm(winItem.PersonName)}', 1)");
                    award_film_id++;
                }

                foreach (var nominationItem in movie.Nominations)
                {
                    sqlConnector.DDL($"INSERT INTO award_type VALUES({award_film_id}, {film_id}, '{Norm(nominationItem.AwardName)}', '{Norm(nominationItem.PersonName)}', 0)");
                    award_film_id++;
                }

                film_id++;
            }

            oscards_awards_id++;
        }

        private object Norm(string movieName)
        {
            return movieName.Replace("'", "''");
        }

        private void SaveInXml(KeyValuePair<int, AcademyAwardsWinnersModel> kv)
        {
            var serializer = new XmlSerializer(typeof(AcademyAwardsWinnersModel));
            using (TextWriter writer = new StreamWriter(string.Format(FILE_SAVE_FORMAT, kv.Key)))
            {
                serializer.Serialize(writer, kv.Value);
            }
        }

        private void Extract(AcademyAwardsWinnersModel yearAwardModel)
        {
            ExtractOscarMetadataForYear(yearAwardModel);

            if (yearAwardModel.StartDate < new DateTime(2017, 1, 1))
            {
                ViewAllMoviesByFilm();
                ExtractMoviesListAwardsAndNominations(yearAwardModel);
            }
            else
            {
                ViewAllMoviesByCategory();
                ExtractMoviesListAwardsAndNominationsByCategory(yearAwardModel);
            }
        }

        private void ExtractMoviesListAwardsAndNominationsByCategory(AcademyAwardsWinnersModel yearAwardModel)
        {
            var groupingCategoryElements = driver.ByXpaths(@"//*[@id='quicktabs-tabpage-honorees-0']/div/div[2]/div[@class='view-grouping']");

            if (yearAwardModel.PrizeItemModels == null)
                yearAwardModel.PrizeItemModels = new List<OscarPrizeItemModel>();

            foreach (var category in groupingCategoryElements)
            {
                var categoryName = category.ByXpath(@"./div[@class='view-grouping-header']/h2").Text;

                //if (!storeByMovies.ContainsKey(categoryName)) storeByMovies.Add(categoryName, new OscarPrizeItemModel());

                var winsSections = category.ByXpaths(@"./div[@class='view-grouping-content']/div[count(preceding-sibling::h3)=1]");
                var nominationsSections = category.ByXpaths(@"./div[@class='view-grouping-content']/div[count(preceding-sibling::h3)=2]");

                ExtractWins(categoryName, winsSections);
                ExtractNominations(categoryName, nominationsSections);
            }
        }

        private void ExtractNominations(string categoryName, ReadOnlyCollection<IWebElement> nominationsSections)
        {
            foreach (var nominationElement in nominationsSections)
            {
                var personName = nominationElement.ByXpath(@"./div[1]").Text;
                var movieName = nominationElement.ByXpath(@"./div[2]").Text;

                if (!storeByMovies.ContainsKey(movieName))
                    storeByMovies.Add(movieName, new OscarPrizeItemModel()
                    {
                        MovieName = movieName,
                        Nominations = new List<MoviePropertyItemModel>(),
                        Wins = new List<MoviePropertyItemModel>()
                    });
                storeByMovies[movieName].Nominations.Add(new MoviePropertyItemModel()
                {
                    PersonName = personName,
                    AwardName = categoryName
                });
            }
        }

        private void ExtractWins(string categoryName, ReadOnlyCollection<IWebElement> winsSections)
        {
            foreach (var winElement in winsSections)
            {
                var personName = winElement.ByXpath(@"./div[1]").Text;
                var movieName = winElement.ByXpath(@"./div[2]").Text;

                if (!storeByMovies.ContainsKey(movieName))
                    storeByMovies.Add(movieName, new OscarPrizeItemModel()
                    {
                        MovieName = movieName,
                        Nominations = new List<MoviePropertyItemModel>(),
                        Wins = new List<MoviePropertyItemModel>()
                    });
                storeByMovies[movieName].Wins.Add(new MoviePropertyItemModel()
                    {
                        PersonName = personName,
                        AwardName = categoryName
                    });
            }
        }

        private void ViewAllMoviesByCategory()
        {
            var aViewByFilmElement = driver.ByXpath(@"//*[@id='quicktabs-tab-honorees-0']");
            aViewByFilmElement.Click();
        }

        private void ExtractMoviesListAwardsAndNominations(AcademyAwardsWinnersModel yearAwardModel)
        {
            var alphabetsGroupingElements = driver.ByXpaths(XPATH_GROUP_MOVIES_BY_ALPHA);
            foreach (var alphGroup in alphabetsGroupingElements)
            {
                var sameAlphMoviesElements = alphGroup.ByXpaths(XPATH_MOVIES_ITEM_GROUP);

                if (yearAwardModel.PrizeItemModels == null)
                    yearAwardModel.PrizeItemModels = new List<OscarPrizeItemModel>();

                int index = 0;
                foreach (var movieElement in sameAlphMoviesElements)
                {
                    var oscarPrizeModel = new OscarPrizeItemModel();
                    yearAwardModel.PrizeItemModels.Add(oscarPrizeModel);
                    ExtractMovieNameWithNominations(oscarPrizeModel, index, movieElement);
                    index++;
                }
            }
        }

        private void ExtractMovieNameWithNominations(OscarPrizeItemModel oscarPrizeModel, int index, IWebElement movieElement)
        {
            oscarPrizeModel.MovieName = movieElement.ByXpath(@"./div[@class='view-grouping-header']").Text;
            
            var winsNominationsCountText = movieElement.ByXpath(@"./span").Text.ToLower();
            var winsCountTextElement = movieElement.ByXpath(@"./span/span[@class='golden-text']");
            string nominationsCountText = winsNominationsCountText;
            string winsCountText = "";

            if(winsCountTextElement != null)
            {
                winsCountText = winsCountTextElement.Text.ToLower();
                nominationsCountText = winsNominationsCountText.Replace(winsCountText, "");
                nominationsCountText = nominationsCountText.Replace(",", "");
            }

            if(!string.IsNullOrWhiteSpace(nominationsCountText))
            {
                nominationsCountText = nominationsCountText.Replace("nomination", "").Replace("s", "").Trim();
                oscarPrizeModel.NominationsCount = Convert.ToInt32(nominationsCountText);
            }

            if (!string.IsNullOrWhiteSpace(winsCountText))
            {
                winsCountText = winsCountText.Replace("win", "").Replace("s", "").Trim();
                oscarPrizeModel.WinsCount = Convert.ToInt32(winsCountText);
            }

            var moviePropertiesElements = movieElement.ByXpaths(@"./div[@class='view-grouping-content']/div");

            oscarPrizeModel.Nominations = new List<MoviePropertyItemModel>();
            oscarPrizeModel.Wins = new List<MoviePropertyItemModel>();

            foreach (var moviePropertyElement in moviePropertiesElements)
            {
                ExtractMovieProperty(oscarPrizeModel, moviePropertyElement);
            }
        }

        private void ExtractMovieProperty(OscarPrizeItemModel oscarPrizeModel, IWebElement moviePropertyElement)
        {
            var moviePropertyMainElement = moviePropertyElement.ByXpath(@"./div/div");
            var goldenTextSpan = moviePropertyMainElement.ByXpath(@"./span[@class='golden-text']");
            var moviePropertyMainList = moviePropertyElement.Text.Split('-').Select(p => p.Trim()).ToList();

            var personName = moviePropertyMainList[1];
            var personNameNorm = personName.Split(new string[] { " in " }, StringSplitOptions.None)[0];

            if (goldenTextSpan != null && moviePropertyMainList[0].StartsWith("*"))
            {
                moviePropertyMainList[0] = moviePropertyMainList[0].Substring(1).Trim();
                oscarPrizeModel.Wins.Add(new MoviePropertyItemModel()
                {
                    AwardName = moviePropertyMainList[0],
                    PersonName = personNameNorm
                });
            }
            else
            {
                oscarPrizeModel.Nominations.Add(new MoviePropertyItemModel()
                {
                    AwardName = moviePropertyMainList[0],
                    PersonName = personNameNorm
                });
            }
        }

        private void ViewAllMoviesByFilm()
        {
            var aViewByFilmElement = driver.ByXpath(@"//*[@id='quicktabs-tab-honorees-1']");
            aViewByFilmElement.Click();
            var aViewAllElement = driver.ByXpath(@"//*[@id='quicktabs-tabpage-honorees-1']//div[@class='glossary']/a[1]");
            aViewAllElement.Click();
        }

        private void ExtractOscarMetadataForYear(AcademyAwardsWinnersModel yearAwardModel)
        {
            var sectionElement = driver.ByXpath(@"//div[contains(@class, 'view-awards-ceremonies')]/div[@class='view-content']/div");

            ExtractOscarRank(yearAwardModel, sectionElement);
            ExtractOscarLocation(yearAwardModel, sectionElement);
            ExtractOscarDate(yearAwardModel, sectionElement);
            ExtractOscarHonoringDate(yearAwardModel, sectionElement);
        }

        private void ExtractOscarHonoringDate(AcademyAwardsWinnersModel yearAwardModel, IWebElement sectionElement)
        {
            var honoringElement = sectionElement.ByXpath(@"div[4]");
            var honoringTitle = honoringElement.Text.ToLower().Replace("honoring movies released from ", "").Replace(".", "").Replace("-", "").Replace("  ", " ").Replace(",", "").Trim();
            var dates = honoringTitle.Split(' ');

            if(dates.Length != 6)
            {
                honoringTitle = honoringElement.Text.ToLower().ToLower().Replace("honoring movies released in ", "").Trim();
                yearAwardModel.HonoringReleaseStartDate = DateTime.ParseExact(honoringTitle, "yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                yearAwardModel.HonoringReleaseStartDate = DateTime.ParseExact($"{dates[0]} {dates[1]} {dates[2]}", "MMMM d yyyy", CultureInfo.InvariantCulture);
                yearAwardModel.HonoringReleaseEndDate = DateTime.ParseExact($"{dates[3]} {dates[4]} {dates[5]}", "MMMM d yyyy", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Thursday, May 16, 1929
        /// </summary>
        /// <param name="yearAwardModel"></param>
        /// <param name="sectionElement"></param>
        private void ExtractOscarDate(AcademyAwardsWinnersModel yearAwardModel, IWebElement sectionElement)
        {
            var dateElement = sectionElement.ByXpath(@"div[3]");
            yearAwardModel.StartDate = DateTime.ParseExact(dateElement.Text, "dddd, MMMM d, yyyy", CultureInfo.InvariantCulture);
        }

        private void ExtractOscarLocation(AcademyAwardsWinnersModel yearAwardModel, IWebElement sectionElement)
        {
            var locationElement = sectionElement.ByXpath(@"div[2]");
            yearAwardModel.Location = locationElement.Text;
        }

        private void ExtractOscarRank(AcademyAwardsWinnersModel yearAwardModel, IWebElement sectionElement)
        {
            var titleElement = sectionElement.ByXpath(@"div[1]");
            var textTitle = titleElement.Text.ToLower();
            var regex = new Regex(@"the\s(\d+)(st|nd|rd|th)\sacademy\sawards");

            var match = regex.Match(textTitle);
            yearAwardModel.Rank = Convert.ToInt32(match.Groups[1].Value);
        }
        
    }
}
