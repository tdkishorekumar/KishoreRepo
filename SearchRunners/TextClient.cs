using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SearchFight.Utilities;
using System.ComponentModel;
using System.Diagnostics;

namespace SearchFight.SearchRunners
{
    [XmlInclude(typeof(HttpTextClient))]
    public abstract class TextClient
    {
        public static readonly TextClient Default = new HttpTextClient();

        public abstract Task<string> GetResponseText(Uri uri, StringDictionary headers);
    }

    public class HttpTextClient : TextClient
    {
        private const long DefaultTimeout = 30000;

        [XmlAttribute]
        [DefaultValue(DefaultTimeout)]
        public long Timeout { get; set; }

        public override async Task<string> GetResponseText(Uri uri, StringDictionary headers)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip }))
            {
                if (Timeout > 0)
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout);
                else
                    client.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);

                if (headers != null)
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                try
                {
                    return await client.GetStringAsync(uri);
                }
                catch (HttpRequestException ex)
                {
                    throw new WebRequestException(ex.Message, ex);
                }
                catch (TaskCanceledException ex)
                {
                    throw new WebRequestException("The request timed out.", ex);
                }
            }
        }
    }
}
