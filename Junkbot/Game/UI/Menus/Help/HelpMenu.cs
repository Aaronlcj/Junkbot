using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI;
using Junkbot.Game.UI.Controls;
using Junkbot.Game.UI.Menus;
using Junkbot.Game.UI.Menus.Help;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.World.Level
{
    internal class HelpMenu : UIPage
    {
        internal JunkbotGame JunkbotGame;
        private UxShell Shell;
        private Button Next;
        private Button Previous;
        private Button Ok;
        private ISpriteBatch _sprites;
        private ISpriteBatch _buttons;
        private ISpriteBatch _box;
        private ISpriteBatch _helpText;
        private FontService _fontService;
        private IList<HelpTextItem> _textList;
        private int _page;
        bool disposed = false;
        //write zindex sorting for uipage to block input

        // Protected implementation of Dispose pattern.
        
        public HelpMenu(UxShell shell, JunkbotGame junkbotGame)
            : base(shell, junkbotGame)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;
            _page = 1;
            _fontService = new FontService();
            LoadText();
            Next = new Button(JunkbotGame,this, "next_button", new SizeF(36, 26), new PointF(391, 355),2);
            //Previous = new Button(JunkbotGame, "prev_button", new SizeF(36, 26), new PointF(300, 350), this);
            //Ok = new Button(JunkbotGame, "ok_button", new SizeF(76, 42), new PointF(351, 342), this);
            Shell.AddComponent(Next);
            /*Shell.AddComponents(new List<UxComponent>()
                {
                    Next,
                    Previous,
                    Ok
                }
            );*/
        }

        public void LoadText()
        {
            JArray helpTextJson = JArray.Parse(File.ReadAllText(Environment.CurrentDirectory + $@"\Content\Text\Help\Page_{_page}.json"));
            _textList = helpTextJson.Select(p => new HelpTextItem
            {
                Name = ((string)p["name"]),
                Main = (Dictionary<int, string>)p["main"].ToObject<Dictionary<int, string>>(),
                Sub = (Dictionary<int, string>)p["sub"].ToObject<Dictionary<int, string>>(),
                Position = (Point)p["position"].ToObject<Point>(),
                TemplatePosition = (JToken)p["template_position"] != null ? (Point)p["template_position"].ToObject<Point>() : new Point()
            }).ToList();
        }

        public override void ChangeProperty()
        {
            if (_page == 1)
            {
                _page = 2;
                Next = null;
                Previous = new Button(JunkbotGame,this, "prev_button", new SizeF(36, 26), new PointF(300, 350), 2);
                Ok = new Button(JunkbotGame,this, "ok_button", new SizeF(76, 42), new PointF(351, 342), 2);
                Shell.AddComponents(new List<UxComponent>()
                    {
                        Previous,
                        Ok
                    }
                );
            }
            else
            {
                _page = 1;
                Previous = null;
                Ok = null;
                Next = new Button(JunkbotGame,this, "next_button", new SizeF(36, 26), new PointF(391, 355), 2);
                Shell.AddComponent(Next);
            }
            LoadText();
        }

        public void Render(IGraphicsController graphics)
        {
            _box = graphics.CreateSpriteBatch("help-box-atlas");
            _sprites = graphics.CreateSpriteBatch("help-atlas");
            _helpText = graphics.CreateSpriteBatch("help-text-atlas");
            _buttons = graphics.CreateSpriteBatch("buttons-atlas");

            //draw dialog box
            _box.Draw(
                "help-box",
                new Rectangle(
                    6, 4, _box.GetSpriteUV("help-box").Width, _box.GetSpriteUV("help-box").Height)
            );
            _box.Finish();

            //draw menu items
            foreach (HelpTextItem item in _textList)
            {
                //draw page 2 template
                if (_page == 2)
                {
                    _sprites.Draw(
                        "template",
                        new Rectangle(
                            item.TemplatePosition.X, item.TemplatePosition.Y, _sprites.GetSpriteUV("template").Width,
                            _sprites.GetSpriteUV("template").Height)
                    );
                }

                //sprites
                _sprites.Draw(
                    item.Name,
                    new Rectangle(
                        item.Position.X, item.Position.Y, _sprites.GetSpriteUV(item.Name).Width, _sprites.GetSpriteUV(item.Name).Height)
                );

                //initial text positioning
                int xPos = _page == 1 ? item.Position.X + 88 : item.TemplatePosition.X + 57;
                int yPos = _page == 1 ? item.Position.Y : item.TemplatePosition.Y + 8;
                int mainLinePosition = yPos;

                //main text
                foreach (KeyValuePair<int, string> mainText in item.Main)
                {
                    _helpText.Draw(
                        mainText.Value.ToLower(),
                        new Rectangle(
                            xPos, mainLinePosition, _helpText.GetSpriteUV(mainText.Value.ToLower()).Width, _helpText.GetSpriteUV(mainText.Value.ToLower()).Height)
                    );
                    mainLinePosition += _page == 1 ? 14 : 12;
                }

                int subLinePosition = _page == 1 ? mainLinePosition + 1 : mainLinePosition;

                //sub text
                foreach (KeyValuePair<int, string> subText in item.Sub)
                {
                    _helpText.Draw(
                        subText.Value.ToLower(),
                        new Rectangle(
                            xPos, subLinePosition, _helpText.GetSpriteUV(subText.Value.ToLower()).Width, _helpText.GetSpriteUV(subText.Value.ToLower()).Height)
                    );
                    subLinePosition += _page == 1 ? 9 : 6;
                }
            }

            //buttons
            if (_page == 1)
            {
                _buttons.Draw(
                    Next.Name,
                    new Rectangle(
                        (int)Next.Location.X, (int)Next.Location.Y, (int)Next.Size.Width, (int)Next.Size.Height)
                );
            }

            if (_page == 2)
            {
                _buttons.Draw(
                    Previous.Name,
                    new Rectangle(
                        (int)Previous.Location.X, (int)Previous.Location.Y, (int)Previous.Size.Width, (int)Previous.Size.Height)
                );
                _buttons.Draw(
                    Ok.Name,
                    new Rectangle(
                        (int)Ok.Location.X, (int)Ok.Location.Y, (int)Ok.Size.Width, (int)Ok.Size.Height)
                );
            }

            _helpText.Finish();
            _sprites.Finish();

            _buttons.Finish();
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

        ~HelpMenu()
        {
            Dispose(false);
        }
    }
}
