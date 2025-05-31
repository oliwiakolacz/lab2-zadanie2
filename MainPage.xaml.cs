using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace lab2_zadanie2;

public partial class MainPage : ContentPage
{
    private string currentInput = "";
    private double? firstOperand = null;
    private string currentOperator = null;
    private bool isResultDisplayed = false;

    public MainPage()
    {
        var stopwatch = Stopwatch.StartNew();
        InitializeComponent();
        stopwatch.Stop();

        LogInitializationTime(stopwatch.Elapsed, 1);
    }

    private void OnDigitClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        if (isResultDisplayed)
        {
            currentInput = "";
            isResultDisplayed = false;
        }

        currentInput += button.Text;
        DisplayEntry.Text = currentInput;
    }

    private void OnDecimalClicked(object sender, EventArgs e)
    {
        if (isResultDisplayed)
        {
            currentInput = "";
            isResultDisplayed = false;
        }
        if (!currentInput.Contains("."))
        {
            if (string.IsNullOrEmpty(currentInput))
                currentInput = "0.";
            else
                currentInput += ".";
            DisplayEntry.Text = currentInput;
        }
    }

    private void OnEqualsClicked(object sender, EventArgs e)
    {
        try
        {
            if (firstOperand != null && currentOperator != null && !string.IsNullOrEmpty(currentInput))
            {
                double secondOperand = double.Parse(currentInput);
                double result = Calculate(firstOperand.Value, secondOperand, currentOperator);
                DisplayEntry.Text = result.ToString();
                firstOperand = result;
                currentInput = "";
                currentOperator = null;
                isResultDisplayed = true;
            }
        }
        catch (Exception ex)
        {
            DisplayEntry.Text = "Błąd";
            LogError(ex);
            isResultDisplayed = true;
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        currentInput = "";
        firstOperand = null;
        currentOperator = null;
        DisplayEntry.Text = "";
        isResultDisplayed = false;
    }
	
	private void OnOperatorClicked(object sender, EventArgs e)
	{
    	var button = sender as Button;
    	if (button == null) return;

    	try
    	{
        	if (!string.IsNullOrEmpty(currentInput))
        	{
            	if (firstOperand == null)
            	{
                	firstOperand = double.Parse(currentInput.Replace(',', '.'), CultureInfo.InvariantCulture);
            	}
            	else if (currentOperator != null)
            	{
                	firstOperand = Calculate(
                    firstOperand.Value,
                    double.Parse(currentInput.Replace(',', '.'), CultureInfo.InvariantCulture),
                    currentOperator
                	);
                	DisplayEntry.Text = firstOperand.ToString();
            	}
        	}
        	currentOperator = button.Text;
        	currentInput = "";
        	isResultDisplayed = false;
    	}
    	catch (Exception ex)
    	{
        	DisplayEntry.Text = "Błąd";
        	LogError(ex);
        	isResultDisplayed = true;
    	}
	}


    private double Calculate(double a, double b, string op)
	{
		switch (op)
		{
			case "+": return a + b;
			case "−": return a - b;
			case "×": return a * b;
			case "÷":
				if (b == 0)
					throw new DivideByZeroException("Dzielenie przez zero!");
				return a / b;
			default: throw new InvalidOperationException("Nieznany operator.");
		}
	}

    private void LogInitializationTime(TimeSpan elapsed, int millisecLimit)
    {
        if (elapsed.TotalMilliseconds >= millisecLimit)
        {
                var psi = new ProcessStartInfo
                {
                    FileName = "logger",
                    Arguments = $"\"[CSharpApp] ERROR: Za długi czas ładowania aplikacji {elapsed.TotalMicroseconds} millisekund\"",
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
        }
    }

    private void LogError(Exception ex)
    {
        DisplayAlert("Błąd", ex.Message, "OK");
    }
}
