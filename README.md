# CloudQA Automation Practice Form Tests

A robust Selenium WebDriver test suite for the CloudQA Automation Practice Form, designed with resilience and maintainability in mind.

## Overview

This project implements automated end-to-end tests for form interactions on the CloudQA Automation Practice Form. The tests are built using C# and Selenium WebDriver, with a focus on creating stable, maintainable test code that adapts to changes in the web application.

## Features

- **Resilient Element Location**: Tests use label-based selectors that remain stable even when HTML attributes or element positions change
- **Automatic Driver Management**: Selenium Manager automatically downloads and configures the correct ChromeDriver version
- **Comprehensive Coverage**: Tests validate text inputs, radio buttons, and dropdown selections
- **Clean Architecture**: Well-structured helper methods promote code reuse and maintainability

## Prerequisites

- .NET 10.0 SDK or later
- Google Chrome browser installed
- Internet connection (for Selenium Manager to download ChromeDriver)

## Quick Start

### Installation

```bash
# Restore NuGet packages
dotnet restore

# Build the project
dotnet build
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run a specific test
dotnet test --filter "FullyQualifiedName~TestFirstNameField"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Test Coverage

| Test | Description |
|------|-------------|
| `TestFirstNameField` | Validates text input functionality for the First Name field |
| `TestGenderField` | Tests radio button selection for gender options |
| `TestStateDropdown` | Verifies dropdown selection for state/country options |

## Project Structure

```
.
├── AutomationPracticeFormTests.cs    # Main test class
├── CloudQATests.csproj              # Project configuration
├── README.md                         # This file
└── PROJECT_DOCUMENTATION.md         # Detailed technical documentation
```

## Technology Stack

- **.NET 10.0**: Target framework
- **NUnit 3.14.0**: Testing framework
- **Selenium WebDriver 4.15.0**: Browser automation
- **Selenium Manager**: Automatic driver management (built into Selenium 4.6+)

## Design Philosophy

The test suite is designed with the following principles:

1. **Stability Over Speed**: Tests prioritize reliability and maintainability
2. **Label-Based Selection**: Elements are located by their semantic labels rather than fragile IDs or classes
3. **Graceful Degradation**: Multiple fallback strategies ensure tests continue working even when page structure changes
4. **Separation of Concerns**: Helper methods encapsulate element location logic, keeping tests clean and readable

## Contributing

When adding new tests:

1. Use the existing helper methods (`FindInputByLabel`, `FindRadioByLabel`, `FindSelectByLabel`)
2. Follow the established naming conventions
3. Ensure tests are independent and can run in any order
4. Update documentation when adding new functionality


