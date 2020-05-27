using SearchFight.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace SearchFight.SearchRunners
{
    public class WebClientSearchRunner : SerializableSearchRunner
    {
        [XmlAttribute]
        public string Address { get; set; }

        [XmlAttribute]
        public string QueryName { get; set; }

        public StringDictionary Headers { get; set; }
        public StringDictionary Parameters { get; set; }
        public ResultFinder Finder { get; set; }
        public ResultParser Parser { get; set; }
        public QueryFormatter QueryFormatter { get; set; }
        public TextClient Client { get; set; }

        public WebClientSearchRunner() { }

        public override async Task<long> Run(string query)
        {
            if (Finder == null)
                throw new ConfigurationException("Finder cannot be null.");

            if (Address == null)
                throw new ConfigurationException("Address cannot be null.");

            if (string.IsNullOrWhiteSpace(QueryName))
                throw new ConfigurationException("QueryName cannot be empty.");

            var uriBuilder = BuildUri(query);
            var responseText = await (Client ?? TextClient.Default).GetResponseText(uriBuilder.Uri, Headers);
            var resultText = Finder.Find(responseText);
            return (Parser ?? ResultParser.Default).Parse(resultText);
        }

        private UriBuilder BuildUri(string query)
        {
            var parameters = HttpUtility.ParseQueryString(String.Empty);

            if (Parameters != null)
            {
                foreach (var param in Parameters)
                    parameters[param.Key] = param.Value;
            }

            parameters[QueryName] = (QueryFormatter ?? QueryFormatter.Default).FormatQuery(query);

            try
            {
                var uriBuilder = new UriBuilder(Address);
                uriBuilder.Query = parameters.ToString();
                return uriBuilder;
            }
            catch (UriFormatException ex)
            {
                throw new ConfigurationException("The given Address is not a valid URL.", ex);
            }
        }
    }
}
