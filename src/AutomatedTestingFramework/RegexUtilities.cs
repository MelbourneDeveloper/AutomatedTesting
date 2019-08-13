using System;
using System.Text.RegularExpressions;

namespace AutomatedTestingFramework
{
    public class RegexUtilities
    {
        private const string NoParamValueRegexPattern = @"(\#.*\#)";
        private const string OneParamValueRegexPattern = @"(\#.*\#)(\[.*\])";
        private const string TwoParamsValueRegexPattern = @"(\#.*\#)(\[.*\])(\[.*\])";

        private static string StripHash(string value)
        {
            return value.Replace("#", string.Empty);
        }

        private static string StripSquareBrackets(string value)
        {
            return value.Replace("[", string.Empty).Replace("]", string.Empty);
        }

        public static object GetDependentValueArgument(string value)
        {
            var twoParamsMatch = Regex.Match(value, TwoParamsValueRegexPattern);

            if (twoParamsMatch.Groups.Count == 4 && twoParamsMatch.Groups[1].Value == "#VALUE#")
            {
                var tokens = new[] { StripSquareBrackets(twoParamsMatch.Groups[2].Value), StripSquareBrackets(twoParamsMatch.Groups[3].Value) };
                return new ValueArgument { UseCaseName = tokens[0], ValueKey = tokens[1] };
            }

            var oneParamMatch = Regex.Match(value, OneParamValueRegexPattern);
            if (oneParamMatch.Groups.Count == 3)
            {
                if (oneParamMatch.Groups[1].Value == "#MACRO#") return new MacroArgument { Key = StripSquareBrackets(oneParamMatch.Groups[2].Value) };
                if (oneParamMatch.Groups[1].Value == "#REPEATERMACRO#") return new RepeaterMacroArgument { Key = StripSquareBrackets(oneParamMatch.Groups[2].Value) };
                if (oneParamMatch.Groups[1].Value == "#VALUE#") return new ValueArgument { ValueKey = StripSquareBrackets(oneParamMatch.Groups[2].Value) };

                if (oneParamMatch.Groups[1].Value == "#RANDOMGUID#")
                {
                    var parameterOne = StripSquareBrackets(oneParamMatch.Groups[2].Value);
                    if (!int.TryParse(parameterOne, out var length)) throw new Exception($"The text {parameterOne} is not a valid length");
                    return Guid.NewGuid().ToString().Substring(0, length);
                }

                if (oneParamMatch.Groups[1].Value == "#DATETIMEGEN#")
                {
                    var parameterOne = StripSquareBrackets(oneParamMatch.Groups[2].Value);
                    int.TryParse(parameterOne, out var addedDays);
                    var dateTime = DateTime.Now.AddDays(addedDays).ToString("dddd, dd MMMM yyyy HH:mm:ss tt");
                    return dateTime;
                }
            }

            var noParamMatch = Regex.Match(value, NoParamValueRegexPattern);
            if (noParamMatch.Groups.Count == 2 && noParamMatch.Groups[1].Value == "#RANDOMGUID#")
            {
                return Guid.NewGuid().ToString();
            }

            return null;
        }

        public static string ReplaceString(string inputString, string patternString, string replaceString) => Regex.Replace(inputString, patternString, replaceString);

        public static Match MathPattern(string inputString, string patternString) => Regex.Match(inputString, patternString);

        public static MatchCollection MatchPatternAll(string inputString, string patternString) => Regex.Matches(inputString, patternString);

        public static object ReadXml(string path)
        {
            var reader = new System.Xml.XmlTextReader(path);
            return reader;
        }
    }
}
