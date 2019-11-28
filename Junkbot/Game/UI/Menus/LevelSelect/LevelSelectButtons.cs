using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Level
{
    internal class LevelSelectButtons
    {

        private ISpriteBatch _buttons;
        private ISpriteBatch _tabs;

        public IList<JunkbotDecalData> Button_List;
        private string _tab;
        public int Tab { get; set; }
        private string _building1;
        private string _building2;
        private string _building3;
        private string _building4;
        private string _hallOfFame;
        private string _help;
        private string _mainMenu;
        private bool _hover = false;

        public LevelSelectButtons()
        {
            _tab = "tab_1";
            _building1 = "building_tab_1";
            _building2 = "building_tab_2";
            _building3 = "building_tab_3";
            _building4 = "building_tab_4";

            Tab = 1;
            _hallOfFame = "hiscore";
            _help = "help";
            _mainMenu = "quit";
        }
        // make buttons list w/ loc & size for mouse checking
        public void HoverButton(string button)
        {
            _hover = _hover == false ? true : false;

            switch (button)
            {
                case "building_1":
                    _building1 = _hover ? _building1 += "_x" : _building1.Remove(_building1.Length - 2, 2);
                    break;
                case "building_2":
                    _building2 = _hover ? _building2 += "_x" : _building2.Remove(_building2.Length - 2, 2);
                    break;
                case "building_3":
                    _building3 = _hover ? _building3 += "_x" : _building3.Remove(_building3.Length - 2, 2);
                    break;
                case "building_4":
                    _building4 = _hover ? _building4 += "_x" : _building4.Remove(_building4.Length - 2, 2);
                    break;
                case "hiscore":
                    _hallOfFame = _hover ? _hallOfFame += "_x" : _hallOfFame.Remove(_hallOfFame.Length - 2, 2);
                    break;
                case "help":
                    _help = _hover ? _help += "_x" : _help.Remove(_help.Length - 2, 2);
                    break;
                case "quit":
                    _mainMenu = _hover ? _mainMenu += "_x" : _mainMenu.Remove(_mainMenu.Length - 2, 2);
                    break;
            }
        }

        public void ChangeTab(int tab)
        {
            Tab = tab;
            _tab = $"tab_{Tab}";
        }

        public void Render(IGraphicsController graphics)
        {
            _buttons = graphics.CreateSpriteBatch("buttons-atlas");
            _tabs = graphics.CreateSpriteBatch("level-select-atlas");

            _tabs.Draw(
                _tab,
                new Rectangle(
                    0, 52, 468, 22)
            );
            _tabs.Draw(
                _building1,
                new Rectangle(
                    21, 59, 83, 13)
                );
            _tabs.Draw(
                _building2,
                new Rectangle(
                    131, 59, 88, 13)
                );
            _tabs.Draw(
                _building3,
                new Rectangle(
                    244, 59, 89, 13)
            );
            _tabs.Draw(
                _building4,
                new Rectangle(
                    357, 59, 88, 13)
            );
            _tabs.Finish();

            _buttons.Draw(
                _hallOfFame,
                new Rectangle(
                    511, 321, 96, 26)
                );
            _buttons.Draw(
                _help,
                new Rectangle(
                    511, 350, 96, 26)
            );
            _buttons.Draw(
                _mainMenu,
                new Rectangle(
                    511, 379, 96, 26)
            );
            _buttons.Finish();
        }
    }
}
