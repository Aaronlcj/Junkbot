using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI;
using SharpFont;

namespace Junkbot.Game.World.Level
{
    internal class LevelSelectText
    {

        private ISpriteBatch _font;
        private ISpriteBatch _levelNumbers;
        private FontService _fontService;
        private List<string> _levels;
        private LevelSelectButtons _levelSelectButtons;


        public LevelSelectText(List<string> levels, LevelSelectButtons buttons)
        {
            _fontService = new FontService();
            _levelSelectButtons = buttons;
            _fontService.SetFont();
            _levels = levels;
        }

       public void Render(IGraphicsController graphics)
        {
           
            var text = File.ReadAllLines(Environment.CurrentDirectory + $@"\smallblacktext2.txt");
            foreach (string line in text)
            {
                Bitmap res = _fontService.RenderString(line, Color.Black);
                res.Save(line + ".png");
            }

            var renString = "EYEBOT";
            
            _font = graphics.CreateSpriteBatch($"levels-b{_levelSelectButtons.Tab}-atlas");
            _levelNumbers = graphics.CreateSpriteBatch($"level-numbers-atlas");

            int b = 94;
            int c = 0;
            foreach (string level in _levels)
            {
                _levelNumbers.Draw(
                    $"{c + 1}",
                    new Rectangle(13, b, _levelNumbers.GetSpriteUV($"{c + 1}").Width, _levelNumbers.GetSpriteUV($"{c + 1}").Height)
                );
                _font.Draw(
                    $"{c}",
                    new Rectangle(74, b, _font.GetSpriteUV($"{c}").Width, _font.GetSpriteUV($"{c}").Height)
                );
                b += 21;
                c++;
            }
            _levelNumbers.Finish();
            _font.Finish();
        }
    }
}
