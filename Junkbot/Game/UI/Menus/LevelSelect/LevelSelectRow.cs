using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.State;
using Junkbot.Game.World.Level;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.UI.Menus.Help
{
    internal class LevelSelectRow : UxComponent
    {
        internal JunkbotGame JunkbotGame { get; set; }
        public RectangleF Bounds { get; set; }
        public bool Enabled { get; set; }
        public PointF Location { get; set; }
        public string Name { get; set; }
        public bool Selectable { get; }
        public SizeF Size { get; set; }
        public bool Hover { get; private set; }

        public LevelSelectRow(JunkbotGame junkbotGame, string name, SizeF size, PointF location, int zIndex)
            : base(name, size, location, zIndex)
        {
            JunkbotGame = junkbotGame;
            Name = name;
            Size = size;
            Location = location;
            Enabled = true;
            Selectable = true;
            Bounds = new RectangleF(Location, Size);
            ZIndex = zIndex;
        }

        public override void OnClick()
        {
            JunkbotGame.CurrentGameState = new LevelState(Name);
         
        }

        public override void OnMouseDown()
        {

        }

        public override void OnMouseEnter()
        {
            Hover = true;
        }

        public override void OnMouseLeave()
        {
            Hover = false;
        }

        public override void OnMouseUp()
        {

        }
    }
}
