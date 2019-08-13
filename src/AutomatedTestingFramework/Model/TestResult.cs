using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedTestingFramework.Model
{
    public class TestResult
    {
        [XmlIgnore]
        public Test Test { get; set; }

        public List<UseCaseResult> UseCaseResults { get; } = new List<UseCaseResult>();

        /// <summary>
        /// Serialization only
        /// </summary>
        public TestResult()
        {

        }

        public TestResult(Test test)
        {
            Test = test;
        }
    }
}
