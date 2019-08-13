using System.Windows.Automation;
using System.Xml.Serialization;

namespace AutomatedTestingFramework.Model
{
    /// <summary>
    /// Result of an ElementStep
    /// </summary>
    public class ElementStepResult : StepResult
    {
        #region Public Properties
        public string ElementName { get; set; }

        public bool GotElement
        {
            get => AutomationElement != null;
            set
            {

            }
        }
        public ElementStepBase ElementStep => (ElementStepBase)Step;

        [XmlIgnore]
        public AutomationElement AutomationElement { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public ElementStepResult()
        {

        }

        public ElementStepResult(ElementStepBase step, AutomationElement automationElement, string elementName) : base(step)
        {
            AutomationElement = automationElement;
            ElementName = elementName;
        }
        #endregion
    }
}
