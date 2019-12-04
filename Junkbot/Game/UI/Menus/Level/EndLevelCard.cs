using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.State;
using Junkbot.Game.UI.Controls;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Game.Interface;
using Oddmatics.Rzxe.Windowing.Graphics;
using SharpFont;

namespace Junkbot.Game.UI.Menus
{
    class EndLevelCard : UIPage
    {
        private ISpriteBatch _sprites;
        private ISpriteBatch _buttons;
        internal JunkbotGame JunkbotGame;
        private UxShell Shell;
        private GameState State;

        private Button SelectLevel;
        private Button GetHint;
        private Button TryAgain;
        private Button NextLevel;

        private bool FailStatus;
        public EndLevelCard(UxShell shell, JunkbotGame junkbotGame, GameState state, bool failStatus)
            : base(shell, junkbotGame, state)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;
            State = state;
            FailStatus = failStatus;
            List<UxComponent> buttons;
            if (FailStatus)
            {
                SelectLevel = new Button(JunkbotGame, this, "fail_selectLevel", new SizeF(86, 42), new PointF(129, 224), 1);
                GetHint = new Button(JunkbotGame, this, "fail_getHint", new SizeF(86, 42), new PointF(225, 224), 1);
                TryAgain = new Button(JunkbotGame, this, "fail_tryAgain", new SizeF(86, 42), new PointF(322, 224), 1);

                buttons = new List<UxComponent>()
                {
                    SelectLevel,
                    GetHint,
                    TryAgain
                };
            }
            else
            {
                SelectLevel = new Button(JunkbotGame, this, "select_level", new SizeF(96, 26), new PointF(347, 276), 1);
                NextLevel = new Button(JunkbotGame, this, "next_level", new SizeF(96, 26), new PointF(347, 304), 1);
                buttons = new List<UxComponent>()
                {
                    SelectLevel,
                    NextLevel
                };
            }

            Shell.AddComponents(buttons);
        }

        public void Render(IGraphicsController graphics)
        {
            _sprites = graphics.CreateSpriteBatch("level-card-atlas");
            _buttons = graphics.CreateSpriteBatch("buttons-atlas");


            if (!FailStatus)
            {
                _sprites.Draw(
                    "dbox_lg",
                    new Rectangle(
                        61, 81,
                        410, 274)
                );
                _sprites.Draw(
                    "level_end_main",
                    new Rectangle(
                        65, 82,
                        383, 207)
                );
                _buttons.Draw(
                    SelectLevel.Name,
                    new Rectangle(
                        (int)SelectLevel.Location.X, (int)SelectLevel.Location.Y, (int)SelectLevel.Size.Width, (int)SelectLevel.Size.Height)
                );
                _buttons.Draw(
                    NextLevel.Name,
                    new Rectangle(
                        (int)NextLevel.Location.X, (int)NextLevel.Location.Y, (int)NextLevel.Size.Width, (int)NextLevel.Size.Height)
                );
            }
            else
            {
                _sprites.Draw(
                    "dbox_sm",
                    new Rectangle(
                        101, 131,
                        336, 164)
                );
                _sprites.Draw(
                    "level_lose",
                    new Rectangle(
                        101, 132,
                        179, 81)
                );
                _buttons.Draw(
                    SelectLevel.Name,
                    new Rectangle(
                        (int)SelectLevel.Location.X, (int)SelectLevel.Location.Y, (int)SelectLevel.Size.Width, (int)SelectLevel.Size.Height)
                );
                _buttons.Draw(
                    GetHint.Name,
                    new Rectangle(
                        (int)GetHint.Location.X, (int)GetHint.Location.Y, (int)GetHint.Size.Width, (int)GetHint.Size.Height)
                );
                _buttons.Draw(
                    NextLevel.Name,
                    new Rectangle(
                        (int)TryAgain.Location.X, (int)TryAgain.Location.Y, (int)TryAgain.Size.Width, (int)TryAgain.Size.Height)
                );
            }
            _sprites.Finish();
            _buttons.Finish();
        }
    }
}
