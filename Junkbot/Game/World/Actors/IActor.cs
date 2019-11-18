using Junkbot.Game.World.Actors.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Actors
{
    internal interface IActor
    {
        AnimationServer Animation { get; }

        IReadOnlyList<Rectangle> BoundingBoxes { get; }

        Size GridSize { get; }

        Point Location { get; set; }

        string Type { get; set; }

        event LocationChangedEventHandler LocationChanged;

/*        Rectangle GetCheckBounds();
*/
        void Update();
    }
}
