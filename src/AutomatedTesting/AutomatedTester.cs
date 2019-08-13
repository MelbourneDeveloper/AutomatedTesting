using AutomatedTesting.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using static System.Windows.Automation.AutomationElement;

namespace AutomatedTesting
{
    public class AutomatedTester
    {
        #region Private Fields
        private Process _Process;
        private UseCaseResult _CurrentUseCaseResult;
        private RepeaterBundle _CurrentRepeaterBundle;
        private TestResult _CurrentTestResult;
        private readonly RegexUtilities _RegexUtilities = new RegexUtilities();
        private Func<AutomationElement, string, AutomationElementSearchProperty, TimeSpan, Task<AutomationElement>> _GetAutomationElementByPropertyConditionFunc => async (parent, id, automationElementSearchProperty, timeout) => await GetElementFromParentAsync(parent, id, automationElementSearchProperty == AutomationElementSearchProperty.AutomationId ? AutomationIdProperty : NameProperty, timeout);
        private readonly Delegate _GetValueFunc = new Func<AutomationElement, Task<string>>(GetAutomationElementValueAsync);
        private readonly Delegate _GetValueByPropertyFunc = new Func<AutomationElement, string, Task<string>>(GetAutomationElementValueByPropertyAsync);
        private readonly Delegate _SelectItemFunc = new Func<AutomationElement, string, Task<bool>>(SelectItemByValueAsync);
        private readonly Delegate _SetFocusFunc = new Func<AutomationElement, Task<bool>>(SetFocus);
        private AutomationElement _AppElement { get; set; }
        private TimeSpan _ElementTimeout { get; set; } = new TimeSpan(0, 0, 5);
        #endregion

        #region Public Properties
        public MacroModel Macros { get; set; } = new MacroModel();
        #endregion

        #region Public Static Properties
        /// <summary>
        /// This is a hack. Probably should be removed in favour of passing a context object around
        /// </summary>
        public static AutomatedTester Current { get; private set; }
        #endregion

        #region Constructor
        public AutomatedTester(MacroModel macros)
        {
            Macros = macros;
        }
        #endregion

        #region Public Methods
        public Task<TestResult> PerformTestAsync(Test test)
        {
            return PerformTestAsync(test, null);
        }

        public async Task<TestResult> PerformTestAsync(Test test, List<string> useCaseNames)
        {
            Current = this;

            ValidateTest(test);

            _CurrentTestResult = new TestResult(test);

            IEnumerable<UseCase> useCases = test.UseCases;

            if (useCaseNames != null && useCaseNames.Count > 0)
            {
                foreach (var useCaseName in useCaseNames)
                {
                    var useCase = useCases.FirstOrDefault(uc => uc.Name == useCaseName);
                    await PerformUseCase(useCase);
                }
            }
            else
            {
                foreach (var useCase in test.UseCases)
                {
                    await PerformUseCase(useCase);
                }
            }

            return _CurrentTestResult;
        }
        #endregion

        #region Private Methods
        private async Task PerformUseCase(UseCase useCase)
        {
            var repeaterBundles = useCase.RepeaterBundles?.Count > 0 ? useCase.RepeaterBundles : new List<RepeaterBundle> { new RepeaterBundle { RepeaterName = "No Repeat" } };

            for (var i = 0; i < repeaterBundles.Count; i++)
            {
                var repeaterBundle = repeaterBundles[i];
                _CurrentRepeaterBundle = repeaterBundle;

                var steps = useCase.Steps;
                var valueTests = useCase.ValueTests;

                //TODO: What is going on here? Why are we creating a variable and then retrieving it from somewhere else? Is there are reason for this?
                _CurrentUseCaseResult = new UseCaseResult(useCase, repeaterBundle.RepeaterName);

                //TODO: This is really nasty. There is no way to get test values across repeater bundles. Every time a Use Case repeats, the test values are cleared out. But the test values won't be cleared out if there are no repeats
                if (i > 0) _CurrentUseCaseResult.TestValues.Clear();

                await PerformStepsAsync(steps, valueTests);

                //TODO: This really not good. This should be part of validating the script but is just here because it is easy.
                var valueTestBase = useCase.ValueTests.FirstOrDefault(vt => !_CurrentUseCaseResult.TestValueResults.Select(tvr => tvr.ValueKey).Contains(vt.ValueKey));
                if (valueTestBase != null)
                {
                    throw new Exception($"The value test with a value key of {valueTestBase.ValueKey} does not have a step to get the value");
                }
            }
        }

        private async Task PerformStepsAsync(StepList steps, ValueTestList valueTests)
        {
            _CurrentTestResult.UseCaseResults.Add(_CurrentUseCaseResult);

            foreach (var step in steps)
            {
                var stepResult = await RunStepAsync(step);

                _CurrentUseCaseResult.StepResults.Add(stepResult);

                if (step is IGetValueStep getValueStep)
                {
                    if (string.IsNullOrEmpty(getValueStep.ValueKey)) continue;

                    _CurrentUseCaseResult.TestValues.Add(getValueStep.ValueKey, stepResult.Value);

                    var valueTestsForStep = valueTests.Where(vt => vt.ValueKey == getValueStep.ValueKey);
                    foreach (var valueTest in valueTestsForStep)
                    {
                        var value = _CurrentUseCaseResult.TestValues[valueTest.ValueKey];

                        var dependentValue = GetDependentValue(value);

                        var item = new TestValueResult(valueTest);
                        item.ErrorMessage = CheckValueTest(valueTest, dependentValue, item);

                        _CurrentUseCaseResult.TestValueResults.Add(item);

                        if (valueTest.IsRequired && !string.IsNullOrEmpty(item.ErrorMessage)) throw new Exception($"The test cannot continue because a required Value Test failed.\r\n {valueTest.ToString()}");
                    }
                }
            }
        }

        private void ValidateTest(Test test)
        {
            foreach (var useCase in test.UseCases)
            {
                var valueTestKeys = new List<string>();

                foreach (var valueStep in useCase.Steps.Where(s => s is IGetValueStep).Cast<IGetValueStep>())
                {
                    if (string.IsNullOrEmpty(valueStep.ValueKey))
                    {
                        continue;
                    }

                    if (valueTestKeys.Contains(valueStep.ValueKey))
                    {
                        throw new Exception($"The Use Case {useCase.Name} contains duplicate steps with value key '{valueStep.ValueKey}'");
                    }

                    valueTestKeys.Add(valueStep.ValueKey);
                }
            }
        }

        private string CheckValueTest(ValueTestBase valueTest, string value, TestValueResult testValueResult)
        {
            string expectedValue = null;
            string retVal = null;

            switch (valueTest)
            {
                case IsIntegerTest isIntegerTest:
                {
                    var isInteger = int.TryParse(value, out var valueAsInteger);
                    if (!isInteger) return $"The value '{value}' is not an integer";
                    if (isIntegerTest.CheckForNotZero && valueAsInteger == 0) return $"The value is zero";
                    expectedValue = "[Some whole number]";
                    break;
                }
                case IsDateTodayTest _ when string.IsNullOrEmpty(value):
                    retVal = "The value is empty";
                    break;
                case IsDateTodayTest isDateTodayTest:
                {
                    DateTime dateTime;
                    try
                    {
                        dateTime = DateTime.ParseExact(value, isDateTodayTest.DateFormat, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return $"The value '{value}' was not in the DateTime format {isDateTodayTest.DateFormat}";
                    }

                    var date = DateTime.Now.Date;
                    expectedValue = date.ToString();
                    retVal = dateTime.Date == date ? null : $"The date {dateTime.Date} is not today";
                    break;
                }
                case EqualsValueTest equalsValueTest:
                    expectedValue = GetDependentValue(equalsValueTest.Value);
                    retVal = expectedValue == value ? null : $"Expected value: '{expectedValue}' Actual Value: '{value}'";
                    break;
                default:
                    throw new NotImplementedException();
            }

            testValueResult.InfoItem = $"Expected: {expectedValue} Actual: {value}";

            return retVal;
        }

        private async Task<StepResult> RunStepAsync(StepBase step)
        {
            var stepResult = new StepResult(step);
            var startDateTime = DateTime.Now;
            stepResult.StepStartTime = startDateTime;

            try
            {
                switch (step)
                {
                    case OpenAppStep openAppStep:
                    {
                        var appPath = GetDependentValue(openAppStep.AppPath);
                        _Process = Process.Start(appPath);
                        stepResult.IsSuccess = true;
                        stepResult.InfoItem = appPath;
                        break;
                    }
                    case CloseAppStep closeAppStep:
                    {
                        var appPath = GetDependentValue(closeAppStep.AppPath);
                        _Process.Kill();
                        _Process.WaitForExit();
                        stepResult.IsSuccess = true;
                        stepResult.InfoItem = appPath;
                        break;
                    }
                    case GetAppWindowStep getAppWindowStep:
                        stepResult = await RunElementStepAsync(RootElement, getAppWindowStep);
                        stepResult.StepStartTime = startDateTime;
                        var getAppWindowStepResult = (ElementStepResult)stepResult;
                        _AppElement = getAppWindowStepResult.AutomationElement;
                        stepResult.IsSuccess = _AppElement != null;
                        stepResult.InfoItem = getAppWindowStepResult.ElementName;
                        break;
                    case ElementStepBase elementStepBase:
                    {
                        if (_AppElement == null)
                        {
                            throw new Exception("The app window has not been obtained.");
                        }

                        stepResult = await RunElementStepAsync(_AppElement, elementStepBase);
                        stepResult.StepStartTime = startDateTime;
                        var elementStepResult = (ElementStepResult)stepResult;
                        stepResult.IsSuccess = elementStepResult.AutomationElement != null;
                        stepResult.InfoItem = $"Element: {elementStepResult.ElementName}";
                        break;
                    }
                    case IExecutable executable:
                        stepResult = new StepResult(step)
                        {
                            StepStartTime = DateTime.Now
                        };
                        var returnValue = await executable.Execute();
                        stepResult.Value = returnValue.ToString();
                        //Correct?
                        stepResult.InfoItem = stepResult.Value;
                        stepResult.IsSuccess = true;
                        break;
                    case SendKeyStep sendKeyStep:
                        WindowsAPI.SendKeyPress(sendKeyStep.KeyCode);
                        stepResult.IsSuccess = true;
                        break;

                    case WaitStep waitStep:
                        await Task.Delay(waitStep.Milliseconds);
                        stepResult.InfoItem = $"{ waitStep.Milliseconds} milliseconds";
                        stepResult.IsSuccess = true;
                        break;
                    case MemoryUsageSnapshotStep memoryUsageSnapshotStep:
                        _Process?.Refresh();
                        var memstepResult = new MemoryUsageSnapshotResult(step, _Process?.WorkingSet64);
                        stepResult.StepStartTime = startDateTime;
                        stepResult = memstepResult;
                        stepResult.InfoItem = $"Memory Usage: { _Process?.WorkingSet64} ";
                        stepResult.IsSuccess = _Process != null;
                        break;

                }
            }
            catch (Exception ex)
            {
                stepResult.Exception = ex;
                stepResult.IsSuccess = false;
            }

            stepResult.StepFinishTime = DateTime.Now;

            return stepResult;
        }

        private async Task<ElementStepResult> RunElementStepAsync(AutomationElement parentElement, ElementStepBase step)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }

            var id = GetDependentValue(step.AutomationId);

            //TODO: This allows for searching for a child in a specific parent. This will cover most bases (mainly TreeViews), but it doesn't allow for searching in children of children. If this is needed, we will need some recursion here.

            if (!string.IsNullOrEmpty(step.ParentAutomationId))
            {
                var parentId = GetDependentValue(step.ParentAutomationId);
                parentElement = await RetryUntilTimeoutAsync<AutomationElement>(_GetAutomationElementByPropertyConditionFunc, parentElement, parentId, step.ParentAutomationElementSearchProperty, _ElementTimeout);
            }

            var automationElement = await RetryUntilTimeoutAsync<AutomationElement>(_GetAutomationElementByPropertyConditionFunc, parentElement, id, step.AutomationElementSearchProperty, _ElementTimeout);

            var stepResult = new ElementStepResult(step, automationElement, id);

            if (automationElement == null) return stepResult;

            switch (step)
            {
                case InvokeStep _:
                {
                    var invokePattern = automationElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    invokePattern.Invoke();
                    stepResult.IsSuccess = true;
                    break;
                }
                case SetValueStep setValueStep:
                {
                    var valuePattern = automationElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    var value = GetDependentValue(setValueStep.Value);

                    if (!string.IsNullOrEmpty(setValueStep.ValueKey))
                    {
                        stepResult.ValueKey = setValueStep.ValueKey;
                        stepResult.Value = value;
                    }

                    valuePattern.SetValue(value);
                    stepResult.IsSuccess = true;
                    break;
                }
                case SetFocusStep _:
                    stepResult.IsSuccess = await RetryUntilTimeoutAsync<bool>(_SetFocusFunc, automationElement);
                    break;
                case ClickStep clickStep:
                {
                    var clickablePoint = automationElement.GetClickablePoint();
                    WindowsAPI.MouseClick((int)clickablePoint.X, (int)clickablePoint.Y, clickStep.IsRightClick);
                    stepResult.IsSuccess = true;
                    break;
                }
                case SelectItemStep selectItemStep:

                    var value2 = GetDependentValue(selectItemStep.Value);

                    if (!string.IsNullOrEmpty(selectItemStep.ValueKey))
                    {
                        stepResult.ValueKey = selectItemStep.ValueKey;
                        stepResult.Value = value2;
                    }

                    stepResult.IsSuccess = await RetryUntilTimeoutAsync<bool>(_SelectItemFunc, automationElement, value2);

                    break;
                case GetValueStep getValueStep:

                    stepResult.ValueKey = getValueStep.ValueKey;

                    if (getValueStep.PropertyName != null)
                    {
                        stepResult.Value = await RetryUntilTimeoutAsync<string>(_GetValueByPropertyFunc, automationElement, getValueStep.PropertyName);
                    }
                    else
                    {
                        stepResult.Value = await RetryUntilTimeoutAsync<string>(_GetValueFunc, automationElement);
                    }

                    stepResult.IsSuccess = stepResult.Value != null;
                    break;
            }

            return stepResult;
        }

        private async Task<T> RetryUntilTimeoutAsync<T>(Delegate func, params object[] args)
        {
            var startTime = DateTime.Now;

            object automationElement = null;

            while (automationElement == null && DateTime.Now < startTime + _ElementTimeout)
            {
                try
                {
                    var task = (Task<T>)func.DynamicInvoke(args);
                    automationElement = await task;
                }
                catch (Exception)
                {
                    //TODO: Logging
                }

                if (automationElement == null)
                {
                    await Task.Delay(1000);
                }
            }

            return automationElement != null ? (T)automationElement : default;
        }

        public string GetDependentValue(string values)
        {
            var stringBuilder = new StringBuilder();

            string[] tokens = null;

            if (values == null)
                tokens = new string[0];
            else
                tokens = values.Split('|');

            foreach (var value in tokens)
            {
                var dependentValueArgument = RegexUtilities.GetDependentValueArgument(value);

                switch (dependentValueArgument)
                {
                    case null:
                        stringBuilder.Append(value);
                        break;
                    case ValueArgument valueArgument:

                        var useCaseResult = GetCurrentUseCaseResult(valueArgument.UseCaseName);

                        if (!useCaseResult.TestValues.ContainsKey(valueArgument.ValueKey))
                        {
                            //TODO: What to do here? We don't have a value so this test is already invalid and shouldn't be running..
                            //We need to have already validated this value before this part of the test was called...
                            throw new NotImplementedException("Fix this");
                        }

                        stringBuilder.Append(useCaseResult.TestValues[valueArgument.ValueKey]);
                        break;
                    case MacroArgument macroArgument:
                    {
                        var macro = Macros.Macros.FirstOrDefault(m => m.Key == macroArgument.Key);
                        if (macro == null)
                        {
                            throw new Exception($"The macro {macroArgument.Key} is not defined");
                        }
                        stringBuilder.Append(macro.Value);
                        break;
                    }

                    //TODO: This code is exactly the same as above
                    case RepeaterMacroArgument repeaterMacroArgument:
                    {
                        var macro = _CurrentRepeaterBundle.MacroModel.Macros.FirstOrDefault(m => m.Key == repeaterMacroArgument.Key);
                        if (macro == null)
                        {
                            throw new Exception($"The macro {repeaterMacroArgument.Key} is not defined");
                        }
                        stringBuilder.Append(macro.Value);
                        break;
                    }

                    case string theString:
                        stringBuilder.Append(theString);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return stringBuilder.ToString();
        }

        private UseCaseResult GetCurrentUseCaseResult(string useCaseName)
        {
            UseCaseResult useCaseResult;
            if (!string.IsNullOrEmpty(useCaseName))
            {
                useCaseResult = _CurrentTestResult.UseCaseResults.FirstOrDefault(ucr => ucr.UseCase.Name == useCaseName);
                if (useCaseResult == null)
                {
                    throw new Exception($"The use case {useCaseName} does not exist or has not been run.");
                }
            }
            else
            {
                useCaseResult = _CurrentUseCaseResult;
            }

            return useCaseResult;
        }
        #endregion

        #region Private Static Methods
        private static Task<AutomationElement> GetElementFromParentAsync(AutomationElement parent, string id, AutomationProperty automationProperty, TimeSpan timeout)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var func = new Func<AutomationElement>(() => { return parent.FindFirst(TreeScope.Descendants, new PropertyCondition(automationProperty, id)); });
            var task = Task.Run(func);

            if (!task.Wait(timeout))
            {
                throw new Exception($"Attempted to get element '{id}' but the attempt timed out after {timeout}");
            }

            return task;
        }

        private static Task<string> GetAutomationElementValueAsync(AutomationElement automationElement)
        {
            return Task.Run(() =>
            {
                string value;

                switch (automationElement.Current.ClassName)
                {
                    case "TextBlock":
                        value = automationElement.Current.Name;
                        break;
                    case "ComboBox":
                        value = GetListSelectedText(automationElement);
                        break;
                    case "CheckBox":
                        var togglePattern = automationElement.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
                        value = togglePattern.Current.ToggleState.ToString();
                        break;
                    default:
                        //TODO: Other classes. This assumes Texbox
                        var valuePattern = automationElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        value = valuePattern.Current.Value;
                        break;
                }

                return value;
            });
        }

        private static Task<string> GetAutomationElementValueByPropertyAsync(AutomationElement automationElement, string propertyName)
        {
            return Task.Run(() =>
            {
                string retVal = null;

                if (propertyName == "*GridRowCount*")
                {
                    if (automationElement.Current.ControlType.LocalizedControlType == "datagrid")
                    {
                        automationElement.TryGetCurrentPattern(GridPattern.Pattern, out var gridPattern);
                        retVal = ((GridPattern)gridPattern).Current.RowCount.ToString();
                    }
                    else
                    {
                        throw new Exception($"The element {automationElement?.Current.AutomationId} is not a datagrid");
                    }
                }
                else
                {
                    var propertyInformation = typeof(AutomationElementInformation).GetProperty(propertyName);
                    retVal = propertyInformation.GetValue(automationElement.Current, null).ToString();
                }

                return retVal;
            });
        }

        private static Task<bool> SelectItemByValueAsync(AutomationElement comboBoxAutomationElement, string value)
        {
            return Task.Run(() =>
            {
                try
                {
                    var expandCollapsePattern = (ExpandCollapsePattern)comboBoxAutomationElement.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                    expandCollapsePattern.Expand();

                    var listItems = comboBoxAutomationElement.FindAll(TreeScope.Descendants, new PropertyCondition(ControlTypeProperty, ControlType.ListItem));

                    foreach (AutomationElement listItem in listItems)
                    {
                        var text = GetListItemText(listItem);

                        if (text != value || !listItem.TryGetCurrentPattern(SelectionItemPatternIdentifiers.Pattern, out var objPattern)) continue;
                        if (!(objPattern is SelectionItemPattern selectionItemPattern)) return false;
                        selectionItemPattern.Select();
                        return true;
                    }
                }
                catch (Exception)
                {
                    //TODO: Logging or whatever...
                }

                return false;
            });
        }

        private static Task<bool> SetFocus(AutomationElement automationElement)
        {
            return Task.Run(() =>
            {
                try
                {
                    automationElement.SetFocus();
                    return true;
                }
                catch
                {
                    //TODO: Logging
                }

                return false;
            });
        }

        private static string GetListItemText(AutomationElement listItem)
        {
            var listItemTextElements = listItem.FindAll(TreeScope.Descendants, new PropertyCondition(ControlTypeProperty, ControlType.Text));
            var textBlock = listItemTextElements[0];
            return textBlock.Current.Name;
        }

        private static string GetListSelectedText(AutomationElement listAutomationElement)
        {
            try
            {
                var expandCollapsePattern = (ExpandCollapsePattern)listAutomationElement.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                expandCollapsePattern.Expand();

                var listItems = listAutomationElement.FindAll(TreeScope.Descendants, new PropertyCondition(ControlTypeProperty, ControlType.ListItem));

                foreach (AutomationElement listItem in listItems)
                {
                    var selectionItemPattern = (SelectionItemPattern)listItem.GetCurrentPattern(SelectionItemPatternIdentifiers.Pattern);
                    if (selectionItemPattern.Current.IsSelected) return GetListItemText(listItem);
                }

                return null;
            }
            catch (Exception)
            {
                //TODO: Logging or whatever...
            }

            return null;
        }
        #endregion
    }
}
