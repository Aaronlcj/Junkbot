using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Actors
{
    internal enum JunkbotCollision
    {
        EatTrash = 9,
        CanWalk = 8,
        StepUpBlocked = 1,
        TurnAround = 2,
        StepDownBlocked = 4,
        StepUp = 5,
        Floor = 6,
        StepDown = 7
    }
}
