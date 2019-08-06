using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleProxy
{
    class HtmlModifier
    {
        public static readonly string ExtraString = "™";

        public static string Modify(string text, int wordLength = 6)
        {
            // Текст который не содержит в себе тегов
            string matchPattern = @">([^<>]*)</?([^>]*)";
            // Слова длины 6
            string replacePattern = @"(^|[^a-zA-Zа-яА-Я0-9])([a-zA-Zа-яА-Я]{" + wordLength + @"})($|[^a-zA-Zа-яА-Я0-9])";

            foreach (Match match in Regex.Matches(text, matchPattern, RegexOptions.Compiled))
            {
                string oldValue = match.Groups[1].Value;
                string tag = match.Groups[2].Value;
                int oldValueIndex = match.Groups[1].Index;
                if (oldValue.Trim().Length == 0 || tag == "script" || tag == "style" || tag == "code") continue;
                string newValue = Regex.Replace(oldValue, replacePattern, @"$1$2" + ExtraString + @"$3", RegexOptions.Compiled);
                Console.WriteLine("Old: {0}", match.Groups[0].Value);
                Console.WriteLine("New: {0}", newValue);
                Console.WriteLine();
                text = text.Replace(oldValue, newValue);
            }
            text = Regex.Replace(text, "(" + ExtraString + ")+", ExtraString);
            text = Regex.Replace(text, "" + ExtraString + "([a-zA-Zа-яА-Я])", "$1");
            return text;
        }
    }
}
