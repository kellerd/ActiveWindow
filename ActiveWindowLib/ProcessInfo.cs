using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveWindowLib
{
   public class ProcessInfo
    {
        public string ProcessId { get; set; }

        public string FileName { get; set; }

        public string FileDescription { get; set; }

        public string ProductName { get; set; }

        public string ProcessName { get; set; }

        public string WindowTitle { get; set; }

        public string ActiveWindowId { get; set; }
    }
}
