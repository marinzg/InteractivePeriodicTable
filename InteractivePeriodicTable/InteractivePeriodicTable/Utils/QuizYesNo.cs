using System;

namespace InteractivePeriodicTable.Utils
{
    [Serializable]
    public class QuizYesNo
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string A1 { get; set; }
        public string A2 { get; set; }
    }
}
