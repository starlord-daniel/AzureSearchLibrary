using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureSearch.Helper
{
    public class BotHelper
    {
        /// <summary>
        /// Converts a list to a dictionary for the follow up prompt.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static Dictionary<string, IReadOnlyList<string>> ConvertListToDict(List<string> options)
        {
            Dictionary<string, IReadOnlyList<string>> optionDict = new Dictionary<string, IReadOnlyList<string>>();

            foreach (var option in options)
            {
                optionDict.Add(option, new List<string> { option });
            }

            return optionDict;
        }

        /// <summary>
        /// Generates a list of infos for the follow up prompt.
        /// </summary>
        /// <param name="unformattedListString">The unformatted string from the database.</param>
        /// <param name="delimiter">The character that seperates the options.</param>
        /// <param name="pattern">The regex pattern to return only the delimited options. The result must be in the first regex group (take a look at the default value, which takes [1,2] and returns 1,2</param>
        /// <returns>A list of elements that were divided by <paramref name="delimiter"/></returns>
        internal static List<string> GenerateListFromString(string unformattedListString, char delimiter = ',', string pattern = "[[](.*)[]]")
        {
            List<string> elements = new List<string>();

            Regex r = new Regex(pattern);

            // Get the group out of the match
            MatchCollection matches = r.Matches(unformattedListString);
            if (matches.Count > 0)
            {
                var delimitedString = matches[0].Groups[1].Value;

                // Split the group by delimiter
                var splitOptions = delimitedString.Split(delimiter);

                if(splitOptions.Length > 1)
                {
                    elements = splitOptions.ToList();
                }
            }
            
            return elements;
        }
    }
}
