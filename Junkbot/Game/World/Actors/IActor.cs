﻿using Junkbot.Game.World.Actors.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Actors
{
    internal interface IActor : IDisposable
    {
        AnimationServer Animation { get; }

        IReadOnlyList<Rectangle> BoundingBoxes { get; }

        Size GridSize { get; }

        Point Location { get; set; }

        bool Rendered { get; set; }

        event LocationChangedEventHandler LocationChanged;

        void Update();

        void Dispose();
    }
}
