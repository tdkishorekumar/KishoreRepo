using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight
{
    public class SearchException : Exception
    {
        public readonly string Language;
        public readonly string Runner;

        public SearchException(string language, string runner, string message, Exception innerException)
            : base(message, innerException)
        {
            Language = language;
            Runner = runner;
        }

        public SearchException(string language, string runner, string message)
            : this(language, runner, message, null) { }
    }


}
