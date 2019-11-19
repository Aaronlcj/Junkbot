using Junkbot.Helpers;
using Junkbot.Game.World.Actors;
using Junkbot.Game.World.Actors.Animation;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Input;
using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Drawing;
using System.IO;
using Pencil.Gaming;

namespace Junkbot.Game.State
{
    /// <summary>
    /// Represents the main menu game state.
    /// </summary>
    internal class SplashGameState : GameState
    {
        System.Timers.Timer _timer;

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
            SetTimer();
        }
        void Timer_Tick(object sender, EventArgs e)
        {
            Scene.UpdateActors();
        }
        public void SetTimer()
        {
            _timer = new System.Timers.Timer(25);
            _timer.Elapsed += Timer_Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public override void ProcessInputs(InputEvents inputs)
        {
            var mousePos = inputs.MousePosition;
            var mousePress = inputs.NewPresses;
            var mouseRelease = inputs.NewReleases;
            // convert mouse x,y to cell coordinates, bind sprite to cursor on press, check region free, assign new location on mouse release
        }
        public override void RenderFrame(IGraphicsController graphics)
        {
          if (Scene.GetPlayfield[16, 19] == null)
            {
                Console.WriteLine();
            }

            foreach (IActor actor in Scene.GetPlayfield)
            {
                if (actor != null)
                {
                    actor.Rendered = false;
                }
            }
            var sb = graphics.CreateSpriteBatch("menu-atlas");
            var actors = graphics.CreateSpriteBatch("actors-atlas");
            graphics.ClearViewport(Color.CornflowerBlue);

            // Render order
            int x;
            int y = 21;
            do
            {
                x = 0;
                do
                {
                    IActor actor = Scene.GetPlayfield[x, y];
                    if (actor != null)
                    {
                        if (actor.Rendered == false)
                        {
                            {
                                int locX;
                                int locY;
                                int sizY;
                                int sizX;
                                Type type = actor.GetType();
                                if (actor is BrickActor)
                                {
                                    ActorAnimationFrame currentFrame = actor.Animation.GetCurrentFrame();


                                    if (actor.BoundingBoxes.Count <= 1)
                                    {
                                        Rectangle testtt = actor.BoundingBoxes[0];
                                        Size testttt = actor.GridSize;
                                        if (actor.Location.X != 0)
                                        {
                                            locX = (actor.Location.X * 15);
                                        }
                                        else
                                        {
                                            locX = actor.Location.X;
                                        }

                                        if (actor.Location.Y != 0)
                                        {
                                            locY = (actor.Location.Y * 18) + 10;
                                        }
                                        else
                                        {
                                            locY = actor.Location.Y + 10;
                                        }

                                        if (actor.GridSize.Height != 1)
                                        {
                                            sizY = (actor.GridSize.Height - 1) * 18 + 14;
                                        }
                                        else
                                        {
                                            sizY = actor.GridSize.Height * 32;
                                        }
                                        sizX = (actor.GridSize.Width - 1) * 15 + 26;
                                        if (actor == Scene.GetPlayfield[8, 9])
                                        {
                                        }
                                        actors.Draw(
                                         currentFrame.SpriteName,
                                         new Rectangle(
                                             new Point(locX, locY), new Size(sizX, sizY)
                                             )
                                         );
                                    }
                                }
                                if (actor is JunkbotActor)
                                {
                                    IActor junkbot = Scene.MobileActors[0];
                                    ActorAnimationFrame frame = junkbot.Animation.GetCurrentFrame();
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
                                    Size size = Size.Add(junkbot.GridSize, Scene.LevelData.Spacing);
                                    sizX = ((junkbot.GridSize.Width - 1) * 15) + 26;
                                    sizY = ((junkbot.GridSize.Height - 1) * 18) + 32;
                                    Point frameOffset = new Point((junkbot.Location.X * 15), locY).Add(junkbot.Animation.GetCurrentFrame().Offset);
                                        actors.Draw(
                                            junkbot.Animation.GetCurrentFrame().SpriteName,
                                            new Rectangle(
                                                frameOffset, junkbot.Animation.GetCurrentFrame().SpriteSize
                                                )
                                            );
                                }
                                actor.Rendered = true;
                                actors.Finish();
                            }
                        }
                    }
                    x += 1;
                }
                while (x != 34);
                y -= 1;
            }
            while (y != 0);

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
        }
    }
}

