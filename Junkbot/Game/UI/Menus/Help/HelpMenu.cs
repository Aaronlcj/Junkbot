using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI;
using Junkbot.Game.UI.Menus.Help;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Junkbot.Game.World.Level
{
    internal class HelpMenu
    {

        private ISpriteBatch _sprites;
        private ISpriteBatch _buttons;
        private ISpriteBatch _box;
        private ISpriteBatch _helpText;
        private FontService _fontService;
        private IList<HelpTextItem> _textList;
        private int _page;
        private string _next;
        private string _previous;
        private string _ok;
        private bool _hover;

        public HelpMenu()
        {
            _page = 2;
            _next = "next_button";
            _previous = "prev_button";
            _ok = "ok_button";
            _hover = false;
            _fontService = new FontService();
            LoadText();
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

        public void HoverButton(string button)
        {
            _hover = _hover == false ? true : false;

            switch (button)
            {
                case "next_button":
                    _next = _hover ? _next += "_x" : _next.Remove(_next.Length - 2, 2);
                    break;
                case "prev_button":
                    _previous = _hover ? _previous += "_x" : _previous.Remove(_previous.Length - 2, 2);
                    break;
                case "ok_button":
                    _ok = _hover ? _ok += "_x" : _ok.Remove(_ok.Length - 2, 2);
                    break;
            }
        }

        public void ChangePage(int page)
        {
            _page = page;
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
                    _next,
                    new Rectangle(
                        391, 355, _buttons.GetSpriteUV(_next).Width, _buttons.GetSpriteUV(_next).Height)
                );
            }

            if (_page == 2)
            {
                _buttons.Draw(
                    _previous,
                    new Rectangle(
                        300, 350, _buttons.GetSpriteUV(_previous).Width, _buttons.GetSpriteUV(_previous).Height)
                );
                _buttons.Draw(
                    _ok,
                    new Rectangle(
                        351, 342, _buttons.GetSpriteUV(_ok).Width, _buttons.GetSpriteUV(_ok).Height)
                );
            }

            _helpText.Finish();
            _sprites.Finish();

            _buttons.Finish();
        }
    }
}
