using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.HTTP;

public interface ILogger
{
    void Error(string message);
    void Info(string message);
}
