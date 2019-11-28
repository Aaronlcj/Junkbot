using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.State;
using Junkbot.Game.World.Level;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.UI.Menus.Help
{
    internal class Button : UxComponent
    {
        internal JunkbotGame JunkbotGame { get; set; }
        private UIPage UIPage { get; set; }
        public RectangleF Bounds { get; set; }
        public bool Enabled { get; set; }
        public PointF Location { get; set; }
        public string Name { get; set; }
        public bool Selectable { get; }
        public SizeF Size { get; set; }
        public int ZIndex { get; set; }

        public Button(JunkbotGame junkbotGame, string name, SizeF size, PointF location, UIPage uiPage)
            : base(name, size, location)
        {
            JunkbotGame = junkbotGame;
            Name = name;
            Size = size;
            Location = location;
            Enabled = true;
            Selectable = true;
            Bounds = new RectangleF(Location, Size);
            ZIndex = 1;
            UIPage = uiPage;
        }

        public override void OnClick()
        {
            if (Name == "play" || Name == "play_x")
            {
                string level = "loading_level";
                JunkbotGame.CurrentGameState = new LevelSelectState(JunkbotGame, level);
            }
            if (Name == "ok_button" || Name == "ok_button_x")
            {
                UIPage.Dispose();
                (JunkbotGame.CurrentGameState as LevelSelectState).HelpMenu = null;

            }
            if (Name == "help" || Name == "help_x")
            {
                (JunkbotGame.CurrentGameState as LevelSelectState).HelpMenu = new HelpMenu(UIPage.Shell, UIPage.JunkbotGame);
            }
            if (Name == "replay_intro" || Name == "replay_intro_x")
            {
                //JunkbotGame.CurrentGameState = new Intro();
            }
            if (Name == "clear_screen" || Name == "clear_screen_x")
            {
                JunkbotGame.CurrentGameState = new MainMenuState("loading_level", JunkbotGame);
            }
            if (Name == "next_button" || Name == "next_button_x")
            {
                UIPage.ChangeProperty();
            }
            if (Name == "prev_button" || Name == "prev_button_x")
            {
                UIPage.ChangeProperty();
            }
            if (Name == "quit_button" || Name == "quit_button_x")
            {
                UIPage.Dispose();
                JunkbotGame.CurrentGameState = new MainMenuState("loading_level", JunkbotGame);

            }
        }

        public override void OnMouseDown()
        {

        }

        public override void OnMouseEnter()
        {
            if (Name == "building_tab_1" || Name == "building_tab_2" || Name == "building_tab_3" || Name == "building_tab_4")
            { }
            else
            {
                Name += "_x";
            }
        }

        public override void OnMouseLeave()
        {
            if (Name == "building_tab_1" || Name == "building_tab_2" || Name == "building_tab_3" || Name == "building_tab_4")
            { }
            else
            {
                Name = Name.Substring(0, Name.Length - 2);
            }
        }

        public override void OnMouseUp()
        {

        }
    }
}
