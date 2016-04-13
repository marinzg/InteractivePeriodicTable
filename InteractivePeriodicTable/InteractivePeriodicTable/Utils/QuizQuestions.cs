using System;
using System.Collections.Generic;

namespace InteractivePeriodicTable.Utils
{
    [Serializable]
    public class QuizQuestions
    {
        public List<QuizWith4Ans> QuizWith4Ans = new List<QuizWith4Ans>();
        public List<QuizYesNo> QuizYesNo = new List<QuizYesNo>();
        public List<QuizPictures> QuizPictures = new List<QuizPictures>();
    }
}
