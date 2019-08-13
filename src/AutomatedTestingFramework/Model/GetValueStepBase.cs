namespace AutomatedTestingFramework.Model
{
    /// <summary>
    /// Gets the value of an Element and stores it for later use
    /// </summary>
    public abstract class GetValueStepBase : ElementStepBase, IGetValueStep
    {
        #region Public Properties
        public string ValueKey { get; set; }
        public string PropertyName { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public GetValueStepBase()
        {

        }

        public GetValueStepBase(string elementName, string automationId, string valueKey) : base(automationId)
        {
            ValueKey = valueKey;
        }
        #endregion
    }
}
