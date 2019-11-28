﻿using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.UI;
using Junkbot.Game.UI.Menus;
using Junkbot.Game.UI.Menus.Help;
using Oddmatics.Rzxe.Game.Interface;
using SharpFont;

namespace Junkbot.Game.World.Level
{
    internal class LevelSelectText : UIPage
    {
        internal JunkbotGame JunkbotGame;
        private UxShell Shell;
        private ISpriteBatch _font;
        private ISpriteBatch _levelNumbers;
        private FontService _fontService;
        private List<string> _levels;
        private LevelSelectButtons _levelSelectButtons;
        private List<UxComponent> Rows;


        public LevelSelectText(JunkbotGame junkbotGame, UxShell shell, List<string> levels, LevelSelectButtons buttons)
        : base (shell, junkbotGame)
        {
            JunkbotGame = junkbotGame;
            Shell = shell;
            _fontService = new FontService();
            _levelSelectButtons = buttons;
            _fontService.SetFont();
            _levels = levels;
            Rows = new List<UxComponent>();
            int i = 88;
            foreach (string level in _levels)
            {
                Rows.Add(new LevelSelectRow(JunkbotGame, level, new SizeF(448, 20), new PointF(10, i), 1));
                i += 21;
            }
            Shell.AddComponents(Rows);
            
        }

       public void Render(IGraphicsController graphics)
        {
            
            _font = graphics.CreateSpriteBatch($"levels-b{_levelSelectButtons.Tab}-atlas");
            _levelNumbers = graphics.CreateSpriteBatch($"level-numbers-atlas");
            var levelAtlas = graphics.CreateSpriteBatch("level-select-atlas");
            int yPos = 95;
            int c = 0;
            foreach (LevelSelectRow row in Rows)
            {
                if (row.Hover == true)
                {
                    levelAtlas.Draw(
                        $"level_selected",
                        new Rectangle((int)row.Location.X, (int)row.Location.Y, (int)row.Size.Width, (int)row.Size.Height)
                    );
                }

                levelAtlas.Draw(
                    "checkbox_off",
                    new Rectangle(
                        46, yPos, 8, 8)
                );
                levelAtlas.Draw(
                    "checkbox_on",
                    new Rectangle(
                        59, yPos, 8, 8)
                );
                yPos += 21;

                _levelNumbers.Draw(
                $"{c + 1}",
                new Rectangle(13, (int)row.Location.Y + 6, _levelNumbers.GetSpriteUV($"{c + 1}").Width, _levelNumbers.GetSpriteUV($"{c + 1}").Height)
                );
                _font.Draw(
                    $"{c}",
                    new Rectangle(74, (int)row.Location.Y + 6, _font.GetSpriteUV($"{c}").Width, _font.GetSpriteUV($"{c}").Height)
                );
                c++;
            }
            levelAtlas.Finish();
            _levelNumbers.Finish();
            _font.Finish();
        }
    }
}
