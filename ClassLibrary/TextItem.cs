using System.Collections.Generic;
using System.Drawing;
using Pencil.Gaming.MathUtils;

namespace ClassLibrary
{
    public class TextItem
    {
        public string Text;
        public Bitmap Bitmap;
        public Vector2i Position;
        public int Id;
        public int GlTextureId;
        public Rectanglei Map;
        public TextItem(string text, Bitmap bitmap, Vector2i position, int id, Rectanglei map)
        {
            Text = text;
            Bitmap = bitmap;
            Position = position;
            Id = id;
            GlTextureId = 0;
            Map = map;
        }
    }
}
