# Project Documentation

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Helper Methods](#helper-methods)
4. [Test Methods](#test-methods)
5. [Element Location Strategy](#element-location-strategy)
6. [How It Works](#how-it-works)

## Project Overview

This test suite automates interactions with the CloudQA Automation Practice Form located at `https://app.cloudqa.io/home/AutomationPracticeForm`. The primary goal is to create tests that remain functional even when the underlying HTML structure changes, making them more maintainable and less brittle than traditional Selenium tests.

### Key Design Decisions

- **Label-Based Selection**: Instead of relying on IDs, classes, or XPath positions, elements are located by their associated label text. This approach is more semantic and stable.
- **Multiple Fallback Strategies**: Each helper method implements several strategies to find elements, ensuring tests work across different HTML structures.
- **No Hard-Coded Selectors**: The code avoids brittle selectors that break when developers refactor the page.

## Architecture

### Class Structure

```
AutomationPracticeFormTests
├── Fields
│   ├── driver (IWebDriver)
│   └── BaseUrl (const string)
├── Setup Methods
│   ├── Setup() - Initializes browser and navigates to form
│   └── TearDown() - Cleans up browser resources
├── Helper Methods
│   ├── FindInputByLabel(string)
│   ├── FindRadioByLabel(string)
│   └── FindSelectByLabel(string)
└── Test Methods
    ├── TestFirstNameField()
    ├── TestGenderField()
    └── TestStateDropdown()
```

### Test Lifecycle

1. **Setup**: Before each test, a new Chrome browser instance is created and navigated to the form URL
2. **Execution**: The test interacts with form elements using helper methods
3. **Assertion**: The test verifies the expected behavior
4. **TearDown**: After each test, the browser is closed and resources are released

## Helper Methods

### FindInputByLabel(string labelText)

Locates a text input field by its associated label.

**Primary Strategy:**
1. Find the label element containing the specified text
2. Check if the label has a `for` attribute pointing to an input ID
3. If found, locate the input by that ID

**Fallback Strategy:**
1. Look for the input as a following sibling of the label
2. If not found, search for an input as a descendant of the label
3. As a last resort, search by placeholder or name attribute

**Example Usage:**
```csharp
var firstNameInput = FindInputByLabel("First Name");
firstNameInput.SendKeys("John");
```

**Why This Works:**
- Labels are semantic HTML elements that rarely change
- The `for` attribute creates a stable relationship between labels and inputs
- Multiple fallback strategies handle different HTML structures

### FindRadioByLabel(string labelText)

Locates a radio button by its associated label text.

**Primary Strategy:**
1. Find the label element containing the specified text
2. Check for a `for` attribute linking to the radio button
3. If found, locate the radio by that ID

**Fallback Strategy:**
1. Search for a radio button as a preceding sibling of the label
2. If not found, search as a following sibling
3. As a last resort, search by value attribute matching the label text

**Example Usage:**
```csharp
var genderRadio = FindRadioByLabel("Male");
genderRadio.Click();
```

**Why This Works:**
- Radio buttons are typically grouped with their labels in predictable structures
- The label text is usually stable even when other attributes change
- Handles both left-to-right and right-to-left label/input arrangements

### FindSelectByLabel(string labelText)

Locates a dropdown/select element or combobox by its associated label.

**Primary Strategy:**
1. Find the label element containing the specified text
2. Check for a `for` attribute linking to a select element
3. If found, wrap it in a SelectElement for interaction

**Advanced Handling:**
- Supports both native `<select>` elements and modern combobox implementations
- Handles comboboxes with `role="combobox"` or `role="listbox"` attributes
- Searches for select elements nested inside combobox containers

**Fallback Strategy:**
1. Look for select as a sibling of the label
2. Search for combobox as a sibling
3. Search for select as a descendant
4. Search by name attribute or aria-label

**Example Usage:**
```csharp
var stateSelect = FindSelectByLabel("State");
stateSelect.SelectByText("Canada");
```

**Why This Works:**
- Modern web applications use various dropdown implementations
- The method handles both traditional selects and modern combobox patterns
- Multiple fallback strategies ensure compatibility across different frameworks

## Test Methods

### TestFirstNameField()

**Purpose:** Validates that text can be entered into the First Name input field.

**Steps:**
1. Locate the First Name input using `FindInputByLabel`
2. Clear any existing value
3. Enter the test value "John"
4. Verify the input contains the entered value

**Assertion:** The input's value attribute matches the entered text.

### TestGenderField()

**Purpose:** Validates radio button selection functionality.

**Steps:**
1. Locate the "Male" radio button using `FindRadioByLabel`
2. Scroll the element into view (ensures it's clickable)
3. Click the radio button if not already selected
4. Verify the radio button is selected

**Assertion:** The radio button's `Selected` property is `true`.

**Why Scroll Into View:**
- Some radio buttons may be below the fold
- Scrolling ensures the element is visible and clickable
- Prevents potential `ElementNotInteractableException` errors

### TestStateDropdown()

**Purpose:** Validates dropdown selection functionality.

**Steps:**
1. Locate the State dropdown using `FindSelectByLabel`
2. Wait for the dropdown to populate with options
3. Select "Canada" from the dropdown
4. Verify the selected option matches the expected value

**Assertion:** The selected option's text matches the expected value.

**Why Wait for Options:**
- Dropdowns may load options asynchronously
- Ensures the dropdown is ready before interaction
- Prevents `NoSuchElementException` when selecting options

## Element Location Strategy

### Why Label-Based Selection?

Traditional Selenium tests often use selectors like:
```csharp
driver.FindElement(By.Id("firstName"))  // Breaks if ID changes
driver.FindElement(By.ClassName("input-field"))  // Breaks if class changes
driver.FindElement(By.XPath("//div[3]/input[2]"))  // Breaks if structure changes
```

These approaches are brittle because:
- IDs are often changed during refactoring
- CSS classes are modified for styling updates
- XPath positions break when elements are reordered

### Our Approach

We use label-based selection:
```csharp
FindInputByLabel("First Name")  // Works as long as label text remains
```

This is more stable because:
- Label text is semantic and rarely changes
- Labels create stable relationships with form controls
- Multiple fallback strategies handle edge cases

### Fallback Strategy Hierarchy

Each helper method implements a hierarchy of strategies:

1. **Primary**: Label with `for` attribute → Element by ID
2. **Secondary**: Label → Sibling element
3. **Tertiary**: Label → Descendant element
4. **Quaternary**: Search by semantic attributes (placeholder, name, aria-label)

This ensures tests continue working even when:
- The HTML structure changes
- Elements are repositioned
- Attributes are modified
- The page is refactored

## How It Works

### Test Execution Flow

```
1. Setup()
   ├── Create ChromeDriver instance
   ├── Navigate to form URL
   └── Wait for page load

2. Test Method
   ├── Call helper method to locate element
   │   ├── Try primary strategy
   │   ├── Try fallback strategies
   │   └── Return element or throw exception
   ├── Interact with element
   └── Assert expected behavior

3. TearDown()
   └── Dispose browser resources
```

### Element Location Process

When `FindInputByLabel("First Name")` is called:

1. **Search for Label:**
   ```xpath
   //label[contains(text(), 'First Name')]
   ```
   This finds any label containing "First Name" text.

2. **Check for Association:**
   ```csharp
   var forAttribute = label.GetAttribute("for");
   ```
   If the label has a `for` attribute, it points to the input's ID.

3. **Locate Input:**
   ```csharp
   driver.FindElement(By.Id(forAttribute))
   ```
   Directly find the input using the ID from the `for` attribute.

4. **Fallback to Sibling:**
   If no `for` attribute exists, look for the input as a sibling:
   ```xpath
   ./following-sibling::input[1]
   ```

5. **Fallback to Descendant:**
   If not a sibling, search within the label's container:
   ```xpath
   .//input
   ```

6. **Final Fallback:**
   Search by semantic attributes:
   ```xpath
   //input[@placeholder='First Name' or contains(@name, 'First Name')]
   ```

### Error Handling

Each helper method uses try-catch blocks to gracefully handle failures:

- **Primary strategy fails**: Try next strategy
- **All strategies fail**: Throw descriptive `NoSuchElementException`
- **Exception message**: Includes the label text that was searched for

This provides clear debugging information when tests fail.

### Browser Management

**Selenium Manager Integration:**
- Automatically detects installed Chrome version
- Downloads matching ChromeDriver version
- Manages driver lifecycle
- No manual driver installation required

**Browser Configuration:**
- Starts maximized for consistent viewport
- Waits for page load completion
- Properly disposes resources after each test

## Best Practices Implemented

1. **Independent Tests**: Each test can run in isolation
2. **Clean Setup/Teardown**: Resources are properly managed
3. **Explicit Waits**: Tests wait for elements to be ready
4. **Descriptive Assertions**: Clear failure messages
5. **Code Reuse**: Helper methods eliminate duplication
6. **Maintainability**: Changes to element location logic are centralized

## Extending the Test Suite

To add a new test:

1. **Identify the element type** (input, radio, select)
2. **Use the appropriate helper method**
3. **Follow the existing test pattern**
4. **Add meaningful assertions**

Example:
```csharp
[Test]
public void TestLastNameField()
{
    var lastNameInput = FindInputByLabel("Last Name");
    lastNameInput.Clear();
    lastNameInput.SendKeys("Doe");
    
    Assert.That(lastNameInput.GetAttribute("value"), Is.EqualTo("Doe"));
}
```

## Troubleshooting

### Test Fails: "Could not find input field with label: X"

**Possible Causes:**
- Label text doesn't match exactly (check for typos or extra spaces)
- Label doesn't exist on the page
- Page structure changed significantly

**Solutions:**
- Verify the label text in the browser
- Check if the element uses a different label structure
- Add a new fallback strategy if needed

### Test Fails: Element Not Interactable

**Possible Causes:**
- Element is hidden or disabled
- Element is outside the viewport
- Element is covered by another element

**Solutions:**
- Add scroll into view (like in `TestGenderField`)
- Wait for element to be enabled
- Check for overlapping elements

### ChromeDriver Version Mismatch

**Solution:**
- Selenium Manager should handle this automatically
- Ensure Chrome browser is up to date
- Clear Selenium Manager cache if issues persist

## Conclusion

This test suite demonstrates how to write maintainable, resilient Selenium tests that adapt to changes in web applications. By focusing on semantic relationships (labels) rather than implementation details (IDs, classes), the tests remain stable and reliable over time.

