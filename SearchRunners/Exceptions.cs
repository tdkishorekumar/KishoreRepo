using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight.SearchRunners
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message) { }

        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ParsingException : Exception
    {
        public ParsingException(string message)
            : base(message) { }

        public ParsingException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class WebRequestException : Exception
    {
        public WebRequestException(string message)
            : base(message) { }

        public WebRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
