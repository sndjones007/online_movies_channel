using MovieDataExtractor.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MovieDataExtractor.Cinemasight
{
    public class CinemaSightConfigSection : WebsiteDataExtractorBaseConfigSection
    {
        /// <summary>
        /// In the Award history index page to get the extract of all the url links
        /// </summary>
        [ConfigurationProperty("awardIndexExtractXpath")]
        public string Xpath_AwardIndexExtract
        {
            get
            {
                return HttpUtility.HtmlDecode((string)this["awardIndexExtractXpath"]);
            }
            set
            {
                this["awardIndexExtractXpath"] = HttpUtility.HtmlEncode(value);
            }
        }

        /// <summary>
        /// The csv file name to save the extracted data for oscar awards
        /// </summary>
        [ConfigurationProperty("fileNameAwards")]
        public string FileName_Awards
        {
            get
            {
                return (string)this["fileNameAwards"];
            }
            set
            {
                this["fileNameAwards"] = value;
            }
        }
    }
}
