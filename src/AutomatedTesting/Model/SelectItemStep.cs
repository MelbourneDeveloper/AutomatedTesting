namespace AutomatedTesting.Model
{
    public class SelectItemStep : ElementStepBase, IGetValueStep
    {
        public string Value { get; set; }
        public string ValueKey { get; set; }

        /// <summary>
        /// Serialization only!
        /// </summary>
        public SelectItemStep()
        {

        }

        public SelectItemStep(string elementName,string automationId, string value) : base(automationId)
        {
            Value = value;
        }
    }
}
