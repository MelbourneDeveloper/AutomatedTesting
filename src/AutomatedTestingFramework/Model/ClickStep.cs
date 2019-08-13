namespace AutomatedTestingFramework.Model
{
    /// <summary>
    /// Simulates a mouse click on an element
    /// </summary>
    public class ClickStep : ElementStepBase
    {
        #region Public PRoperties
        public bool IsRightClick { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Serialization only!
        /// </summary>
        public ClickStep()
        {

        }

        public ClickStep(string elementName, string automationId) : this(elementName, automationId, false)
        {
        }

        public ClickStep(string elementName, string automationId, bool isRightClick) : base(automationId)
        {
            IsRightClick = isRightClick;
        }
        #endregion
    }
}
