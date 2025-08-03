using MZ.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MZ.Xray.Engine
{
    public class PDFProcesser
    {
        public DateTime? StartSelectedDate { get; set; }
        public DateTime? EndSelectedDate { get; set; }
        public ICollection<ReportImageFileModel> ImageFiles { get; set; }
        public string Username { get; set; }
        private string TempPath { get; set; } = Path.GetTempPath();

    }
}
