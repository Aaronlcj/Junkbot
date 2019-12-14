using Pencil.Gaming.MathUtils;
using Junkbot.Game.World.Actors.Animation;
using Junkbot.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.Logic;
using Microsoft.Win32.SafeHandles;
using Rectangle = System.Drawing.Rectangle;

namespace Junkbot.Game.World.Actors
{
    internal class JunkbotActor : IBotActor
    {
        public AnimationServer Animation { get; private set; }

        public IReadOnlyList<System.Drawing.Rectangle> BoundingBoxes { get { return this._BoundingBoxes; } }
        private IReadOnlyList<System.Drawing.Rectangle> _BoundingBoxes = new List<System.Drawing.Rectangle>(new System.Drawing.Rectangle[]
        {
            new System.Drawing.Rectangle(0, 0, 2, 3),
            new System.Drawing.Rectangle(0, 3, 1, 1)
        }).AsReadOnly();

        public bool Rendered { get; set; }
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
        private bool IsWalking = true;
        private IReadOnlyList<Rectangle> CollisionBounds = new List<Rectangle>( new Rectangle[]
        {
            new Rectangle(0,0,0,0),
            new Rectangle(0,0,0,0)
        }).AsReadOnly();

        public Size GridSize { get { return _GridSize; } }
        private static readonly Size _GridSize = new Size(2, 4);

        public FacingDirection FacingDirection { get; set; }

        public Scene Scene { get; set; }

        public event LocationChangedEventHandler LocationChanged;

        public JunkbotActor(AnimationStore store, Scene scene, Point location, FacingDirection initialDirection)
        {
            Animation = new AnimationServer(store);
            Location = location;
            SetWalkingDirection(initialDirection);
            Scene = scene;
            Rendered = false;
        }

        public void Update()
        {
            if (Animation != null)
            {
                Animation.Progress();
            }
        }

        public void SetWalkingDirection(FacingDirection direction)
        {
            FacingDirection = direction;

            if (IsWalking)
            {
                // Detach event if necessary
                //
                try
                {
                    Animation.SpecialFrameEntered -= Animation_SpecialFrameEntered;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SFE exception: " + ex.ToString());
                }


                switch (direction)
                {
                    case FacingDirection.Left:
                        Animation.GoToAndPlay("junkbot-walk-left");
                        break;

                    case FacingDirection.Right:
                        Animation.GoToAndPlay("junkbot-walk-right");
                        break;

                    default:
                        throw new Exception("JunkbotActor.SetWalkingDirection: Invalid direction provided.");
                }

         
            Animation.SpecialFrameEntered += Animation_SpecialFrameEntered;
            }
        }

    
        private void CollectTrash()
        {
            try
            {
                Animation.SpecialFrameEntered -= Animation_SpecialFrameEntered;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (IsWalking)
            {
                var animName = FacingDirection == FacingDirection.Left ? "junkbot-walk-left" : "junkbot-walk-right";
                Animation.GoToAndStop(animName);
                IsWalking = false;
                var binCell = Scene.GetPlayfield[Location.X + 2, Location.Y + 3];
                try
                {
                    foreach (IActor bin in Scene.MobileActors)
                    {
                        if (bin == binCell)
                        {
                            Scene.GetPlayfield[bin.Location.X, bin.Location.Y] = null;
                            Scene.GetPlayfield[bin.Location.X, bin.Location.Y + 1] = null;
                            Scene.GetPlayfield[bin.Location.X + 1, bin.Location.Y] = null;
                            Scene.GetPlayfield[bin.Location.X + 1, bin.Location.Y + 1] = null;

                        }
                    }
                }
                catch
                {
                    Console.WriteLine("NO BIN MATE");
                }
                Animation.GoToAndPlay("junkbot-eat");                
                Scene.LevelStats.CollectedTrashCount += 1;
                Animation.SpecialFrameEntered += Animation_SpecialFrameEntered;
            }
        }
        public System.Drawing.Rectangle GetCheckBounds(Point point, Size size)
        {
            System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(
                Location.Add(point), size);

            return checkBounds;
        }

        private void Animation_SpecialFrameEntered(object sender, EventArgs e)
        {
            Console.WriteLine(Location);
            if (Scene != null)
            {
                //quick maffs:

                int dx = FacingDirection == FacingDirection.Left ? -1 : 1;

                int tar = FacingDirection == FacingDirection.Left ? -1 : 2;

                // Collision detection
                int yPos = 0;
                

                if (IsWalking)
                {
                    JunkbotCollision collisionType = Scene.CollisionDetection.CheckCollisionType(this);
                    if (sender != null)
                    {
                        Console.WriteLine((sender as ActorAnimation).Name);
                        if ((sender as ActorAnimation).Name.Contains("junkbot_eat"))
                        {
                        }
                    }
                    switch (collisionType)
                    {
                        case JunkbotCollision.TurnAround:
                            SetWalkingDirection(FacingDirection == FacingDirection.Left
                                ? FacingDirection.Right
                                : FacingDirection.Left);
                            break;
                        case JunkbotCollision.StepUp:
                            Location = Location.Add(new Point(dx, -1));
                            ;
                            break;
                        case JunkbotCollision.StepDown:
                            Location = Location.Add(new Point(dx, 1));
                            break;
                        case JunkbotCollision.EatTrash:
                            CollectTrash();
                            break;
                        case JunkbotCollision.CanWalk:
                            Location = Location.Add(new Point(dx, 0));
                            break;
                    }

                }
                else
                {
                    var currentFrame = Animation.GetCurrentFrame();
                    if (currentFrame.SpriteName.Contains("junkbot_eat"))
                    {
                        if (Animation.CompareFrame(currentFrame))
                        {
                            var status = Scene.LevelStats.CheckTrashStatus();
                            if (!status)
                            {
                                IsWalking = true;
                                SetWalkingDirection(FacingDirection);
                            }
                            else
                            {
                                Animation.StopPlaying();
                            }
                        }
                    }
                }
            }
        }
                public void Think(TimeSpan deltaTime)
        {

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
        ~JunkbotActor()
        {

            Dispose(false);
            System.Diagnostics.Trace.WriteLine("LevelSelect's destructor is called.");
        }
    }
}
