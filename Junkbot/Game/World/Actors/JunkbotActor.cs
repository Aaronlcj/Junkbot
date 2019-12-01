using Pencil.Gaming.MathUtils;
using Junkbot.Game.World.Actors.Animation;
using Junkbot.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.Logic;
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
            Animation.Progress();
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
                Animation.GoToAndPlay("junkbot-eat");
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

            if (Scene != null)
            {
                //quick maffs:

                int dx = FacingDirection == FacingDirection.Left ? -1 : 1;
                int tar = FacingDirection == FacingDirection.Left ? -1 : 2;

                // Collision detection
                int yPos = 0;
                JunkbotCollision collisionType = CollisionDetection.CheckCollisionType(this, GetCheckBounds(new Point(tar, -1), new Size(1, 7)));
                if (sender != null)
                {
                    Console.WriteLine((sender as ActorAnimation).Name);
                    if ((sender as ActorAnimation).Name.Contains("junkbot_eat"))
                    {
                        IsWalking = true;
                    }
                }

                if (IsWalking)
                {
                    switch (collisionType)
                    {
                        case JunkbotCollision.TurnAround:
                            SetWalkingDirection(FacingDirection == FacingDirection.Left
                                ? FacingDirection.Right
                                : FacingDirection.Left);
                            return;
                        case JunkbotCollision.StepUp:
                            yPos = -1;
                            break;
                        case JunkbotCollision.StepDown:
                            yPos = +1;
                            break;
                    }

                    Location = Location.Add(new Point(dx, yPos));
                }
            }
        }
                public void Think(TimeSpan deltaTime)
        {

        }
    }
}
