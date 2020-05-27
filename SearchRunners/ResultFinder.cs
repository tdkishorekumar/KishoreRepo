using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Xml.Serialization;

namespace SearchFight.SearchRunners
{
    [XmlInclude(typeof(JSONResultFinder))]
    [XmlInclude(typeof(RegexResultFinder))]
    public abstract class ResultFinder
    {
        public abstract string Find(string responseText);
    }

    public class JSONResultFinder : ResultFinder
    {
        public string Path { get; set; }

        private static object Deserialize(string responseText)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.DeserializeObject(responseText);
            }
            catch (ArgumentException ex)
            {
                throw new ParsingException(ex.Message, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ParsingException(ex.Message, ex);
            }
        }

        public override string Find(string responseText)
        {
            if (responseText == null)
                throw new ArgumentNullException("responseText");

            if (Path == null)
                throw new ConfigurationException("Path of JSONResultFinder cannot be null.");

            var data = Deserialize(responseText);

            try
            {
                return Convert.ToString(DataBinder.Eval(data, Path));
            }
            catch (ArgumentException ex)
            {
                throw new ConfigurationException("The given Path is invalid. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ParsingException("The given Path did not match the JSON object.", ex);
            }
        }
    }

    public class RegexResultFinder : ResultFinder
    {
        public string Pattern { get; set; }

        [XmlAttribute]
        [DefaultValue(RegexOptions.None)]
        public RegexOptions Options { get; set; }

        [XmlAttribute]
        [DefaultValue(0)]
        public int GroupIndex { get; set; }

        private System.Text.RegularExpressions.Match FindMath(string responseText)
        {
            try
            {
                Regex re = new Regex(Pattern, Options);
                return re.Match(responseText);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ConfigurationException(ex.Message, ex);
            }
            catch (ArgumentException ex)
            {
                throw new ConfigurationException("The given regular expression is not valid.", ex);
            }
            catch (RegexMatchTimeoutException ex)
            {
                throw new ParsingException(ex.Message, ex);
            }
        }

        public override string Find(string responseText)
        {
            if (responseText == null)
                throw new ArgumentNullException("responseText");

            if (Pattern == null)
                throw new ConfigurationException("Pattern of RegexResultFinder cannot be null");

            try
            {
                var match = FindMath(responseText);

                if (!match.Success)
                    throw new ParsingException("Could not find a matching string.");

                if (match.Groups.Count <= GroupIndex)
                    throw new ParsingException("The given GroupIndex is out of range.");

                return match.Groups[GroupIndex].Value;
            }
            catch (RegexMatchTimeoutException ex)
            {
                throw new ParsingException("Regular expression timed out.", ex);
            }
        }
    }
}
