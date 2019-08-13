using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace AutomatedTestingFramework.Model
{
    public class UseCaseResult
    {
        #region Public Properties
        public string UseCaseName { get; set; }

        public string RepeaterBundleName { get; set; }
        public StepResultList StepResults { get; } = new StepResultList();
        public List<TestValueResult> TestValueResults { get; } = new List<TestValueResult>();

        public bool IsSuccess
        {
            get => StepResults.FirstOrDefault(s => !s.IsSuccess) == null && TestValueResults.FirstOrDefault(s => !s.IsSuccess) == null;
            set
            {

            }
        }

        [XmlIgnore]
        public UseCase UseCase { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> TestValues { get; } = new Dictionary<string, string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Serialization only
        /// </summary>
        public UseCaseResult()
        {

        }

        public UseCaseResult(UseCase useCase, string repeaterBundleName)
        {
            UseCase = useCase;
            UseCaseName = useCase.Name;
            RepeaterBundleName = repeaterBundleName;
        }
        #endregion
    }
}
