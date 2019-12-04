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

    internal class LevelSelectButtons : UIPage
    {
        internal JunkbotGame JunkbotGame;
        private UxShell Shell;
        private Button Building1;
        private Button Building2;
        private Button Building3;
        private Button Building4;
        private Button HallOfFame;
        private Button Help;
        private Button Main;

        private ISpriteBatch _buttons;
        private ISpriteBatch _tabs;

        public IList<JunkbotDecalData> Button_List;
        private string _tab;
        public int Tab { get; set; }

        public LevelSelectButtons(UxShell shell, JunkbotGame junkbotGame, GameState state)
        : base(shell, junkbotGame, state)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;

            Building1 = new Button(JunkbotGame, this, "building_tab_1", new SizeF(83, 13), new PointF(21, 59), 1);
            Building2 = new Button(JunkbotGame, this, "building_tab_2", new SizeF(88, 13), new PointF(131, 59), 1);
            Building3 = new Button(JunkbotGame, this, "building_tab_3", new SizeF(89, 13), new PointF(244, 59), 1);
            Building4 = new Button(JunkbotGame, this, "building_tab_4", new SizeF(88, 13), new PointF(357, 59), 1);
            HallOfFame = new Button(JunkbotGame, this, "hiscore", new SizeF(96, 26), new PointF(511, 321), 1);
            Help = new Button(JunkbotGame, this, "help", new SizeF(96, 26), new PointF(511, 350), 1);
            Main = new Button(JunkbotGame, this, "quit", new SizeF(96, 26), new PointF(511, 379), 1);

            Shell.AddComponents(new List<UxComponent>()
                {
                    Building1,
                    Building2,
                    Building3,
                    Building4,
                    HallOfFame,
                    Help,
                    Main
                }
            );
            Tab = 1;
            _tab = $"tab_{Tab}";
        }

        public void ChangeProperty()
        {
            //Tab = tab;
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
                Building1.Name,
                new Rectangle(
                    (int) Building1.Location.X, (int) Building1.Location.Y, (int) Building1.Size.Width,
                    (int) Building1.Size.Height));
            _tabs.Draw(
                Building2.Name,
                new Rectangle(
                    (int) Building2.Location.X, (int) Building2.Location.Y, (int) Building2.Size.Width,
                    (int) Building2.Size.Height));
            _tabs.Draw(
                Building3.Name,
                new Rectangle(
                    (int) Building3.Location.X, (int) Building3.Location.Y, (int) Building3.Size.Width,
                    (int) Building3.Size.Height));
            _tabs.Draw(
                Building4.Name,
                new Rectangle(
                    (int) Building4.Location.X, (int) Building4.Location.Y, (int) Building4.Size.Width,
                    (int) Building4.Size.Height));
            _tabs.Finish();

            _buttons.Draw(
                HallOfFame.Name,
                new Rectangle(
                    (int) HallOfFame.Location.X, (int) HallOfFame.Location.Y, (int) HallOfFame.Size.Width,
                    (int) HallOfFame.Size.Height));
            _buttons.Draw(
                Help.Name,
                new Rectangle(
                    (int) Help.Location.X, (int) Help.Location.Y, (int) Help.Size.Width, (int) Help.Size.Height));
            _buttons.Draw(
                Main.Name,
                new Rectangle(
                    (int) Main.Location.X, (int) Main.Location.Y, (int) Main.Size.Width, (int) Main.Size.Height));
            _buttons.Finish();
        }
        public void Dispose()
        { }
    }
}
