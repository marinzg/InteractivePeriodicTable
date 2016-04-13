using System;

namespace InteractivePeriodicTable.Utils
{
    [Serializable]
    public class QuizPictures
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string Answer { get; set; }
    }
}
