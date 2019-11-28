using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.State;
using Junkbot.Game.UI.Menus;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.World.Level
{
    internal class Intro : UIPage
    {
        System.Timers.Timer _timer;
        internal JunkbotGame JunkbotGame;
        private UxShell Shell;
        public ISpriteBatch Gif;
        private ISpriteBatch _box;
        private string Rank;
        private int Moves;
        private int currentFrame;
        private int length;
        public bool IntroPlayed = false;
        bool disposed = false;

        public Intro(UxShell shell, JunkbotGame junkbotGame)
        : base(shell, junkbotGame)
        {
            Shell = shell;
            JunkbotGame = junkbotGame;
            currentFrame = 0;
        }

        public void Render(IGraphicsController graphics)
        {
            if (Gif == null && IntroPlayed == false)
            {
                LoadIntro(graphics);
            }
            if (Gif != null && IntroPlayed == false)
            {

            
                _box = graphics.CreateSpriteBatch("intro-box-atlas");

                _box.Draw(
                    "intro-box",
                    new Rectangle(
                        29, 0, _box.GetSpriteUV("intro-box").Width, _box.GetSpriteUV("intro-box").Height)
                );
                _box.Finish();

                length = Gif.GetAtlasLength();
                if (currentFrame == length)
                {
                    IntroPlayed = true;
                    currentFrame = 0;
                }

                if (!IntroPlayed)
                {
                    DrawFrame();
                }

            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            currentFrame += 1;
        }

        public void LoadIntro(IGraphicsController graphics)
        {
            Gif = graphics.CreateSpriteBatch("intro", 1);
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

            Gif.Draw(
                $"intro_{currentFrame}",
                new Rectangle(62, 25,
                    369, 369)
            );
            Gif.FinishFrame(currentFrame);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;

            // Call the base class implementation.
            base.Dispose(disposing);
        }

        ~Intro()
        {
            Dispose(false);
        }
    }
}
