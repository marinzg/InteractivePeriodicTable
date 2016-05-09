using System;
using System.Collections.Generic;

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

    [Serializable]
    public class QuizPictures
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string Answer { get; set; }
    }

    [Serializable]
    public class QuizQuestions
    {
        public List<QuizWith4Ans> QuizWith4Ans = new List<QuizWith4Ans>();
        public List<QuizYesNo> QuizYesNo = new List<QuizYesNo>();
        public List<QuizPictures> QuizPictures = new List<QuizPictures>();
    }
}
