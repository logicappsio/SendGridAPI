using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRex.Metadata;

namespace SendGridAPI.Models
{
    public class Email
    {
        [Metadata("Message", "Plain Text Message", VisibilityType.Default)]
        public string text { get; set; }
        [Metadata("Recipients", "Comma separated list")]
        public string recipients { get; set; }
        [Metadata("From")]
        public string from { get; set; }
        [Metadata("Subject")]
        public string subject { get; set; }
        [Metadata("HTML Message", "HTML Message", VisibilityType.Advanced)]
        public string html { get; set; }
        [Metadata("Click Tracking", "Enable SendGrid Click Tracking", VisibilityType.Advanced)]
        public bool clickTracking { get; set; }
    }
}