using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class DescriptionParts
    {
        public int descriptionPartID { get; set; }
        public int maskID { get; set; }
        public int descriptiveCodeID { get; set; }
        public string descriptionPartSymbols { get; set; }
        public string characteristicDescriptionPartSymbols { get; set; }
    }
}
