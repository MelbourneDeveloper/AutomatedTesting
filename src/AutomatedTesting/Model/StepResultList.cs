using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedTesting.Model
{
    [Serializable]
    [XmlInclude(typeof(ElementStepResult))]
    [XmlInclude(typeof(MemoryUsageSnapshotResult))]   
    public class StepResultList : List<StepResult>
    {

    }
}
