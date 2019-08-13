
namespace AutomatedTestingFramework.Model
{
    /// <summary>
    /// Gets an Element
    /// </summary>
    public class GetElementStep : ElementStepBase
    {
        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public GetElementStep()
        {

        }

        public GetElementStep(string elementName, string automationId) : base(automationId)
        {
        }
        #endregion
    }
}
