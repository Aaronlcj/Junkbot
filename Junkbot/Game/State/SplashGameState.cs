using Junkbot.Game.World.Actors;
using Junkbot.Game.World.Actors.Animation;
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
    internal class SplashGameState : GameState
    {
        public override InputFocalMode FocalMode
        {
            get { return InputFocalMode.Always; }
        }
        public static string[] lvl;
        public Scene Scene;
        public static AnimationStore Store = new AnimationStore();

        public override string Name
        {
            get { return "SplashScreen"; }
        }
        
        public SplashGameState(string level)
        {
            lvl = File.ReadAllLines(Environment.CurrentDirectory + $@"\Content\Levels\{level}.txt");
            Scene = Scene.FromLevel(lvl, Store);
        }
        public override void RenderFrame(IGraphicsController graphics)
        {
            Scene.UpdateActors();

            var sb = graphics.CreateSpriteBatch("menu-atlas");
            var actors = graphics.CreateSpriteBatch("actors-atlas");
            graphics.ClearViewport(Color.CornflowerBlue);

            // Render gray bricks
            foreach (BrickActor item in Scene.ImmobileBricks)
            {
                if (item != null)
                {
/*                {&& item.Color.Name == "Gray"
*/                    ActorAnimationFrame currentFrame = item.Animation.GetCurrentFrame();
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
                            locY = (item.Location.Y * 18) + 10;
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
                        if (item == Scene.GetPlayfield[8, 9])
                        {
                        }
                        actors.Draw(
                         currentFrame.SpriteName,
                         new Rectangle(
                             new Point(locX, locY), new Size(sizX, sizY)
                             )
                         );
                        actors.Finish();
                    }
                }
            }
            // Render Junkbot
            if (Scene.MobileActors != null)
            {
                foreach (IActor actor in Scene.MobileActors)
                {
                    Type type = actor.GetType();
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
                        if (junkbot.Animation.IsPlaying())
                        {
                            if (junkbot.Location.X == 35)
                            {
                                Console.WriteLine("33 lul");
                            }
                            actors.Draw(
                                junkbot.Animation.GetCurrentFrame().SpriteName,
                                new Rectangle(
                                    new Point((junkbot.Location.X * 15), locY), new Size(sizX, sizY)
                                    )
                                );
                        }
                        else
                        {
                            actors.Draw(
                                     "minifig_walk_l_1",
                                     new Rectangle(
                                         new Point(locX, locY), new Size(sizX, sizY)
                                         )
                                     );
                        }
                        actors.Finish();

                    }


                }
            }

            //Render Overlay
            sb.Draw(
               "neo_title",
               new Rectangle(
                   Point.Empty,
                   graphics.TargetResolution
                   )
               );
            sb.Draw(
                "play_default",
                new Rectangle(
                    new Point(186, 146),
                    new Size(116, 45)
                    )
                );
            sb.Finish();


 
            /* //Render movable bricks
             foreach (BrickActor item in Scene.ImmobileBricks)
             {
                 if (item != null && item.Color.Name != "Gray")
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
                             locY = (item.Location.Y * 18) + 10;
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
                         if (item == Scene.GetPlayfield[8, 9])
                         {
                         }
                         actors.Draw(
                          currentFrame.SpriteName,
                          new Rectangle(
                              new Point(locX, locY), new Size(sizX, sizY)
                              )
                          );
                         actors.Finish();
                     }
                 }
             }
 */
        }
    }
}
