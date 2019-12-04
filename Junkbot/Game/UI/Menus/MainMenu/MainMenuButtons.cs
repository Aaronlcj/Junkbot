using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI.Controls;
using Junkbot.Game.UI.Menus;
using Junkbot.Game.UI.Menus.Help;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.World.Level
{
    internal class MainMenuButtons : UIPage
    {

        private ISpriteBatch _buttons;
        internal JunkbotGame JunkbotGame;
        private UxShell Shell;
        private GameState State;

        private Button Play;
        private Button Credits;
        private Button ReplayIntro;
        private Button ClearScreen;

        public MainMenuButtons(UxShell shell, JunkbotGame junkbotGame, GameState state)
            : base(shell, junkbotGame, state)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;
            State = state;
            Play = new Button(JunkbotGame,this, "play", new SizeF(116, 45), new PointF(139, 148), 1);
            Credits = new Button(JunkbotGame, this, "credits", new SizeF(96, 26), new PointF(310, 159), 1);
            ReplayIntro = new Button(JunkbotGame, this, "replay_intro", new SizeF(96, 26), new PointF(508, 337), 1);
            ClearScreen = new Button(JunkbotGame, this, "clear_screen", new SizeF(96, 26), new PointF(508, 367), 1);
            Shell.AddComponents(new List<UxComponent>()
                {
                    Play,
                    Credits,
                    ReplayIntro,
                    ClearScreen
                }
            );
        }
        public void Render(IGraphicsController graphics)
        {
            _buttons = graphics.CreateSpriteBatch("buttons-atlas");

            _buttons.Draw(
                Play.Name,
                new Rectangle(
                    (int)Play.Location.X, (int)Play.Location.Y, (int)Play.Size.Width, (int)Play.Size.Height)
                );
            //Go To Help
            _buttons.Draw(
                Credits.Name,
                new Rectangle(
                    (int)Credits.Location.X, (int)Credits.Location.Y, (int)Credits.Size.Width, (int)Credits.Size.Height)
            );
            //Hint
            _buttons.Draw(
                ReplayIntro.Name,
                new Rectangle(
                    (int)ReplayIntro.Location.X, (int)ReplayIntro.Location.Y, (int)ReplayIntro.Size.Width, (int)ReplayIntro.Size.Height)
            );
            _buttons.Draw(
                ClearScreen.Name,
                new Rectangle(
                    (int)ClearScreen.Location.X, (int)ClearScreen.Location.Y, (int)ClearScreen.Size.Width, (int)ClearScreen.Size.Height)
            );

            _buttons.Finish();
        }
    }
}
