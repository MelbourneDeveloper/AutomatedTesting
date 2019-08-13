using AutomatedTestingFramework.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AutomatedTestingFramework.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var testPath = args[0];
            var macroPath = args[1];
            var xml = File.ReadAllText(testPath);
            var test = Helpers.Deserialise<Test>(xml);

            var macroXml = File.ReadAllText(macroPath);
            var macros = Helpers.Deserialise<MacroModel>(macroXml);

            var automatedTester = new AutomatedTester(macros);

            Go(test, automatedTester).Wait();
        }

        private static async Task Go(Test test, AutomatedTester automatedTester)
        {
            var resultText = string.Empty;
            try
            {
                var result = await automatedTester.PerformTestAsync(test);
                resultText = Helpers.Serialise(result);
            }
            catch (Exception ex)
            {
                resultText = ex.ToString();
            }

            File.WriteAllText("Result.xml", resultText);
        }
    }
}
