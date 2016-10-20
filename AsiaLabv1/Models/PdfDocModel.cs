using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp.Pdf;

namespace AsiaLabv1.Models
{
    public class PdfDocModel
    {
        public PdfDocument PdfDoc { get; set; }
        public Exception ErrorObject { get; set; }
    }
}