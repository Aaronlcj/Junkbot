using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.World.Actors;
using Junkbot.Helpers;

namespace Junkbot.Game.Logic
{
    static class CollisionDetection
    {
        public static Scene Scene;
        public static System.Drawing.Rectangle GetCheckBounds(IBotActor actor, Point point, Size size)
        {
            System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(
                actor.Location.Add(point), size);

            return checkBounds;
        }

        public static JunkbotCollision CheckCollisionType(IBotActor actor, System.Drawing.Rectangle region)
        {
            if (actor is JunkbotActor)
            {
                Point[] cellsToCheck = region.ExpandToGridCoordinates();
                IList<JunkbotCollision> detectionResults = new List<JunkbotCollision>();
                int dx = actor.FacingDirection == FacingDirection.Left ? -1 : 2;
                int sd = actor.FacingDirection == FacingDirection.Left ? 0 : 1;
                int su = actor.FacingDirection == FacingDirection.Left ? -1 : 1;

                int fl = actor.FacingDirection == FacingDirection.Left ? 0 : 2;
                int fl2 = actor.FacingDirection == FacingDirection.Left ? -1 : 3;

                bool StepGapRight =
                    Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, actor.GridSize.Height),
                        new Size(2, 1)));
                bool StepGapLeft =
                    Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, actor.GridSize.Height),
                        new Size(2, 1)));
                bool StepUpBlocked =
                    Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, -1), new Size(1, 1)));
                bool TurnAround = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 1), new Size(1, 1)));
                bool TurnAround2 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 2), new Size(1, 1)));
                bool HeadCheck = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 0), new Size(1, 1)));

                bool StepUp = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(su, 3), new Size(2, 1)));
                bool Floor1 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(fl, 4), new Size(1, 1)));
                bool Floor2 = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(fl2, 4), new Size(1, 1)));
                bool StepDown = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(sd, 5), new Size(1, 1)));
                bool StepDownFloor = Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(sd, 6), new Size(1, 1)));
                bool StepDownBlocked =
                    Scene.CheckGridRegionFree(GetCheckBounds(actor, new Point(dx, 5), new Size(2, 1)));

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
                    return JunkbotCollision.TurnAround;
                }
                else
                {
                    if (StepDown && StepDownBlocked && (Floor1 && Floor2) && !StepUp)
                    {
                        if ((actor.FacingDirection == FacingDirection.Left && StepGapLeft) ||
                            (actor.FacingDirection == FacingDirection.Right && StepGapRight))
                        {
                            return JunkbotCollision.StepDown;
                        }
                    }

                    if (!StepUp && StepUpBlocked)
                    {
                        return JunkbotCollision.StepUp;
                    }

                    if ((Floor1 || Floor2) && stepUp && HeadCheck)
                    {
                        return JunkbotCollision.CanWalk;
                    }

                    return JunkbotCollision.CanWalk;
                }
            }
            else
            {
                return JunkbotCollision.CanWalk;
            }
        }

    }
}
