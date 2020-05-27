using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SearchFight.SearchRunners
{
    public class Configuration
    {
        [XmlArrayItem("SearchRunner")]
        public List<SerializableSearchRunner> SearchRunners { get; set; }
    }
}
