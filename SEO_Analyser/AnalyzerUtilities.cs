using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SEO_Analyser
{
    public class AnalyzerUtilities
    {
        /// <summary>
        /// Get the list of the english stop words stored in a Resources projects text file
        /// </summary>
        /// <returns>string array of the english stop words</returns>
        public static string[] GetStopWordsList()
        {
            return Properties.Resources.en_stop_words.Split(',');
        }

        public static char[] GetDelimitersList()
        {
            return new char[] {' ',',',';','!','?','"', '\'' , '\r', '\n', '\t', '(', ')'};
        }
        /// <summary>
        /// add a word to the list passed as parameter
        /// </summary>
        /// <param name="pW">string to add</param>
        /// <param name="pwordsCounter">list of words</param>
        /// <param name="pCount">define if count how many occurrences of each word</param>
        /// <returns></returns>
        public static Dictionary<string, int> AddWordToList(string pW, Dictionary<string, int> pwordsCounter, bool pCount)
        {            
            if (pW.Trim() != "" && Regex.IsMatch(pW, @"^[a-zA-Z]+$"))
            {
                if (!pCount)
                {
                    if (!pwordsCounter.ContainsKey(pW))
                        pwordsCounter.Add(pW, 0);
                }
                else
                {
                    if (pwordsCounter.ContainsKey(pW))
                        pwordsCounter[pW] += 1;
                    else
                        pwordsCounter.Add(pW, 1);
                }
            }
            return pwordsCounter;
        }

    }
}