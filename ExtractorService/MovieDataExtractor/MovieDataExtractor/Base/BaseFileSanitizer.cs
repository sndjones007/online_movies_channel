using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieDataExtractor.Base
{
    public class BaseFileSanitizer
    {
        /// <summary>
        /// Split a line like csv
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected IEnumerable<string> SplitCSV(string input)
        {
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);

            foreach (Match match in csvSplit.Matches(input))
            {
                yield return ToTitleCase(TrimCsvData(match.Value).ToLower());
            }
        }

        protected string TrimCsvData(string csvValue)
        {
            return csvValue.Trim('"', ' ', '–', ',');
        }

        protected string ToTitleCase(string csvValue)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(csvValue);
        }
    }
}
