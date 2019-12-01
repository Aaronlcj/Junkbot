using Pencil.Gaming.MathUtils;
using Junkbot.Game.World.Actors.Animation;
using Junkbot.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Junkbot.Game.World.Actors
{
    internal interface IBotActor : IThinker
    {
        FacingDirection FacingDirection { get; set; }

        Scene Scene { get; set; }
        void SetWalkingDirection(FacingDirection direction);
    }
}
