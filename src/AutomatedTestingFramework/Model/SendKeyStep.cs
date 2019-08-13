namespace AutomatedTestingFramework.Model
{
    public class SendKeyStep : StepBase
    {
        /// <summary>
        /// Serialization only!
        /// </summary>
        public SendKeyStep()
        {

        }

        public KeyCode KeyCode { get; set; }

        public SendKeyStep(string elementName, string automationId, KeyCode keyCode) : base()
        {
            KeyCode = keyCode;
        }
    }
}