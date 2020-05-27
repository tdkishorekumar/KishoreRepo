using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SearchFight.SearchRunners
{
    [XmlInclude(typeof(DefaultQueryFormatter))]
    [XmlInclude(typeof(QueryStringFormatter))]
    public abstract class QueryFormatter
    {
        public static readonly QueryFormatter Default = new DefaultQueryFormatter();

        public abstract string FormatQuery(string query);
    }

    public class DefaultQueryFormatter : QueryFormatter
    {
        public override string FormatQuery(string query)
        {
            return query;
        }
    }

    public class QueryStringFormatter : QueryFormatter
    {
        [XmlAttribute]
        public string Format { get; set; }

        public QueryStringFormatter() { }

        public QueryStringFormatter(string format)
        {
            Format = format;
        }

        public override string FormatQuery(string query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (Format == null)
                throw new InvalidOperationException("Format cannot be null.");

            try
            {
                return string.Format(Format, query);
            }
            catch (FormatException ex)
            {
                throw new ParsingException("The given format is not valid.", ex);
            }
        }
    }
}
