using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataExtractor.Base
{
    /// <summary>
    /// The base class which helps extracting data from the website and save it
    /// probably oin a csv format
    /// </summary>
    public class ExtractDataFromWebsite
    {
        /// <summary>
        /// The Selenium driver
        /// </summary>
        protected SeleniumService driver;

        /// <summary>
        /// A base url is needed to initialize
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="config"></param>
        public ExtractDataFromWebsite(SeleniumService driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Helper method to split a html content
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        protected List<string> Helper_SplitNoEmptyEntries(string htmlContent, 
            params string[] splitter)
        {
            return htmlContent.Split(splitter, 
                StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Helper method to trim the data extracted
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string Helper_TrimData(string content, params char[] trimOptions)
        {
            if (content == null) return "";
            return (trimOptions == null || trimOptions.Count() <= 0) ? content.Trim(' ', ':', ',') :
                content.Trim(trimOptions);
        }

        /// <summary>
        /// Helper method to trim the data extracted
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string Helper_TrimDataWithNewLines(string content, params char[] trimOptions)
        {
            return Helper_TrimData(content).Replace("\r","").Replace("\n","");
        }

        /// <summary>
        /// Helper method to parse and get the html root element
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        protected HtmlNode Helper_HtmlParse(string htmlContent)
        {
            var hap = new HtmlDocument();
            hap.LoadHtml(htmlContent);

            return hap.DocumentNode;
        }
    }
}
