using Oddmatics.Rzxe.Windowing.Graphics;
using Pencil.Gaming.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using Pencil.Gaming.MathUtils;

namespace Oddmatics.Rzxe.Windowing.Implementations.GlfwFx
{
    internal sealed class GLGraphicsController : IGraphicsController
    {
        public Size TargetResolution { get; private set; }

        //public FontService FontService { get; set; }

        private GLResourceCache ResourceCache { get; set; }


        public GLGraphicsController(
            GLResourceCache resourceCache,
            Size targetResolution
            )
        {
            ResourceCache = resourceCache;
            TargetResolution = targetResolution;
            //FontService = new FontService();

        }


        public void ClearViewport(Color color)
        {
            GL.ClearColor(
                (float) color.R / 255,
                (float) color.G / 255,
                (float) color.B / 255,
                1.0f
                );

            GL.Clear(ClearBufferMask.ColorBufferBit);
            //FontService.BitmapList.Clear();
            

        }

        public ISpriteBatch CreateSpriteBatch(string atlasName, int type = 0, IList<TextItem> bitmapList = null)
        {
            return new GLSpriteBatch(
                this,
                atlasName,
                type,
                bitmapList,
                ResourceCache
                );
        }

        public void DeleteAtlas(string atlas)
        {
            ResourceCache.DeleteAtalas(atlas);
        }
        public void RenderText(string state, IList<TextItem> textItems, int type)
        {

            //create text object w/ bitmap, text string, position, id
            ISpriteBatch textSpriteBatch = CreateSpriteBatch(state, type, textItems);
            int i = 0;
            do
            {
                TextItem currenTextItem = textItems[i];

                var position = currenTextItem.Position;
                var uv = currenTextItem.Map;

                /*try
                {

                     uv = textSpriteBatch.GetSpriteUV($"{currenTextItem.Text}");
                }
                catch
                {
                    Console.WriteLine("error 404");
                    uv = textSpriteBatch.GetSpriteUV($"{currenTextItem.Text}");

                }*/

                textSpriteBatch.DrawText(
                    $"{currenTextItem.Text}",
                    new System.Drawing.Rectangle(
                        position.X, position.Y, uv.Size.X, uv.Size.Y
                    ), currenTextItem.Map
                );
                textSpriteBatch.FinishText(currenTextItem.GlTextureId, new Vector2(uv.Size.X, uv.Size.Y));
                i++;
            } while (i != textItems.Count);
        }
    }
}
