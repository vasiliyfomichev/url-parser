#region

using System.Runtime.Serialization;

#endregion

namespace URL_Parser.Models
{
    [DataContract]
    public class WordReportItem
    {
        [DataMember]
        public string Word { get; set; }

        [DataMember]
        public int Count { get; set; }
    }
}