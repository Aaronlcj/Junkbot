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
        private IActor brickToBind = null;
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
        
        private void BindToMouse(BrickActor brick, Point mousePos)
        {
            if (brick != null)
            {
                Scene.BrickGrid[brick.Location.X, brick.Location.Y] = brick;

                brick.BoundLocation = new Point(mousePos.X, mousePos.Y);
/*                brick.Location = new Point(35, 21);
*/
                brick.IsBound = true;
            }
        }
        public override void ProcessInputs(InputEvents inputs)
        {
            var MousePosition = inputs.MousePosition;
            var MousePress = inputs.NewPresses;
            var MouseRelease = inputs.NewReleases;
/*            if (MousePress.Count != 0 || MouseRelease.Count != 0)
            {
                Console.WriteLine();
            }*/
            Point MousePoint = new Point((int)Math.Floor(MousePosition.X), (int)Math.Floor(MousePosition.Y));
            Point MousePosAsCell = MousePoint.Reduce(Scene.LevelData.Spacing);

            if ((MousePosAsCell.X >= 0 && MousePosAsCell.X < 35) && (MousePosAsCell.Y >= 0 && MousePosAsCell.Y <= 21))
            {
                Console.WriteLine(Scene.GetPlayfield.GetLength(0).ToString() + Scene.GetPlayfield.GetLength(1).ToString() + MousePosAsCell);

                IActor cell = Scene.GetPlayfield[MousePosAsCell.X, MousePosAsCell.Y];

                if (cell != null && brickToBind == null)
                {
                    foreach (string activeInput in inputs.ActiveDownedInputs)
                    {
                        if (brickToBind == null)
                        {
                            if (activeInput == "mb.left")
                            {
                                brickToBind = cell;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                if (cell == null && brickToBind != null)
                {
                    foreach (string activeInput in inputs.ActiveDownedInputs)
                    {
                        if (activeInput == "mb.left")
                        {
                            brickToBind = null;
                        }
                    }
                }
            }
            BindToMouse(brickToBind as BrickActor, MousePosAsCell);
            // convert mouse x,y to cell coordinates, bind sprite to cursor on press, check region free, assign new location on mouse release
        }
        public override void RenderFrame(IGraphicsController graphics)
        {
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
            BrickActor movingBrick = null;
            Point BoundLocation = new Point(9999, 9999);

            foreach (BrickActor item in Scene.BrickGrid)
            {
                if (item != null)
                {
                    movingBrick = item;
                    break;
                }
            }

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
                                    BrickActor brick = actor as BrickActor;
                                    ActorAnimationFrame currentFrame = brick.Animation.GetCurrentFrame();

                                    int xMod = brick.Location.X;
                                    int yMod = brick.Location.Y;
                                    if (brick.BoundingBoxes.Count <= 1)
                                    {
                                        

                                        if (brick.BoundLocation != brick.Location)
                                        {
                                            xMod = brick.BoundLocation.X;
                                            yMod = brick.BoundLocation.Y;
                                        }


                                        if (brick.Location.X != 0)
                                        {
                                            locX = (xMod * 15);
                                        }
                                        else
                                        {
                                            locX = xMod;
                                        }

                                        if (actor.Location.Y != 0)
                                        {
                                            locY = (yMod * 18) + 10;
                                        }
                                        else
                                        {
                                            locY = yMod + 10;
                                        }

                                        if (actor.GridSize.Height != 1)
                                        {
                                            sizY = (brick.GridSize.Height - 1) * 18 + 14;
                                        }
                                        else
                                        {
                                            sizY = brick.GridSize.Height * 32;
                                        }
                                        sizX = (brick.GridSize.Width - 1) * 15 + 26;
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
                    if (movingBrick != null)
                    {
                        int locX;
                        int locY;
                        int sizY;
                        int sizX;
                        if (movingBrick.Location.X == x && movingBrick.Location.Y == y)
                        {
                            ActorAnimationFrame currentFrame = movingBrick.Animation.GetCurrentFrame();

                            int xMod = movingBrick.Location.X;
                            int yMod = movingBrick.Location.Y;
                            if (movingBrick.BoundingBoxes.Count <= 1)
                            {


                                if (movingBrick.BoundLocation != BoundLocation)
                                {
                                    xMod = movingBrick.BoundLocation.X;
                                    yMod = movingBrick.BoundLocation.Y;
                                }


                                if (movingBrick.Location.X != 0)
                                {
                                    locX = (xMod * 15);
                                }
                                else
                                {
                                    locX = xMod;
                                }

                                if (actor.Location.Y != 0)
                                {
                                    locY = (yMod * 18) + 10;
                                }
                                else
                                {
                                    locY = yMod + 10;
                                }

                                if (actor.GridSize.Height != 1)
                                {
                                    sizY = (movingBrick.GridSize.Height - 1) * 18 + 14;
                                }
                                else
                                {
                                    sizY = movingBrick.GridSize.Height * 32;
                                }
                                sizX = (movingBrick.GridSize.Width - 1) * 15 + 26;
                                actors.Draw(
                                 currentFrame.SpriteName,
                                 new Rectangle(
                                     new Point(locX, locY), new Size(sizX, sizY)
                                     )
                                 );
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

