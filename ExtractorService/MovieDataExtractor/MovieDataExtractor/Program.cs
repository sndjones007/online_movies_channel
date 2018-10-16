using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using SeleniumTest.Movies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //using (var driver = new SeleniumService())
                //{
                //    var awardWinnersObj = new WikiOscarAwards(driver);
                //    awardWinnersObj.Run();
                //}
                var preferences = new InternetExplorerOptions();
                preferences.SetLoggingPreference(LogType.Browser, LogLevel.All);
                preferences.SetLoggingPreference(LogType.Client, LogLevel.All);
                preferences.SetLoggingPreference(LogType.Driver, LogLevel.All);
                preferences.SetLoggingPreference(LogType.Profiler, LogLevel.All);
                preferences.SetLoggingPreference(LogType.Server, LogLevel.All);

                IWebDriver driver = new InternetExplorerDriver(@"D:\WebDriver\IEDriverServer_Win32_3.14.0",
                    preferences);
                //IWebDriver driver = new ChromeDriver(@"D:\WebDriver\chromedriver_win32");
                driver.Navigate().GoToUrl(@"https://en.wikipedia.org/wiki/Main_Page");
                var element = driver.FindElement(By.XPath(@"//*[@id='mp-topbanner']/div/div[1]/a"));
                element.Click();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
