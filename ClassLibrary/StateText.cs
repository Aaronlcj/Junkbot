using System.Collections.Generic;
using System.Drawing;
using Pencil.Gaming.MathUtils;

namespace ClassLibrary
{
    public class StateText
    {
        public string State;
        public IList<TextItem> TextList;

        public StateText(string state, IList<TextItem> textList)
        {
            State = state;
            TextList = textList;
        }
    }
    
}
