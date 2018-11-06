using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MovieDataExtractor
{
    public class SeleniumService : IDisposable
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IWebDriver driver;

        public SeleniumService()
        {
            Initialize();
        }

        public void Dispose()
        {
            if (driver != null) driver.Close();
        }

        public void Initialize()
        {
            driver = new ChromeDriver(@"D:\WebDriver\chromedriver_win32");
            //driver = new InternetExplorerDriver(@"D:\WebDriver\IEDriverServer_Win32_3.14.0");
        }

        public void Navigate(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public IWebElement ByName(string name)
        {
            try
            {
                return driver.FindElement(By.Name(name));
            }
            catch(Exception /*ex*/)
            {
                return null;
            }
        }

        public IWebElement ByXpath(string name)
        {
            try
            {
                return driver.FindElement(By.XPath(name));
            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }

        public ReadOnlyCollection<IWebElement> ByXpaths(string name)
        {
            try
            {
                return driver.FindElements(By.XPath(name));
            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }

        public string GetHref(string xpath)
        {
            var ciaLinkElement = driver.FindElement(By.XPath(xpath));
            return ciaLinkElement.GetAttribute("href");
        }

        public IList<IWebElement> GetElements(string xpath)
        {
            return driver.FindElements(By.XPath(xpath));
        }
    }

    public static class IWebElement_Extension
    {
        /// <summary>
        /// The logger for the cinemasight award extract class
        /// </summary>
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ReadOnlyCollection<IWebElement> ByXpaths(this IWebElement thisObj, string name)
        {
            try
            {
                return thisObj.FindElements(By.XPath(name));
            }
            catch (Exception ex)
            {
                logger.Error("", ex);
            }
            return null;
        }

        public static IWebElement ByXpath(this IWebElement thisObj, string name)
        {
            try
            {
                return thisObj.FindElement(By.XPath(name));
            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }
    }
}
