using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CloudQATests
{
    [TestFixture]
    public class AutomationPracticeFormTests
    {
        private IWebDriver driver;
        private const string BaseUrl = "https://app.cloudqa.io/home/AutomationPracticeForm";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(BaseUrl);
            
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Dispose();
        }

        private IWebElement FindInputByLabel(string labelText)
        {
            try
            {
                var label = driver.FindElement(By.XPath($"//label[contains(text(), '{labelText}')]"));
                var forAttribute = label.GetAttribute("for");
                
                if (!string.IsNullOrEmpty(forAttribute))
                {
                    return driver.FindElement(By.Id(forAttribute));
                }
                
                try
                {
                    return label.FindElement(By.XPath("./following-sibling::input[1]"));
                }
                catch
                {
                    return label.FindElement(By.XPath(".//input"));
                }
            }
            catch
            {
                try
                {
                    return driver.FindElement(By.XPath($"//input[@placeholder='{labelText}' or contains(@name, '{labelText}')]"));
                }
                catch
                {
                    throw new NoSuchElementException($"Could not find input field with label: {labelText}");
                }
            }
        }

        private IWebElement FindRadioByLabel(string labelText)
        {
            try
            {
                var label = driver.FindElement(By.XPath($"//label[contains(text(), '{labelText}')]"));
                var forAttribute = label.GetAttribute("for");
                
                if (!string.IsNullOrEmpty(forAttribute))
                {
                    return driver.FindElement(By.Id(forAttribute));
                }
                
                try
                {
                    return label.FindElement(By.XPath("./preceding-sibling::input[@type='radio']"));
                }
                catch
                {
                    return label.FindElement(By.XPath("./following-sibling::input[@type='radio']"));
                }
            }
            catch
            {
                try
                {
                    return driver.FindElement(By.XPath($"//input[@type='radio' and @value='{labelText}']"));
                }
                catch
                {
                    throw new NoSuchElementException($"Could not find radio button with label: {labelText}");
                }
            }
        }

        private SelectElement FindSelectByLabel(string labelText)
        {
            try
            {
                var label = driver.FindElement(By.XPath($"//label[contains(text(), '{labelText}')]"));
                var forAttribute = label.GetAttribute("for");
                
                if (!string.IsNullOrEmpty(forAttribute))
                {
                    try
                    {
                        var selectElement = driver.FindElement(By.Id(forAttribute));
                        return new SelectElement(selectElement);
                    }
                    catch
                    {
                        var combobox = driver.FindElement(By.XPath($"//*[@id='{forAttribute}' and (@role='combobox' or @role='listbox')]"));
                        try
                        {
                            var selectInside = combobox.FindElement(By.XPath(".//select"));
                            return new SelectElement(selectInside);
                        }
                        catch
                        {
                            return new SelectElement(combobox);
                        }
                    }
                }
                
                try
                {
                    var selectElement = label.FindElement(By.XPath("./following-sibling::select[1]"));
                    return new SelectElement(selectElement);
                }
                catch
                {
                    try
                    {
                        var combobox = label.FindElement(By.XPath("./following-sibling::*[@role='combobox' or @role='listbox'][1]"));
                        try
                        {
                            var selectInside = combobox.FindElement(By.XPath(".//select"));
                            return new SelectElement(selectInside);
                        }
                        catch
                        {
                            return new SelectElement(combobox);
                        }
                    }
                    catch
                    {
                        var selectElement = label.FindElement(By.XPath(".//select"));
                        return new SelectElement(selectElement);
                    }
                }
            }
            catch
            {
                try
                {
                    var selectElement = driver.FindElement(By.XPath($"//select[contains(@name, '{labelText}')]"));
                    return new SelectElement(selectElement);
                }
                catch
                {
                    try
                    {
                        var combobox = driver.FindElement(By.XPath($"//*[(@role='combobox' or @role='listbox') and (contains(@aria-label, '{labelText}') or contains(@name, '{labelText}'))]"));
                        try
                        {
                            var selectInside = combobox.FindElement(By.XPath(".//select"));
                            return new SelectElement(selectInside);
                        }
                        catch
                        {
                            return new SelectElement(combobox);
                        }
                    }
                    catch
                    {
                        throw new NoSuchElementException($"Could not find select or combobox element with label: {labelText}");
                    }
                }
            }
        }

        [Test]
        public void TestFirstNameField()
        {
            var testValue = "John";
            var firstNameInput = FindInputByLabel("First Name");
            
            firstNameInput.Clear();
            firstNameInput.SendKeys(testValue);
            
            Assert.That(firstNameInput.GetAttribute("value"), Is.EqualTo(testValue), 
                "First Name field should contain the entered value");
        }

        [Test]
        public void TestGenderField()
        {
            var genderOption = "Male";
            var genderRadio = FindRadioByLabel(genderOption);
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", genderRadio);
            
            if (!genderRadio.Selected)
            {
                genderRadio.Click();
            }
            
            Assert.That(genderRadio.Selected, Is.True, 
                $"Gender radio button '{genderOption}' should be selected");
        }

        [Test]
        public void TestStateDropdown()
        {
            var stateOption = "Canada";
            var stateSelect = FindSelectByLabel("State");
            
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => stateSelect.Options.Count > 0);
            
            stateSelect.SelectByText(stateOption);
            
            Assert.That(stateSelect.SelectedOption.Text, Is.EqualTo(stateOption), 
                $"State dropdown should have '{stateOption}' selected");
        }
    }
}
