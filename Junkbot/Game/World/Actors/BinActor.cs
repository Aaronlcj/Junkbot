using Junkbot.Game.World.Actors.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World.Actors
{
    internal class BinActor : IActor
    {
        public AnimationServer Animation { get; private set; }

        public IReadOnlyList<Rectangle> BoundingBoxes { get { return this._BoundingBoxes; } }
        private IReadOnlyList<Rectangle> _BoundingBoxes;

        public Size GridSize { get { return _GridSize; } }
        private static readonly Size _GridSize = new Size(1, 2);
        public String Type { get; set; } = "BinActor";

        public Point Location
        {
            get { return _Location; }
            set
            {
                Point oldLocation = _Location;
                _Location = value;

                LocationChanged?.Invoke(this, new LocationChangedEventArgs(oldLocation, value));
            }
        }
        private Point _Location;

        public event LocationChangedEventHandler LocationChanged;


        public BinActor(AnimationStore store, Point location)
        {
            Animation = new AnimationServer(store);
            _BoundingBoxes = new List<Rectangle>().AsReadOnly();
            Location = location;
        }


        public void Update()
        {
            Animation.Progress();
        }

    }
}
