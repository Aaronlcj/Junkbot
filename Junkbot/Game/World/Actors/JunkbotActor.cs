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
            int dx = FacingDirection == FacingDirection.Left ? -1 : 1;

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

                // Cell detection
                bool doesFloorExist = Scene.CheckFloorExists(this);
                bool TurnAround = Scene.CheckGridRegionFree(GetCheckBounds(new Point(tar, 0), new Size(1, 3)));
                bool Ceiling = Scene.CheckGridRegionFree(GetCheckBounds(new Point(-1, -1), new Size(4, 1)));
                bool StepUp = Scene.CheckGridRegionFree(GetCheckBounds(new Point(tar, GridSize.Height -1), new Size(1, 1)));
                bool StepUpBlocked = Scene.CheckGridRegionFree(GetCheckBounds(new Point(tar, -1), new Size(1, 4)));
                bool StepDown1 = Scene.CheckGridRegionFree(GetCheckBounds(new Point(0, GridSize.Height + 1), new Size(1, 1)));
                bool StepDown2 = Scene.CheckGridRegionFree(GetCheckBounds(new Point(0, GridSize.Height + 1), new Size(1, 2)));
                bool StepDownBlocked = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx * GridSize.Width, +1), new Size(1, 4)));
                bool Floor = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx, GridSize.Height), new Size(1, 1)));
                bool Gap = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx * GridSize.Width, GridSize.Height), new Size(2, 2)));
                bool StepGapRight = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx, GridSize.Height), new Size(2, 1)));
                bool StepGapLeft = Scene.CheckGridRegionFree(GetCheckBounds(new Point(dx, GridSize.Height), new Size(2, 1)));
                int locMods = 0;

                Console.WriteLine(Location.ToString());

                if (!TurnAround)
                {
                    SetWalkingDirection(FacingDirection == FacingDirection.Left ? FacingDirection.Right : FacingDirection.Left);
                    return;
                }
                else
                {

                    if (!StepUp && StepUpBlocked)
                    {
                        locMods = -1;
                    }
                    else
                    {
                        if (FacingDirection == FacingDirection.Right)
                        {


                            if ((!StepDown1 && StepGapRight) || (!StepDown2 && StepGapRight))
                            {
                                locMods = +1;
                            }
                        }
                        if (FacingDirection == FacingDirection.Left)
                        {
                            if ((!StepDown1 && StepGapLeft) || (!StepDown2 && StepGapLeft))
                            {
                                locMods = +1;
                            }
                        }
                    }
                    Location = Location.Add(new Point(dx, locMods));
                    return;
                }
            }
        }
                public void Think(TimeSpan deltaTime)
        {

        }
    }
}
