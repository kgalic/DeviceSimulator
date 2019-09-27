using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public interface IMessageExpressionService
    {
          string ParseMessageExpressions(string message);
    }
}
