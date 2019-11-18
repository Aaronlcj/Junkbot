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
    internal class BotActor : IThinker
    {
        public AnimationServer Animation { get; private set; }

        public IReadOnlyList<System.Drawing.Rectangle> BoundingBoxes { get { return this._BoundingBoxes; } }
        private IReadOnlyList<System.Drawing.Rectangle> _BoundingBoxes = new List<System.Drawing.Rectangle>(new System.Drawing.Rectangle[]
        {
            new System.Drawing.Rectangle(0, 0, 3, 2)
        }).AsReadOnly();
        public String Type { get; set; } = "BotActor";

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

        public Size GridSize { get { return _GridSize; } }
        private static readonly Size _GridSize = new Size(2, 2);


        public FacingDirection FacingDirection;

        private Scene Scene;


        public event LocationChangedEventHandler LocationChanged;


        public BotActor(AnimationStore store, Scene scene, Point location, FacingDirection initialDirection)
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


        private void SetWalkingDirection(FacingDirection direction)
        {
            FacingDirection = direction;

            // Detach event if necessary
            //
            /* try
             {
                 Animation.SpecialFrameEntered -= Animation_SpecialFrameEntered;
             }
             catch (Exception ex) { }*/
            // rewrite
            switch (direction)
            {
                case FacingDirection.Left:
                    Animation.GoToAndPlay("climbbot-walk-left");
                    break;

                case FacingDirection.Right:
                    Animation.GoToAndPlay("climbbot-walk-right");
                    break;
                case FacingDirection.Up:
                    Animation.GoToAndPlay("climbbot-walk-up");
                    break;
                case FacingDirection Down:
                    Animation.GoToAndPlay("climbbot-walk-down");
                    break;

                default:
                    throw new Exception("JunkbotActor.SetWalkingDirection: Invalid direction provided.");
            }

            Animation.SpecialFrameEntered += Animation_SpecialFrameEntered;
        }

        public System.Drawing.Rectangle GetCheckBounds()
        {
            int dx = FacingDirection == FacingDirection.Left ? -1 : 1;

            System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(
               Location.Add(new Point(dx * GridSize.Width, 0)),
               new Size(1, 2)
               );

            return checkBounds;
        }
        private void Animation_SpecialFrameEntered(object sender, EventArgs e)
        {
            if (Scene != null)
            {
                bool doesFloorExist = Scene.CheckBotFloorExists(this);
                bool boundaryCheck = Scene.BoundaryCheck(GetCheckBounds());
                
                if (boundaryCheck)
                {
                    switch (FacingDirection)
                    {
                        case FacingDirection.Left:
                            SetWalkingDirection(FacingDirection.Right);
                            break;
                        case FacingDirection.Right:
                            SetWalkingDirection(FacingDirection.Left);
                            break;
                        case FacingDirection.Up:
                            SetWalkingDirection(FacingDirection.Down);
                            break;
                        case FacingDirection.Down:
                            SetWalkingDirection(FacingDirection.Up);
                            break;
                    }
                }
                /*if (doesFloorExist)
                    {
                    SetWalkingDirection(FacingDirection.Right);
                }*/
                if (!doesFloorExist && FacingDirection != FacingDirection.Down && !boundaryCheck)
                {
                    SetWalkingDirection(FacingDirection.Down);
                    /*Scene.CheckBotFloorExists(this);
                    SetWalkingDirection(FacingDirection == FacingDirection.Left ? FacingDirection.Right : FacingDirection.Left);*/
                }
            }

            int dx = 0;
            int vx = 0;
            // Each tile is 15x18
            switch (FacingDirection)
            {
                case FacingDirection.Left:
                    dx -= 1;
                    break;
                case FacingDirection.Right:
                    dx += 1;
                    break;
                case FacingDirection.Up:
                    vx -= 1;
                    break;
                case FacingDirection.Down:
                    vx += 1;
                    break;
            }
            // Check if we should turn around now
            //
            /*   System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(
                   Location.Add(new Point(dx * GridSize.Width, 0)),
                   new Size(2, 2)
                   );*/

            /*     if (!Scene.CheckGridRegionFree(checkBounds))
                 {
                     Location = Location.Add(new Point(dx, 0));

                     SetWalkingDirection(FacingDirection == FacingDirection.Left ? FacingDirection.Right : FacingDirection.Left);
                     return;
                 }*/

            // Space is free, now check whether we need an elevation change, prioritize upwards changes
            //
            /*  System.Drawing.Rectangle floorUpCheckBounds = new System.Drawing.Rectangle(
                  Location.Add(new Point(dx, GridSize.Height - 1)),
                  new Size(1, 1)
                  );

              if (!Scene.CheckGridRegionFree(floorUpCheckBounds))
              {
                  // Elevate up
                  //
                  Location = Location.Add(new Point(dx, -1));
                  return;
              }

              // Now check downwards
              //
              System.Drawing.Rectangle floorMissingCheckBounds = new System.Drawing.Rectangle(
                  Location.Add(new Point(dx, GridSize.Height)),
                  new Size(1, 1)
                  );

              System.Drawing.Rectangle floorDownCheckBounds = new System.Drawing.Rectangle(
                  Location.Add(new Point(dx, GridSize.Height + 1)),
                  new Size(1, 1)
                  );

              if (Scene.CheckGridRegionFree(floorMissingCheckBounds) && !Scene.CheckGridRegionFree(floorDownCheckBounds))
              {
                  // Lower junkbot
                  //
                  Location = Location.Add(new Point(dx, 1));
                  return;
              }*/

            Location = Location.Add(new Point(dx, vx));
        }
        //public Vector2 Location { get; set; }


        //public JunkbotActor()
        //{
        //    Location = Vector2.Zero;
        //}


        public void Think(TimeSpan deltaTime)
        {

        }
    }
}
