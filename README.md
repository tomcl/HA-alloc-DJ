# FYP-Allocation-Code

## Dependencies

- Windows operating system
- C++ compiler (MinGW) - Installation tutorial: [MinGW Configuration for Visual Studio Code](https://code.visualstudio.com/docs/cpp/config-mingw)
- .NET 7 - Available at: [Download .NET](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- Run the following command in the first InterfacingCode directory to restore dependencies: ```dotnet restore```

## Running Allocation

Allocation hyperparams can be changed in the HyperParams module, refer to the user guide on what each do.

To run the allocation, follow these steps:

1. Put the necessary CSV files from the User Guide in the `csvs` folder located in the second InterfacingCode directory.
2. Open the terminal/command prompt in the FYP-Allocation-Code directory.
3. Run the `run.bat` file by executing the following command: ```.\run.bat```

## Running Tests
Tests will pass with the default hyperparams changing these can cause tests to fail.
To run the tests, follow these steps:

1. Open the terminal/command prompt in the InterfacingCode.Tests directory.
2. Execute the following command: ```dotnet test```