using System;
using System.Xml.Serialization;

namespace AutomatedTestingFramework.Model
{
    public class StepResult
    {
        #region Public Properties
        public string Type { get; set; }
        public bool IsSuccess { get; set; }
        public string Value { get; set; }
        public string ValueKey { get; set; }

        public DateTime StepStartTime { get; set; }
        public DateTime StepFinishTime { get; set; }

        public string ExceptionString
        {
            get => Exception?.ToString();
            set
            {

            }
        }

        public string InfoItem { get; set; }

        [XmlIgnore]
        public StepBase Step { get; set; }

        [XmlIgnore]
        public Exception Exception { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Serialization only
        /// </summary>
        public StepResult()
        {

        }

        public StepResult(StepBase step)
        {
            Step = step;
            Type = step.GetType().Name;
        }
        #endregion
    }
}
