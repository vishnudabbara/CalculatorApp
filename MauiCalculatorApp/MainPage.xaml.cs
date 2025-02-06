using System;
using System.Data;

namespace MauiCalculatorApp
{
    public partial class MainPage : ContentPage
    {
        private string currentInput = "";
        private string expression = "";
        private bool isNewOperation = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnNumberClicked(object sender, EventArgs e)
        {
            if (sender is not Button button || DisplayLabel == null)
                return;

            if (isNewOperation)
            {
                currentInput = "";
                isNewOperation = false;
            }

            string buttonText = button.Text;

            if (buttonText == "." && currentInput.Contains("."))
                return; // Prevent multiple decimals

            currentInput += buttonText;
            DisplayLabel.Text = currentInput;
        }

        private void OnOperatorClicked(object sender, EventArgs e)
        {
            if (sender is not Button button || DisplayLabel == null || ExpressionLabel == null)
                return;

            if (isNewOperation)
            {
                expression = DisplayLabel.Text; // Use the last result for the next operation
                isNewOperation = false;
            }

            if (!string.IsNullOrEmpty(currentInput))
            {
                // Prevent appending current input again if it's already used
                if (!expression.EndsWith(" "))
                {
                    expression += " " + currentInput;
                }
                currentInput = "";
            }

            // Prevent multiple operators (e.g., "17 + +")
            if (!string.IsNullOrEmpty(expression) && expression.EndsWith(" "))
            {
                expression = expression.TrimEnd(); // Remove extra space before appending new operator
            }

            expression += " " + button.Text + " ";
            ExpressionLabel.Text = expression;
            DisplayLabel.Text = "0";
        }

        private void OnEqualClicked(object sender, EventArgs e)
        {
            if (DisplayLabel == null || ExpressionLabel == null)
                return;

            if (!string.IsNullOrEmpty(currentInput))
            {
                expression += " " + currentInput;
                ExpressionLabel.Text = expression;
            }

            try
            {
                string safeExpression = expression.Replace("×", "*").Replace("÷", "/");

                // ** FIX: Prevents double appending of the result **
                double result = Convert.ToDouble(new DataTable().Compute(safeExpression, null));

                DisplayLabel.Text = result.ToString();

                // ** FIX: Reset state to prevent duplication **
                currentInput = "";
                expression = result.ToString(); // Store result for next calculation
                isNewOperation = true;
            }
            catch
            {
                DisplayLabel.Text = "Error";
                currentInput = "";
                expression = "";
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            if (DisplayLabel == null || ExpressionLabel == null)
                return;

            currentInput = "";
            expression = "";
            DisplayLabel.Text = "0";
            ExpressionLabel.Text = "";
        }

        private void OnNegateClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentInput))
                return;

            if (currentInput.StartsWith("-"))
                currentInput = currentInput[1..];
            else
                currentInput = "-" + currentInput;

            DisplayLabel.Text = currentInput;
        }
    }
}
