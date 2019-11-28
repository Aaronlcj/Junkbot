using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Junkbot.Game.UI.Menus.Help
{
    class HelpTextItem
    {
        public string Name { get; set; }
        public Dictionary<int, string> Main { get; set; }
        public Dictionary<int, string> Sub { get; set; }
        public Point Position { get; set; }
        public Point TemplatePosition { get; set; }

    }
}
