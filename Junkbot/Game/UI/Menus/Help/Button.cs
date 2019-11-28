using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.State;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.UI.Menus.Help
{
    internal class Button : UxComponent
    {
        internal JunkbotGame JunkbotGame { get; set; }
        public RectangleF Bounds { get; set; }
        public bool Enabled { get; set; }
        public PointF Location { get; set; }
        public string Name { get; set; }
        public bool Selectable { get; }
        public SizeF Size { get; set; }
        public int ZIndex { get; set; }

        public Button(JunkbotGame junkbotGame, string name, SizeF size, PointF location)
            : base(name, size, location)
        {
            JunkbotGame = junkbotGame;
            Name = name;
            Size = size;
            Location = location;
            Enabled = true;
            Selectable = true;
            Bounds = new RectangleF(Location, Size);
            ZIndex = 1;
        }

        public override void OnClick()
        {
            string level = "loading_level";

            JunkbotGame.CurrentGameState = new LevelSelectState(level);
        }

        public override void OnMouseDown()
        {

        }

        public override void OnMouseEnter()
        {

        }

        public override void OnMouseLeave()
        {

        }

        public override void OnMouseUp()
        {

        }
    }
}
