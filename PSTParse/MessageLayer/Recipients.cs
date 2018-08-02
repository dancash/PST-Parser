using System.Collections.Generic;

namespace PSTParse.MessageLayer
{
    public class Recipients
    {
        public List<Recipient> To { get; } = new List<Recipient>();
        public List<Recipient> CC { get; } = new List<Recipient>();
        public List<Recipient> BCC { get; } = new List<Recipient>();
    }
}
