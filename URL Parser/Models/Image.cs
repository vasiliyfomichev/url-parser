#region

using System;

#endregion

namespace URL_Parser.Models
{
    [Serializable]
    public class Image
    {
        public string Src { get; set; }
        public string Alt { get; set; }
    }
}