#region

using System.Runtime.Serialization;

#endregion

namespace URL_Parser.Models
{
    [DataContract]
    public class Image
    {
        [DataMember]
        public string Src { get; set; }

        [DataMember]
        public string Alt { get; set; }
    }
}