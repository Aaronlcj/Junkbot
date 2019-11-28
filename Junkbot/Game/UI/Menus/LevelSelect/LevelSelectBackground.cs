﻿using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Level
{
    internal class LevelSelectBackground
    {
        private ISpriteBatch _levelSelectSprites;

        private string _building1;
        private string _building2;
        private string _building3;
        private string _building4;
        private string _background;
        private string _plaque;
        private string _checkbox_on;
        private string _checkbox_off;
        private string _levelSelected;
        private int currentFrame;
        public LevelSelectBackground()
        {
            _building1 = "building_icon_1";
            _building1 = "building_icon_1";
            _building2 = "building_icon_2";
            _building3 = "building_icon_3";
            _building4 = "building_icon_4";
            _background = "background";
            _plaque = "plaque_welcome";
            _checkbox_on = "checkbox_on";
            _checkbox_off = "checkbox_off";
            _levelSelected = "level_selected";
            currentFrame = 0;
        }

        public void Render(IGraphicsController graphics)
        {
            _levelSelectSprites = graphics.CreateSpriteBatch("level-select-atlas");
           
            _levelSelectSprites.Draw(
                _background,
                new Rectangle(Point.Empty,
                    graphics.TargetResolution)
            );
            _levelSelectSprites.Draw(
                _building1,
                new Rectangle(
                    27, 10, 61, 38)
                );
            _levelSelectSprites.Draw(
                _building2,
                new Rectangle(
                    143, 19, 58, 28)
                );
            _levelSelectSprites.Draw(
                _building3,
                new Rectangle(
                    254, 13, 63, 34)
            );
            _levelSelectSprites.Draw(
                _building4,
                new Rectangle(
                    373, 5, 56, 42)
            );

            _levelSelectSprites.Draw(
                _plaque,
                new Rectangle(
                    488, 155, 142, 60)
                );

            
            _levelSelectSprites.Finish();
        }
    }
}
