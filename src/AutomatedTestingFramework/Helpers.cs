using AutomatedTestingFramework.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AutomatedTestingFramework
{
    public static class Helpers
    {
        public static string Serialise<SerialiseType>(SerialiseType theObject, Type[] extraTypes = null)
        {
            if (extraTypes == null)
            {
                extraTypes = GetExtraTypes<StepBase>(AppDomain.CurrentDomain.GetAssemblies()).ToArray();
            }

            var sb = new StringBuilder();
            using (var streamWriter = new StringWriter(sb))
            {
                var serializer = new XmlSerializer(theObject.GetType(), extraTypes);
                serializer.Serialize(streamWriter, theObject);
            }

            return sb.ToString();
        }

        public static SerialiseType Deserialise<SerialiseType>(string objectXml, Type[] extraTypes = null)
        {
            if (extraTypes == null)
            {
                extraTypes = GetExtraTypes<StepBase>(AppDomain.CurrentDomain.GetAssemblies()).ToArray();
            }

            var serializer = new XmlSerializer(typeof(SerialiseType), extraTypes);
            var sr = new StringReader(objectXml);
            var retVal = serializer.Deserialize(sr);
            return (SerialiseType)retVal;
        }

        public static bool IsBaseClassOf(Type type, Type baseClass)
        {
            if (type.BaseType == null)
            {
                return false;
            }

            if (type.BaseType == baseClass)
            {
                return true;
            }

            return IsBaseClassOf(type.BaseType, baseClass);
        }

        public static List<Type> GetExtraTypes<T>(System.Reflection.Assembly[] assemblies)
        {
            var extraTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                if (assembly.FullName == typeof(T).Assembly.FullName)
                {
                    continue;
                }

                foreach (var type in assembly.GetTypes())
                {
                    if (type != typeof(object) && IsBaseClassOf(type, typeof(T)))
                    {
                        extraTypes.Add(type);
                    }
                }
            }

            return extraTypes;
        }


        public static MacroModel GetMacros(string macroPath)
        {
            var macroXml = File.ReadAllText(macroPath);
            var macros = Deserialise<MacroModel>(macroXml);
            return macros;
        }


        public static string GetTestXmlStringFromFilePath(string filePath)
        {
            var xmlString = File.ReadAllText(filePath);
            var path = Path.GetDirectoryName(filePath);
            xmlString = SetUseCases(xmlString, path);
            xmlString = SetSteps(xmlString, path);
            return xmlString;
        }

        private static string SetSteps(string xmlString, string path)
        {
            var includePattern = "(?<=<Include StepsFile=\")(.*)(?=\")";
            var matches = RegexUtilities.MatchPatternAll(xmlString, includePattern);

            foreach (Match match in matches)
            {
                var file = File.ReadAllText($@"{path}\{match.Value}");
                var stepsPattern = @"<StepBase(.|\n)*<\/StepBase>";
                var steps = RegexUtilities.MathPattern(file, stepsPattern);
                var replacePattern = $".*{ match.Value}.*";
                replacePattern = replacePattern.Replace(@"\", @"\\");
                xmlString = RegexUtilities.ReplaceString(xmlString, replacePattern, steps.Value);
            }

            return xmlString;
        }

        private static string SetUseCases(string xmlString, string path)
        {
            var includePattern = "(?<=<Include UseCaseFile=\")(.*)(?=\")";
            var matches = RegexUtilities.MatchPatternAll(xmlString, includePattern);

            foreach (Match match in matches)
            {
                var useCase = File.ReadAllText($@"{path}\{match.Value}");
                var replacePattern = $".*{ match.Value}.*";
                replacePattern = replacePattern.Replace(@"\", @"\\");

                xmlString = RegexUtilities.ReplaceString(xmlString, replacePattern, useCase);
            }

            return xmlString;
        }
    }
}
