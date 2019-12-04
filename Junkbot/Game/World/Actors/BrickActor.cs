using Junkbot.Game.World.Actors.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Junkbot.Game.World.Actors
{
    internal class BrickActor : IActor
    {
        public static IList<Color> ValidColors = new List<Color>(new Color[]
        {
            Color.Red, Color.Yellow, Color.White, Color.Green, Color.Blue
        }).AsReadOnly();

        public bool Rendered { get; set; }
        public bool Selected { get; set; }
        public bool CanMove { get; set; }

        public AnimationServer Animation { get; private set; }

        public IReadOnlyList<Rectangle> BoundingBoxes { get { return this._BoundingBoxes; } }
        private IReadOnlyList<Rectangle> _BoundingBoxes;

        public Color Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
                UpdateBrickAnim();
            }
        }
        private Color _Color;

        public Size GridSize { get { return _GridSize; } }
        private Size _GridSize;

        public bool IsImmobile
        {
            get { return _Color.Name == "Gray"; }
        }

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
        public Point MovingLocation
        {
            get { return _MovingLocation; }
            set
            {
                Point oldMovingLocation = _MovingLocation;
                _MovingLocation = value;

                MovingLocationChanged?.Invoke(this, new LocationChangedEventArgs(oldMovingLocation, value));
            }
        }
        private Point _MovingLocation;
        public BrickSize Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                _BoundingBoxes = new List<Rectangle>(new Rectangle[] {
                    new Rectangle(0, 0, (int)value, 1)
                }).AsReadOnly();
                _GridSize = new Size((int)value, 1);
                UpdateBrickAnim();
            }
        }
        private BrickSize _Size;
        
        private Scene Scene;
        private void SetBoundingBox(BrickSize size)
        {
            _Size = size;
            _BoundingBoxes = new List<Rectangle>(new Rectangle[] {
                    new Rectangle(0, 0, (int)size, 1)
                }).AsReadOnly();
        }
            public bool CanBePlaced()
        {
            var thisBounds = new System.Drawing.Rectangle(new Point(this.MovingLocation.X, this.MovingLocation.Y), this.GridSize);
            var upBounds = new System.Drawing.Rectangle(new Point(this.MovingLocation.X, this.MovingLocation.Y - 1), this.GridSize);
            var downBounds = new System.Drawing.Rectangle(new Point(this.MovingLocation.X, this.MovingLocation.Y + 1), this.GridSize);
            bool downBlocked = Scene.CheckGridRegionFree(downBounds);
            /* var leftBounds = new System.Drawing.Rectangle(new Point(this.BoundLocation.X - 1, this.BoundLocation.Y), new Size(1, 1));
             var rightBounds = new System.Drawing.Rectangle(new Point(this.BoundLocation.X + this.GridSize.Width - 1, this.BoundLocation.Y), new Size(1, 1));*/
            bool upBlocked = Scene.CheckGridRegionFree(upBounds);
           /* bool leftBlocked = Scene.CheckGridRegionFree(leftBounds);
            bool rightBlocked = Scene.CheckGridRegionFree(rightBounds);*/
            bool thisBlocked = Scene.CheckGridRegionFree(thisBounds);

           /* do
            {

            var studCheck = new System.Drawing.Rectangle(new Point(this.BoundLocation.X + currentCell, this.BoundLocation.Y + 1), new Size(1, 1));
            studExists = Scene.CheckGridRegionFree(downBounds);*/
                if (!thisBlocked || (!upBlocked && !downBlocked) ||(upBlocked && downBlocked) )
                     
                {
                    return false;
                }
                else
                {
                    return true;
                }
            /*}
            while (currentCell <= this.GridSize.Width);*/

        }
        public bool CanBePicked()
        {
            var upBounds = new System.Drawing.Rectangle(new Point(this.MovingLocation.X, this.MovingLocation.Y - 1), this.GridSize);
            var downBounds = new System.Drawing.Rectangle(new Point(this.MovingLocation.X, this.MovingLocation.Y + 1), this.GridSize);
            bool downBlocked = Scene.CheckGridRegionFree(downBounds);
            bool upBlocked = Scene.CheckGridRegionFree(upBounds);

            if (!upBlocked && !downBlocked)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public event LocationChangedEventHandler LocationChanged;
        public event LocationChangedEventHandler MovingLocationChanged;


        public BrickActor(AnimationStore store, Scene scene, Point location, Color color, BrickSize size)
        {
            Animation = new AnimationServer(store);
            _BoundingBoxes = new List<Rectangle>().AsReadOnly();
            _Color = color;
            _GridSize = new Size((int)size, 1);
            Location = location;
            _Size = size;
            Rendered = false;
            Selected = false;
            SetBoundingBox(size);
            UpdateBrickAnim();
            MovingLocation = new Point();
            Scene = scene;
            CanMove = color.Name != "Gray" ? true : false;
        }


        public void Update()
        {
            Animation.Progress();
        }


        private void UpdateBrickAnim()
        {
            string brickSize = ((int)Size).ToString();

            if (IsImmobile)
                Animation.GoToAndStop("legopart-brick-immobile-" + brickSize);
            else
            {
                if (!ValidColors.Contains(Color))
                    return;

                Animation.GoToAndStop("legopart-brick-" + Color.Name.ToLower() + "-" + brickSize);
            }
        }
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                Animation.StopPlaying();
                Animation = null;
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
        ~BrickActor()
        {

            Dispose(false);
            System.Diagnostics.Trace.WriteLine("LevelSelect's destructor is called.");
        }
    }
}
