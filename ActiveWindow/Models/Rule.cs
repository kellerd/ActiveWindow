using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveWindow.Models
{
    public class Rule
    {
        public RuleType Comparison { get; set; }

        public bool Enabled { get; set; }

        public string What { get; set; }

        public TimeSpan From { get; set; }

        public TimeSpan To { get; set; }
        public RuleCommand Do {get;set;}
        public static RuleCommand Ignore
        {
            get
            {
                return new IgnoreCommand();
            }
        }

        private class IgnoreCommand : RuleCommand
        {

        }

    }
    public class RuleCommand
    {

    }
    public class ApplyProjectCommand : RuleCommand
    {
        public string RoutingSymbol { get; set; }
        public string Project { get; set; }
        public string SubClassification { get; set; }
    }
    public enum RuleType
    {
        Equals
    }
}
