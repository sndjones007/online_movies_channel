using SeleniumTest.Cinemasight;

namespace SeleniumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new SeleniumService())
            {
                var awardWinnersObj = new CinemaSightOscarExtract(driver);
                awardWinnersObj.Run();
            }
        }
    }
}
