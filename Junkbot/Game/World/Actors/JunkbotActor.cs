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
    internal class JunkbotActor : IThinker
    {
        public AnimationServer Animation { get; private set; }

        public IReadOnlyList<System.Drawing.Rectangle> BoundingBoxes { get { return this._BoundingBoxes; } }
        private IReadOnlyList<System.Drawing.Rectangle> _BoundingBoxes = new List<System.Drawing.Rectangle>(new System.Drawing.Rectangle[]
        {
            new System.Drawing.Rectangle(0, 0, 2, 3),
            new System.Drawing.Rectangle(0, 3, 1, 1)
        }).AsReadOnly();

        public String Type { get; set; } = "JunkbotActor";
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
        private static readonly Size _GridSize = new Size(2, 4);

        public FacingDirection FacingDirection;

        private Scene Scene;

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

        private void SetWalkingDirection(FacingDirection direction)
        {
            FacingDirection = direction;


            // Detach event if necessary
            //
            try
            {
                Animation.SpecialFrameEntered -= Animation_SpecialFrameEntered;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

        public System.Drawing.Rectangle GetCheckBounds(Point point, Size size)
        {
            System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(
               Location.Add(point), size);

            return checkBounds;
        }

        private void CollectTrash()
        {
            //Animation.GoToAndPlay("junkbot-trash-left");
        }

        private JunkbotCollision CheckCollisionType(System.Drawing.Rectangle region)
        {
            Point[] cellsToCheck = region.ExpandToGridCoordinates();
            IList<JunkbotCollision> detectionResults = new List<JunkbotCollision>();
            int dx = FacingDirection == FacingDirection.Left ? -1 : 1;
            bool StepGapRight = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx, GridSize.Height), new Size(2, 1)));
            bool StepGapLeft = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx, GridSize.Height), new Size(2, 1)));
            bool Floor2 = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx, GridSize.Height), new Size(2, 1)));
            bool stepDown = false;
            bool floor = false;
            bool stepUp = false;
            bool stepDownBlocked = false;
            bool turnAround = false;
            bool stepUpBlocked = false;
            int index = 1;

            foreach (Point cell in cellsToCheck)
            {
                if (index != 7)
                {


                    if (index == 2 || index == 3 || index == 4) 
                    {
                        if (Scene.GetPlayfield[cell.X, cell.Y] != null)
                        {
                            if (Scene.GetPlayfield[cell.X, cell.Y] is BinActor)
                            {
                                CollectTrash();
                            }
                            detectionResults.Add((JunkbotCollision)2);
                        }
                    }
                    else
                    {
                        if (index == 6)
                        {
                            if ((Scene.GetPlayfield[cell.X, cell.Y] != null))
                            {
                                detectionResults.Add((JunkbotCollision)index);
                            }
                        }
                        else
                        {
                            if (Scene.GetPlayfield[cell.X, cell.Y] != null)
                            {
                                detectionResults.Add((JunkbotCollision)index);
                            }
                        }
                    }
                }

                else
                {
                    if (FacingDirection == FacingDirection.Left)
                    {
                        if ((Scene.GetPlayfield[cell.X + 1, cell.Y] != null) || (Scene.GetPlayfield[cell.X, cell.Y] != null))
                        {
                            detectionResults.Add((JunkbotCollision)index);
                        }
                    }
                    else
                    {
                        if ((Scene.GetPlayfield[cell.X - 1, cell.Y] != null) || (Scene.GetPlayfield[cell.X, cell.Y] != null))
                        {
                            detectionResults.Add((JunkbotCollision)index);
                        }
                    }
                }
                index += 1;
            }

            foreach (JunkbotCollision result in detectionResults)
            {
                switch (result)
                {
                    case JunkbotCollision.StepUpBlocked:
                        stepUpBlocked = true;
                        break;

                    case JunkbotCollision.TurnAround:
                        turnAround = true;
                        break;

                    case JunkbotCollision.StepDownBlocked:
                        stepDownBlocked = true;
                        break;

                    case JunkbotCollision.StepUp:
                        stepUp = true;
                        break;

                    case JunkbotCollision.Floor:
                        floor = true;
                        break;

                    case JunkbotCollision.StepDown:
                        stepDown = true;
                        break;
                }
                JunkbotCollision collisionType = result;
            }

            if (turnAround)
            {
                return JunkbotCollision.TurnAround;
            }
            else
            {
                if (stepDown && !stepDownBlocked && !stepUp && Floor2 && !floor)
                {
                    if ((FacingDirection == FacingDirection.Left && StepGapLeft) || (FacingDirection == FacingDirection.Right && StepGapRight))
                    {
                        return JunkbotCollision.StepDown;
                    }
                }
                if (stepUp && !stepUpBlocked)
                {
                    return JunkbotCollision.StepUp;
                }
                if (!Floor2 && !stepDownBlocked && !stepUp)
                {
                    return JunkbotCollision.CanWalk;
                }
                return JunkbotCollision.TurnAround;
            }
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
                JunkbotCollision collisionType = CheckCollisionType(GetCheckBounds(new Point(tar, -1), new Size(1, 7)));

                switch (collisionType)
                {
                    case JunkbotCollision.TurnAround:
                        SetWalkingDirection(FacingDirection == FacingDirection.Left ? FacingDirection.Right : FacingDirection.Left);
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
                public void Think(TimeSpan deltaTime)
        {

        }
    }
}
