namespace AutomatedTesting.Model
{
    /// <summary>
    /// A test to see if the value from an Element is an integer and optionally checks to see if it is not zero.
    /// </summary>
    public class IsIntegerTest : ValueTestBase
    {
        public bool CheckForNotZero { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} CheckForNotZero: {CheckForNotZero}";
        }
    }
}
