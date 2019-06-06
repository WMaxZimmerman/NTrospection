using NTrospection.CLI.Attributes;
using System;

namespace NTrospection.Fake.CLI.Controllers
{
    [CliController("math", "Performs math operations")]
    public class MathController
    {
        [CliCommand("add", "Adds two numbers")]
        public void Add(int firstNum, int secondNum)
        {
            Console.WriteLine(firstNum + secondNum);
        }
    }
}
