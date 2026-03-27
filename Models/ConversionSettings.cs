using System;

namespace SolidConvert_Pro.Models
{
    public class ConversionSettings
    {
        public bool ExportToPdf { get; set; }
        public bool ExportToDwg { get; set; }
        public string OutputFolder { get; set; }
        public string LastOutputFolder { get; set; }
    }
}


