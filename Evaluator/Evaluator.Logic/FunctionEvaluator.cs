using System.Globalization;
using System.Text;

namespace Evaluator.Logic;

public class FunctionEvaluator
{
    public static double Evalute(string infix)
    {
        var postfix = ToPostfix(infix); 
        return Calculate(postfix);     
    }

    // Parameter type changed from string to List<string>
    private static double Calculate(List<string> postfix)
    {
        var stack = new Stack<double>();

        foreach (var token in postfix)
        {
            // Added this to recognize multi-digit and decimal numbers
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
            {
                stack.Push(number); // Push number to stack
            }
            else if (IsOperator(token)) // Added this to check for operators
            {
                var operand2 = stack.Pop();
                var operand1 = stack.Pop();
                stack.Push(Result(operand1, token, operand2));  // Modified to use string as operator
            }
        }

        return stack.Pop();
    }

    // Changed operator type to string
    private static double Result(double operand1, string op, double operand2)
    {
        return op switch
        {
            "+" => operand1 + operand2,
            "-" => operand1 - operand2,
            "*" => operand1 * operand2,
            "/" => operand1 / operand2,
            "^" => Math.Pow(operand1, operand2),
            _ => throw new Exception("Invalid operator"),
        };
    }

    // Changed return type to List<string>
    private static List<string> ToPostfix(string infix)
    {
        var stack = new Stack<string>();          // Stack of operators (now strings)
        var postfix = new List<string>();         // Output list of tokens
        var numberBuilder = new StringBuilder();  // Accumulator for multi-digit/decimal numbers

        foreach (var ch in infix)
        {
            if (char.IsDigit(ch) || ch == '.')    // Accepts digits and decimal point
            {
                numberBuilder.Append(ch);         // Builds the number
            }
            else
            {
                if (numberBuilder.Length > 0)     // Add number to output before handling operator
                {
                    postfix.Add(numberBuilder.ToString());
                    numberBuilder.Clear();
                }

                if (ch == '(')
                {
                    stack.Push(ch.ToString());
                }
                else if (ch == ')')
                {
                    while (stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Pop(); 
                }
                else if (IsOperator(ch.ToString())) // Changed from char to string
                {
                    while (stack.Count > 0 && PriorityExpression(ch) <= PriorityStack(stack.Peek()[0]))
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Push(ch.ToString());
                }
            }
        }

        if (numberBuilder.Length > 0)             // Add any remaining number to output
        {
            postfix.Add(numberBuilder.ToString());
        }

        while (stack.Count > 0)
        {
            postfix.Add(stack.Pop());
        }

        return postfix;
    }

    private static int PriorityStack(char item)
    {
        return item switch
        {
            '^' => 3,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 0,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static int PriorityExpression(char item)
    {
        return item switch
        {
            '^' => 4,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 5,
            _ => throw new Exception("Invalid expression."),
        };
    }

    // Changed to accept string instead of char
    private static bool IsOperator(string token) => "()^*/+-".Contains(token);
    
}
