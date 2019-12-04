using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.World.Actors;
using Junkbot.Helpers;
using Microsoft.Win32.SafeHandles;

namespace Junkbot.Game.Logic
{
    internal class CollisionDetection : IDisposable
    {
        internal Scene Scene { get; set; }

        public CollisionDetection(Scene scene)
        {
            Scene = scene;
        }
        public static System.Drawing.Rectangle GetCheckBounds(IBotActor actor, Point point, Size size)
        {
            System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(
                actor.Location.Add(point), size);

            return checkBounds;
        }

        public JunkbotCollision CheckCollisionType(IBotActor actor)
        {
            if (actor is JunkbotActor)
            {
                //Point[] cellsToCheck = region.ExpandToGridCoordinates();
                IList<JunkbotCollision> detectionResults = new List<JunkbotCollision>();
                int dx = actor.FacingDirection == FacingDirection.Left ? -1 : 2;
                int sd = actor.FacingDirection == FacingDirection.Left ? 0 : 1;
                int su = actor.FacingDirection == FacingDirection.Left ? -1 : 1;
                bool StepUpBlocked =
                    Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, -1), new Size(1, 1)));
                bool TurnAround = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 1), new Size(1, 1)));
                bool TurnAround2 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 2), new Size(1, 1)));
                bool HeadCheck = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 0), new Size(1, 1)));
                bool StepUp = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 3), new Size(1, 1)));
                bool Floor1 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(sd, 4), new Size(1, 1)));
                bool Floor2 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 4), new Size(1, 1)));
                bool StepDown1 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(sd, 5), new Size(1, 1)));
                bool StepDown2 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 5), new Size(1, 1)));
                bool StepDownBlocked = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(su, 4), new Size(2, 1)));

                bool stepDown = false;
                bool floor = false;
                bool stepUp = false;
                bool stepDownBlocked = false;
                bool turnAround = false;
                bool stepUpBlocked = false;
                int index = 1;

                /*foreach (Point cell in cellsToCheck)
                {
                    if (index != 7)
                    {
    
    
                        if (index == 2 || index == 3 || index == 1) 
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
                }*/

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

                if (!TurnAround | !TurnAround2 || (!HeadCheck && !StepUp))
                {
                    var cell = Scene.GetPlayfield[actor.Location.X + 2, actor.Location.Y + 3];
                    if (cell is BinActor)
                    {
                        return JunkbotCollision.EatTrash;
                    }
                    else
                    {
                        return JunkbotCollision.TurnAround;

                    }
                }
                else
                {

                    if ((!StepDown1 || StepDown1 && !StepDown2) && StepDownBlocked && (Floor1 && Floor2) && StepUp)
                    {
                        /*if ((actor.FacingDirection == FacingDirection.Left && StepGapLeft) ||
                            (actor.FacingDirection == FacingDirection.Right && StepGapRight))
                        {*/
                            return JunkbotCollision.StepDown;
                        //}
                    }

                    if (!StepUp && StepUpBlocked)
                    {
                        return JunkbotCollision.StepUp;
                    }

                    if ((!Floor1 || !Floor2))
                    {
                        return JunkbotCollision.CanWalk;
                    }
                    Console.WriteLine("no collision matches - turn around");
                    return JunkbotCollision.TurnAround;
                }
            }
            else
            {
                return JunkbotCollision.CanWalk;
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

                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
        ~CollisionDetection()
        {
            Dispose(false);
            System.Diagnostics.Trace.WriteLine("CollisionDetection's destructor is called.");
        }
    }
}
