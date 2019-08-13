namespace AutomatedTesting.Model
{
    public class WaitStep : StepBase
    {
        /// <summary>
        /// Serialization only!
        /// </summary>
        public WaitStep()
        {

        }

        public int Milliseconds { get; set; }

        public WaitStep(string elementName, string automationId, int milliseconds) : base()
        {
            Milliseconds = milliseconds;
        }
    }
}