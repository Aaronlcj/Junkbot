using Junkbot.Game.World.Actors;
using Junkbot.Game.World.Actors.Animation;
using Junkbot.Game.World.Level;
using Junkbot.Helpers;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Input;
using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Drawing;
using System.IO;

namespace Junkbot.Game.State
{
    /// <summary>
    /// Represents the main menu game state.
    /// </summary>
    internal class DemoGameState : GameState
    {
        public override InputFocalMode FocalMode
        {
            get { return InputFocalMode.Always; }
        }
        public static string[] lvl;
        public Scene Scene;
        public static AnimationStore Store = new AnimationStore();

        public DemoGameState(string level)
        {
            lvl = File.ReadAllLines(Environment.CurrentDirectory + $@"\Content\Levels\{level}.txt");
            Scene = Scene.FromLevel(lvl, Store);
        }
        public override string Name
        {
            get { return "DemoGame"; }
        }


        public override void RenderFrame(IGraphicsController graphics)
        {
            Scene test = Scene;

            var sb = graphics.CreateSpriteBatch("actors-atlas");
            var bck = graphics.CreateSpriteBatch("background-atlas");
            var dec = graphics.CreateSpriteBatch("decals-atlas");

            graphics.ClearViewport(Color.CornflowerBlue);
            if (Scene.LevelData.Backdrop != null)
            {
                bck.Draw(
                         Scene.LevelData.Backdrop,
                         new Rectangle(
                             Point.Empty, graphics.TargetResolution
                             )
                         );
                bck.Finish();
            }
            /*foreach (IActor brick in test.GetPlayfield)
            {
                if (brick != null)
                {
                    ActorAnimationFrame currentFrame = brick.Animation.GetCurrentFrame();
                    Point pointLoc = Point.Add(currentFrame.Offset, test.CellSize);
                    Point drawLoc = new Point(pointLoc.X, pointLoc.Y);
                    if (brick.BoundingBoxes.Count <= 1)
                    {
                        Rectangle boundingbox = brick.BoundingBoxes[0];

                        sb.Draw(
                        currentFrame.SpriteName,
                        new Rectangle(
                            brick.Location,
                        new Size(boundingbox.Width, boundingbox.Height)
                           )
                       );
                    }
                }
            }*/
            /*            IActor item = Scene.GetPlayfield[0, 10];
                        IActor b2 = Scene.GetPlayfield[8, 9];*/

            foreach (IActor item in Scene.ImmobileBricks)
            {
                if (item != null)
                {
                    ActorAnimationFrame currentFrame = item.Animation.GetCurrentFrame();
                    int locX;
                    int locY;
                    int sizY;

                    if (item.BoundingBoxes.Count <= 1)
                    {
                        Rectangle testtt = item.BoundingBoxes[0];
                        Size testttt = item.GridSize;
                        if (item.Location.X != 0)
                        {
                            locX = (item.Location.X * 15);
                        }
                        else
                        {
                             locX = item.Location.X;
                        }

                        if (item.Location.Y != 0)
                        {
                            locY = (item.Location.Y  * 18) + 10;
                        }
                        else
                        {
                            locY = item.Location.Y + 10;
                        }

                        if (item.GridSize.Height != 1)
                        {
                            sizY = (item.GridSize.Height - 1) * 18 + 14;
                        }
                        else
                        {
                            sizY = item.GridSize.Height * 32;
                        }
                        int sizX = (item.GridSize.Width - 1) * 15 + 26;
                        if (item == Scene.GetPlayfield[8,9])
                        {
                        }
                        sb.Draw(
                         currentFrame.SpriteName,
                         new Rectangle(
                             new Point(locX, locY), new Size(sizX, sizY)
                             )
                         );
                    }
                }
            }


           /* if (Scene.LevelData.Decals != null)
            {
                foreach (JunkbotDecalData decal in Scene.LevelData.Decals)
                {


                    dec.Draw(
                             decal.Decal,
                             new Rectangle(
                                 dec.GetSpriteMap[decal.Decal], graphics.TargetResolution
                                 )
                             );
                }
            }*/
            if (Scene.MobileActors != null)
            {
                foreach (IActor actor in Scene.MobileActors)
                {
                    Type type = actor.GetType();
                    if (type.Name == "BotActor")
                    {
                        IActor climb_bot = Scene.MobileActors[2];
                        ActorAnimationFrame frame = climb_bot.Animation.GetCurrentFrame();
                        int locX;
                        int locY;
                        if (climb_bot.Location.X != 0)
                        {
                            if (climb_bot.Location.X == 1)
                            {
                                locX = 32;
                            }
                            else
                            {
                                locX = (climb_bot.Location.X * 15);
                            }
                        }
                        else
                        {
                            locX = climb_bot.Location.X;
                        }

                        if (climb_bot.Location.Y != 0)
                        {
                            locY = (climb_bot.Location.Y + 1)  * 18;
                        }
                        else
                        {
                            locY = climb_bot.Location.Y;
                        }
                        int sizX = ((climb_bot.GridSize.Width - 1) * 15) + 5;
                        int sizY = (climb_bot.GridSize.Height * 18) + 2;
                        sb.Draw(
                                 "climbbot_walk_r_1",
                                 new Rectangle(
                                     new Point(locX, locY), new Size(sizX, sizY)
                                     )
                                 );
                        sb.Finish();
                    }
                    if (type.Name == "BinActor")
                    {
                        IActor bin = Scene.MobileActors[1];
                        ActorAnimationFrame frame = bin.Animation.GetCurrentFrame();
                        int locX;
                        int locY;
                        if (bin.Location.X != 0)
                        {
                            if (bin.Location.X == 1)
                            {
                                locX = 32;
                            }
                            else
                            {
                                locX = (bin.Location.X * 15) + 14;
                            }
                        }
                        else
                        {
                            locX = bin.Location.X + 2;
                        }

                        if (bin.Location.Y != 0)
                        {
                            locY = (bin.Location.Y * 18) + 10;
                        }
                        else
                        {
                            locY = bin.Location.Y;
                        }
                        int sizX = 31;
                        int sizY = 47;
                        sb.Draw(
                                 "bin",
                                 new Rectangle(
                                     new Point(locX, locY), new Size(sizX, sizY)
                                     )
                                 );
                        sb.Finish();
                    }
                    if (type.Name == "JunkbotActor")
                    {
                        IActor junkbot = Scene.MobileActors[0];
                        ActorAnimationFrame frame = junkbot.Animation.GetCurrentFrame();
                        int locX;
                        int locY;
                        if (junkbot.Location.X != 0)
                        {
                            if (junkbot.Location.X == 1)
                            {
                                locX = 32;
                            }
                            else
                            {
                                locX = ((junkbot.Location.X - 2) * 15);

                            }
                        }
                        else
                        {
                            locX = junkbot.Location.X;
                        }

                        if (junkbot.Location.Y != 0)
                        {
                            locY = (junkbot.Location.Y * 18) + 10;
                        }
                        else
                        {
                            locY = junkbot.Location.Y + 10;
                        }
                        int sizX = ((junkbot.GridSize.Width - 1) * 15) + 26;
                        int sizY = ((junkbot.GridSize.Height - 1) * 18) + 32;
                        sb.Draw(
                                 "minifig_walk_l_1",
                                 new Rectangle(
                                     new Point(locX, locY), new Size(sizX, sizY)
                                     )
                                 );
                        sb.Finish();
                    }
                   

                }
            }
            /* public override void RenderFrame(IGraphicsController graphics)
             {
                 var sb = graphics.CreateSpriteBatch("menu-atlas");

                 graphics.ClearViewport(Color.CornflowerBlue);

                 sb.Draw(
                     "neo_title",
                     new Rectangle(
                         Point.Empty,
                         graphics.TargetResolution
                         )
                     );

                 sb.Finish();
             }*/
        }
    }
}
