using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Level
{
    internal class LevelSidebar
    {
        public Point Location;

        public ushort Par;

        public Size Size;

        private ISpriteBatch Background;

        private ISpriteBatch Buttons;

        public Int32 Moves;

        public bool RecordSet;

        public LevelSidebar(JunkbotLevelData levelData)
        {
            Location = new Point(530, 0);
            Size = new Size(120, 420);
            Moves = 0;
            RecordSet = false;
            Par = levelData.Par;
        }

        public void Render(IGraphicsController graphics)
        {
            Background = graphics.CreateSpriteBatch("sidebar-atlas");
            Buttons = graphics.CreateSpriteBatch("buttons-atlas");

            //Sidebar Background
            Background.Draw(
                "sidebar",
                new Rectangle(
                   Location, Size)
                );

            Background.Finish();
            //Restart Level
            Buttons.Draw(
            "restart_level",
            new Rectangle(
                544, 281, 96, 26)
            );
            //Select Level
            Buttons.Draw(
                "mainmenu",
                new Rectangle(
                    544, 308, 96, 26)
                );
            //Go To Help
            Buttons.Draw(
                "gotohelp",
                new Rectangle(
                    544, 335, 96, 26)
                );
            //Hint
            Buttons.Draw(
                "hint_button",
                new Rectangle(
                    544, 371, 96, 26)
                );
            Buttons.Finish();
        }
    }
}
