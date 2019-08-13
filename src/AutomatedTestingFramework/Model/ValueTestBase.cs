namespace AutomatedTestingFramework.Model
{
    public abstract class ValueTestBase
    {
        public string ValueKey { get; set; }
        public bool IsRequired { get; set; }

        public override string ToString()
        {
            return $"Type: {GetType().Name} ValueKey: {ValueKey} IsRequired: {IsRequired}";
        }

    }
}
