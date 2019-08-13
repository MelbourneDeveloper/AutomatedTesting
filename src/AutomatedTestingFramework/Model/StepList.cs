using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedTesting.Model
{
    [Serializable]
    [XmlInclude(typeof(ClickStep))]
    [XmlInclude(typeof(SetValueStep))]
    [XmlInclude(typeof(GetAppWindowStep))]
    [XmlInclude(typeof(GetValueStep))]
    [XmlInclude(typeof(InvokeStep))]
    [XmlInclude(typeof(SendKeyStep))]
    [XmlInclude(typeof(SetFocusStep))]
    [XmlInclude(typeof(SelectItemStep))]
    [XmlInclude(typeof(WaitStep))]
    [XmlInclude(typeof(OpenAppStep))]
    [XmlInclude(typeof(CloseAppStep))]
    [XmlInclude(typeof(MemoryUsageSnapshotStep))]
    public class StepList : List<StepBase>
    {
    }
}
