using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractivePeriodicTable.Data
{
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
}
