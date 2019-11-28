using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Level
{
    internal class LevelSelectGif
    {
        System.Timers.Timer _timer;
        private ISpriteBatch _gif;
        private string Rank;
        private int Moves;
        private int currentFrame;
        private int length;

        public LevelSelectGif()
        {
            Rank = "new_employee";
            Moves = 0;
            currentFrame = 0;
            SetTimer();

        }

        public void Render(IGraphicsController graphics)
        {
            _gif = graphics.CreateSpriteBatch(Rank, 1);


            DrawFrame();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            currentFrame += 1;
        }
        public void SetTimer()
        {
            _timer = new System.Timers.Timer(66);
            _timer.Elapsed += Timer_Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private void DrawFrame()
        {
            length = _gif.GetAtlasLength();
            if (currentFrame >= length)
            {
                currentFrame = 0;
            }
            _gif.Draw(
                $"{Rank}_{currentFrame}",
                new Rectangle(492, 23,
                    135, 118)
            );
            _gif.FinishFrame(currentFrame);
        }
    }
}
