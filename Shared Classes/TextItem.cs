using System.Drawing;
using Pencil.Gaming.MathUtils;

namespace Shared_Classes
{
    public class TextItem
    {
        public string Text;
        public Bitmap Bitmap;
        public Vector2i Position;
        public int Id;
        public int GlTextureId;
        public TextItem(string text, Bitmap bitmap, Vector2i position, int id)
        {
            Text = text;
            Bitmap = bitmap;
            Position = position;
            Id = id;
            GlTextureId = 0;
        }
    }
}
