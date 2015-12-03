#region

using System;

#endregion

namespace URL_Parser.Models
{
    [Serializable]
    public class WordReportItem
    {
        public string Word { get; set; }
        public int Count { get; set; }
    }
}