namespace AutomatedTesting.Model
{
    /// <summary>
    /// A test to see if the value from an Element is a date, and that date is today's date
    /// </summary>
    public class IsDateTodayTest : ValueTestBase
    {
        public string DateFormat { get; set; }
    }
}
