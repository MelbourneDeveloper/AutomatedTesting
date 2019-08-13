namespace AutomatedTesting.Model
{
    /// <summary>
    /// Calls the 'Invoke' pattern on an Element. This is usually for simulating a button click.
    /// </summary>
    public class InvokeStep : ElementStepBase
    {
        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public InvokeStep()
        {

        }

        public InvokeStep(string elementName, string automationId) : base(automationId)
        {
        }
        #endregion
    }
}
