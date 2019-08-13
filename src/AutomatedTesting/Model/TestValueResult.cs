using System.Xml.Serialization;

namespace AutomatedTesting.Model
{
    public class TestValueResult
    {
        #region Public Properties
        public bool IsRequired { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess
        {
            get => string.IsNullOrEmpty(ErrorMessage);
            set
            {

            }
        }

        public string Type { get; set; }
        public string ValueKey { get; set; }
        public string InfoItem { get; set; }

        [XmlIgnore]
        public ValueTestBase ValueTest { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Serialization only!
        /// </summary>
        public TestValueResult()
        {
        }

        /// </summary>
        public TestValueResult(ValueTestBase valueTest)
        {
            ValueTest = valueTest;
            Type = valueTest.GetType().Name;
            ValueKey = valueTest.ValueKey;
        }

        #endregion

    }
}
