namespace AutomatedTestingFramework.Model
{
    /// <summary>
    /// Gets the main app window
    /// </summary>
    public class GetAppWindowStep : GetElementStep
    {
        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public GetAppWindowStep()
        {

        }

        public GetAppWindowStep(string elementName, string automationId) : base(elementName, automationId)
        {
        }
        #endregion
    }
}
