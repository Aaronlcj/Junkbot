using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.State;
using Junkbot.Game.UI.Controls;
using Junkbot.Game.UI.Menus;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.World.Level
{
    internal class LevelSidebar : UIPage
    {
        public Point Location;

        public ushort Par;

        public Size Size;

        private ISpriteBatch Background;

        private ISpriteBatch Buttons;

        public Int32 Moves;

        public bool RecordSet;
        private Button RestartLevel;
        private Button SelectLevel;
        private Button GoToHelp;
        private Button Hint;

        public LevelSidebar(UxShell shell, JunkbotGame junkbotGame, GameState state, LevelStats levelStats)
            : base(shell, junkbotGame, state)
        {
            Location = new Point(530, 0);
            Size = new Size(120, 420);
            Moves = 0;
            RecordSet = false;
            Par = levelStats.Par;
            RestartLevel = new Button(JunkbotGame, this, "restart_level", new SizeF(96, 26), new PointF(544, 281), 1);
            SelectLevel = new Button(JunkbotGame, this, "select_level", new SizeF(96, 26), new PointF(544, 308), 1);
            GoToHelp = new Button(JunkbotGame, this, "go_to_help", new SizeF(96, 26), new PointF(544, 335), 1);
            Hint = new Button(JunkbotGame, this, "hint", new SizeF(96, 26), new PointF(544, 371), 1);
            Shell.AddComponents(new List<UxComponent>()
            {
                RestartLevel,
                SelectLevel,
                GoToHelp,
                Hint
            });

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

            Buttons.Draw(
                RestartLevel.Name,
                new Rectangle(
                    (int)RestartLevel.Location.X, (int)RestartLevel.Location.Y, (int)RestartLevel.Size.Width, (int)RestartLevel.Size.Height)
            ); 
            Buttons.Draw(
                SelectLevel.Name,
                new Rectangle(
                    (int)SelectLevel.Location.X, (int)SelectLevel.Location.Y, (int)SelectLevel.Size.Width, (int)SelectLevel.Size.Height)
            ); 
            Buttons.Draw(
                GoToHelp.Name,
                new Rectangle(
                    (int)GoToHelp.Location.X, (int)GoToHelp.Location.Y, (int)GoToHelp.Size.Width, (int)GoToHelp.Size.Height)
            ); 
            Buttons.Draw(
                Hint.Name,
                new Rectangle(
                    (int)Hint.Location.X, (int)Hint.Location.Y, (int)Hint.Size.Width, (int)Hint.Size.Height)
            );
            Buttons.Finish();
        }
    }
}
