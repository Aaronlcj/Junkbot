using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Level
{
    internal class MainMenuBackground
    {

        private ISpriteBatch _sprites;

        public MainMenuBackground()
        {
        }

        public void Render(IGraphicsController graphics)
        {
            _sprites = graphics.CreateSpriteBatch("main-menu-atlas");

            _sprites.Draw(
                "wrapper",
                new Rectangle(
                    Point.Empty,
                    graphics.TargetResolution)
                );

            _sprites.Draw(
                "arrow_right",
                new Rectangle(
                    34, 152, 45, 37)
                );
            _sprites.Draw(
                "arrow_right",
                new Rectangle(
                    85, 152, 45, 37)
            );
            _sprites.Draw(
                "arrow_left",
                new Rectangle(
                    260, 152, 45, 37)
            );
            _sprites.Draw(
                "arrow_left",
                new Rectangle(
                    407, 152, 45, 37)
            );
            _sprites.Finish();
        }

        public void RenderFrame(IGraphicsController graphics)
        {
            _sprites = graphics.CreateSpriteBatch("main-menu-atlas");

            _sprites.Draw(
                "background_frame",
                new Rectangle(
                    310, 159, 96, 26)
            );
            _sprites.Finish();
        }
    }
}
