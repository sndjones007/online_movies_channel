using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataExtractor.Base
{
    public class WebsiteDataExtractorBaseConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("baseUrl")]
        public string BaseUrl
        {
            get
            {
                return (string)this["baseUrl"];
            }
            set
            {
                this["baseUrl"] = value;
            }
        }
    }
}
