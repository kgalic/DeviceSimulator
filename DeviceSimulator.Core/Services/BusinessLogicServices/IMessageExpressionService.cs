using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public interface IMessageExpressionService
    {
          string ParseMessageExpressions(string message);
    }
}
