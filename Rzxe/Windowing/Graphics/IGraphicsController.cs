using ClassLibrary;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oddmatics.Rzxe.Windowing.Graphics
{
    public interface IGraphicsController
    {
        Size TargetResolution { get; }

        void ClearViewport(Color color);

        ISpriteBatch CreateSpriteBatch(string atlasName, int type = 0, IList<TextItem> bitmapList = null);
        void DeleteAtlas(string atlas);
        /*
        FontService FontService { get; set;  }
        */
        void RenderText(string state, IList<TextItem> textItems, int type);
    }
}
