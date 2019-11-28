using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Level
{
    internal class Intro
    {
        System.Timers.Timer _timer;
        private ISpriteBatch _gif;
        private ISpriteBatch _box;
        private string Rank;
        private int Moves;
        private int currentFrame;
        private int length;

        public Intro()
        {
            currentFrame = 0;
        }

        public void Render(IGraphicsController graphics)
        {
            _box = graphics.CreateSpriteBatch("intro-box-atlas");

            _box.Draw(
                "intro-box",
                new Rectangle(
                    29, 0, _box.GetSpriteUV("intro-box").Width, _box.GetSpriteUV("intro-box").Height)
            );
            _box.Finish();
            _gif = graphics.CreateSpriteBatch("intro", 1);
            length = _gif.GetAtlasLength();
            if (currentFrame == length)
            {
                currentFrame = 0;
            }

            DrawFrame();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            currentFrame += 1;
        }
        public void SetTimer()
        {
            _timer = new System.Timers.Timer(83);
            _timer.Elapsed += Timer_Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private void DrawFrame()
        {
            if (_timer == null)
            {
                SetTimer();
            }

            _gif.Draw(
                $"intro_{currentFrame}",
                new Rectangle(62, 25,
                    369, 369)
            );
            _gif.FinishFrame(currentFrame);
        }
    }
}
