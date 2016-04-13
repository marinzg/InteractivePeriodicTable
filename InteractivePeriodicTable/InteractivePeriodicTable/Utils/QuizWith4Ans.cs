using System;

namespace InteractivePeriodicTable.Utils
{
    [Serializable]
    public class QuizWith4Ans
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string A1 { get; set; }
        public string A2 { get; set; }
        public string A3 { get; set; }
        public string A4 { get; set; }
    }
}
