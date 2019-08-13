namespace AutomatedTesting.Model
{
    public class RepeaterBundle
    {
        #region Public Properties
        public string RepeaterName { get; set; }
        public MacroModel MacroModel { get; set; } = new MacroModel();
        #endregion
    }
}
