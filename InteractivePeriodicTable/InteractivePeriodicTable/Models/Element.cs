namespace InteractivePeriodicTable.Models
{
    public class Element
    {
        public int atomicNumber { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int elementCategory { get; set; }
        public int elementSubcategory { get; set; }
        public int group { get; set; }
        public string block { get; set; }
        public int period { get; set; }
        public int phase { get; set; }
        public int crystalStructure { get; set; }
    }

}
