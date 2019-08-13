using System.ComponentModel;
using System.Xml.Serialization;

namespace AutomatedTestingFramework.Model
{
    /// <summary>
    /// Gets an element by AutomationId or by Name
    /// </summary>
    public class ElementStepBase : StepBase
    {
        #region Public Properties

        /// <summary>
        /// See AutomationId, but this value is used for finding the parent. Note: This only allows for saerching for one parent. It is not currently possible to search in children of children.
        /// </summary>
        public string ParentAutomationId;

        [DefaultValue(AutomationElementSearchProperty.AutomationId)]
        public AutomationElementSearchProperty ParentAutomationElementSearchProperty { get; set; } = AutomationElementSearchProperty.AutomationId;

        /// <summary>
        /// Represents a textual value to search for the Automation Element by. The value should be unique to any descendants of the parent. If the parent is not specified, the App's window will be used. This is used with AutomationElementSearchProperty to find the element.
        /// </summary>
        public string AutomationId { get; set; }

        [DefaultValue(AutomationElementSearchProperty.AutomationId)]
        /// <summary>
        /// Specifies which automation property to search on. It will usually be AutomationId, but could sometimes be Name. In future, there may potentially be other properties to search on.
        /// </summary>
        public AutomationElementSearchProperty AutomationElementSearchProperty { get; set; } = AutomationElementSearchProperty.AutomationId;

        [XmlIgnore]
        public string DisplayName => AutomationId;
        #endregion

        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public ElementStepBase()
        {
        }

        public ElementStepBase(string automationId)
        {
            AutomationId = automationId;
        }
        #endregion
    }
}
