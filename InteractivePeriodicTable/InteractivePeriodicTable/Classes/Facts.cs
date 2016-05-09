using System;
using System.Collections.Generic;

namespace InteractivePeriodicTable.Data
{
    [Serializable]
    public class Facts
    {
        public string Fact { get; set; }
    }

    [Serializable]
    public class AllFacts
    {
        public List<Facts> Facts = new List<Facts>();
    }
}
