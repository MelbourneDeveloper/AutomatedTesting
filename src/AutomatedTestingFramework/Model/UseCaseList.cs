using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutomatedTesting.Model
{
    [Serializable]
    [XmlInclude(typeof(UseCase))]
    public class UseCaseList : List<UseCase>
    {
    }
}
