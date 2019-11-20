using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Actors
{
    internal enum JunkbotCollision
    {
        CanWalk = 8,
        StepUpBlocked = 1,
        TurnAround = 2,
        StepDownBlocked = 4,
        StepUp = 5,
        Floor = 6,
        StepDown = 7
    }
}
