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
        private LevelSelectState State { get; set; }
        public RectangleF Bounds { get; set; }
        public bool Enabled { get; set; }
        public PointF Location { get; set; }
        public string Name { get; set; }
        public int BuildingTab { get; set; }
        public int LevelID { get; set; }
        public bool Selectable { get; }
        public SizeF Size { get; set; }
        public bool Hover { get; private set; }

        public LevelSelectRow(JunkbotGame junkbotGame, string name, int tab, int id, SizeF size, PointF location, int zIndex, LevelSelectState state)
            : base(name, size, location, zIndex)
        {
            JunkbotGame = junkbotGame;
            Name = name;
            BuildingTab = tab;
            LevelID = id;
            Size = size;
            Location = location;
            Enabled = true;
            Selectable = true;
            Bounds = new RectangleF(Location, Size);
            ZIndex = zIndex;
            State = state;
        }

        public override void OnClick()
        {
            State.Dispose();
            JunkbotGame.CurrentGameState = null;
            JunkbotGame.CurrentGameState = new LevelState(Name, BuildingTab, LevelID, JunkbotGame);
         
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
