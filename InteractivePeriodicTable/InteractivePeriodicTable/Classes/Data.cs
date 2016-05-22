using System;
using System.Collections.Generic;

namespace InteractivePeriodicTable.Data
{
    #region POMOĆNE KLASE KVIZ
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
    public class QuizWithPictures
    {
        public int ID { get; set; }
        public string Answer { get; set; }
    }

    [Serializable]
    public class QuizQuestions
    {
        public List<QuizWith4Ans> QuizWith4Ans = new List<QuizWith4Ans>();
        public List<QuizYesNo> QuizYesNo = new List<QuizYesNo>();
        public List<QuizWithPictures> QuizWithPictures = new List<Data.QuizWithPictures>();
    }
    #endregion

    #region POMOĆNE KLASE ZANIMLJIVOSTI
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
    #endregion

    #region POMOĆNE KLASE DRAG&DROP
    public class Phase
    {
        public int id { get; set; }
        public string name { get; set; }
    }
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
    public class ElementCategory
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class ElementSubcategory
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class CrystalStructure
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    #endregion

    #region POMOĆ ZA IGRE
    public enum Game
    {
        Quiz,
        DragDrop
    }
    public class ComboBoxPairs
    {
        public Game _Key { get; set; }
        public string _Value { get; set; }

        public ComboBoxPairs(Game _key, string _value)
        {
            _Key = _key;
            _Value = _value;
        }
    }
    #endregion
}
