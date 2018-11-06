using log4net;
using MovieDataExtractor.Cinemasight;
using MovieDataExtractor.OscarOrg;
using System.Configuration;
using System.Reflection;

namespace MovieDataExtractor
{
    class Program
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            logger.Info("Start the Movie Extractor program");

            // Extract the website data
            using (var driver = new SeleniumService())
            {
                logger.Info("Start extracting movie data from cinemasight url");

                var awardCinemaSightObj = new CinemaSightAwardExtract(driver);
                awardCinemaSightObj.Run();

                logger.Info("End extracting movie data from cinemasight url");
                logger.Info("Start extracting movie data from oscar org url");

                var awardOscarOrgObj = new OscarOrgAwardExtract(driver);
                awardOscarOrgObj.Run();

                logger.Info("End extracting movie data from oscar org url");
            }

            logger.Info("End the Movie Extractor program");
        }
    }
}
