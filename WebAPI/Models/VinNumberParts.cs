using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class VinNumberParts
    {
        public int MaskID { get; set; }
        public string Symbols { get; set; }
        public string MaskContent { get; set; }
    }
}
