namespace AutomatedTestingFramework.Model
{
    public class SetValueStep : ElementStepBase, IGetValueStep
    {
        public string Value { get; set; }

        public string ValueKey { get; set; }

        /// <summary>
        /// Serialization only!
        /// </summary>
        public SetValueStep()
        {

        }

        public SetValueStep(string elementName,string automationId, string value) : base(automationId)
        {
            Value = value;
        }

    }
}
