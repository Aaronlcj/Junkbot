﻿using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI.Menus.Help;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.World.Level
{
    internal class MainMenuButtons
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

        public MainMenuButtons(UxShell shell, JunkbotGame junkbotGame)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;
            _play = "play";
            _credits = "credits";
            _replayIntro = "replay_intro";
            _clearScreen = "clear_screen";
            Play = new Button(JunkbotGame, "play", new SizeF(116, 45), new PointF(139, 148));
            Shell.AddComponent(Play);
        }
        // make buttons list w/ loc & size for mouse checking

        public void HoverButton(string button)
        {
            _hover = _hover == false ? true : false;

            switch (button)
            {
                case "play":
                    _play = _hover ? _play += "_x" : _play.Remove(_play.Length - 2, 2);
                    break;
                case "credits":
                    _credits = _hover ? _credits += "_x" : _credits.Remove(_credits.Length - 2, 2);
                    break;
                case "replay_intro":
                    _replayIntro = _hover ? _replayIntro += "_x" : _replayIntro.Remove(_replayIntro.Length - 2, 2);
                    break;
                case "clear_screen":
                    _clearScreen = _hover ? _clearScreen += "_x" : _clearScreen.Remove(_clearScreen.Length - 2, 2);
                    break;
            }
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
                _credits,
                new Rectangle(
                    310, 159, 96, 26)
                );
            //Hint
            _buttons.Draw(
                _replayIntro,
                new Rectangle(
                    508, 337, 96, 26)
                );
            _buttons.Draw(
                _clearScreen,
                new Rectangle(
                    508, 367, 96, 26)
            );

            _buttons.Finish();
        }
    }
}