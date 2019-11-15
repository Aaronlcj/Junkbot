﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oddmatics.Rzxe.Windowing.Graphics
{
    public interface ISpriteBatch
    {
        void Draw(string spriteName, Rectangle rect);

        Pencil.Gaming.MathUtils.Rectanglei GetSpriteUV(string spriteName);

        void Finish();
    }
}
