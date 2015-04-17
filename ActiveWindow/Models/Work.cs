using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveWindow.ViewModels
{
    public class Work : ValueObject<Work>
    {
        public string What { get; set; }

        public DateTimeOffset When { get; set; }
        
    }
}
