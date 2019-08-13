using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedTesting.Model
{
    [Serializable]
    [XmlInclude(typeof(IsIntegerTest))]
    [XmlInclude(typeof(IsDateTodayTest))]
    [XmlInclude(typeof(EqualsValueTest))]   
    public class ValueTestList : List<ValueTestBase>
    {
    }
}
