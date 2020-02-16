using Oddmatics.Rzxe.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pencil.Gaming.MathUtils;
using Rectangle = System.Drawing.Rectangle;

namespace Oddmatics.Rzxe.Windowing.Graphics
{
    public interface ISpriteBatch
    {
        void Draw(string spriteName, Rectangle rect);
        void DrawText(string spriteName, Rectangle rect, Rectanglei uv);
        Pencil.Gaming.MathUtils.Rectanglei GetSpriteUV(string spriteName);
        int GetAtlasLength();
        void DeleteAtlas(string atlas);
        void Finish();
        void FinishFrame(int frame);
        void FinishText(int id, Vector2 size);
    }
}
