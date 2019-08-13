using System.Collections.Generic;

namespace AutomatedTesting.Model
{
    public class UseCase
    {
        #region Public Properties
        public string Name { get; set; }
        public StepList Steps { get; } = new StepList();
        public ValueTestList ValueTests { get; } = new ValueTestList();
        public List<RepeaterBundle> RepeaterBundles { get; } = new List<RepeaterBundle>();
        #endregion

        #region Constructors
        /// <summary>
        /// Serialization Only!
        /// </summary>
        public UseCase()
        {
        }

        public UseCase(string name)
        {
            Name = name;
        }

        public UseCase(string name, StepBase step) : this(name)
        {
            Steps.Add(step);
        }
        #endregion
    }
}
