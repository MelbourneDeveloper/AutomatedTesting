using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedTesting.Model
{
    [Serializable]
    [XmlInclude(typeof(Macro))]
    public class MacroList : List<Macro>
    {
    }
}
