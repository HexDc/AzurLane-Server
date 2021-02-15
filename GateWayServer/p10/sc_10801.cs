using System;
using System.Collections.Generic;

namespace p10
{
    [Serializable]
    public class sc_10801
    {
        public string gateway_ip { get; set; }

        public uint gateway_port { get; set; }

        public string url { get; set; }

        public List<string> version { get; set; }

        public string proxy_ip { get; set; }

        public uint proxy_port { get; set; }

        public uint is_ts { get; set; }
    }
}
