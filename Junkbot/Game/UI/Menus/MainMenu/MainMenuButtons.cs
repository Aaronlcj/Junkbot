using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI.Menus;
using Junkbot.Game.UI.Menus.Help;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.World.Level
{
    internal class MainMenuButtons : UIPage
    {

        private ISpriteBatch _buttons;
        internal JunkbotGame JunkbotGame;
        public IList<JunkbotDecalData> Button_List;
        private string _play = "play";
        private string _credits;
        private string _replayIntro;
        private string _clearScreen;
        private bool _hover = false;
        private UxShell Shell;
        private Button Play;
        private Button Credits;
        private Button ReplayIntro;
        private Button ClearScreen;

        public MainMenuButtons(UxShell shell, JunkbotGame junkbotGame)
            : base(shell, junkbotGame)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;
            _play = "play";
            _credits = "credits";
            _replayIntro = "replay_intro";
            _clearScreen = "clear_screen";
            Play = new Button(JunkbotGame, "play", new SizeF(116, 45), new PointF(139, 148), this);
            Credits = new Button(JunkbotGame, "credits", new SizeF(96, 26), new PointF(310, 159), this);
            ReplayIntro = new Button(JunkbotGame, "replay_intro", new SizeF(96, 26), new PointF(508, 337), this);
            ClearScreen = new Button(JunkbotGame, "clear_screen", new SizeF(96, 26), new PointF(508, 367), this);
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
