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
        private BrickActor brickToBind = null;
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
        
        private bool IsBrickConnected(BrickActor brick)
        {
            var brickPos = brick.Location;

            int x = brickPos.X;
            int y = brickPos.Y;
            int index = 0;

            do
            {
                IActor sceneCell = Scene.GetPlayfield[x + index, y - 1];
                if ((x + index >= 0 && y - 1 >= 0) && (x + index <= 34 && y - 1 <= 21))
                {
                    if (sceneCell != null)
                    {
                        Scene.ConnectedBricks.Add(sceneCell as BrickActor);
                        IsBrickConnected(sceneCell as BrickActor);
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    return false;
                }
            }
            while ((index) != brick.GridSize.Width);
            return true;
        }

        private void BindToMouse(BrickActor brick, Point mousePos)
        {
            if (brick != null)
            {
                bool isConnected = IsBrickConnected(brick);
                if (isConnected)
                {
                    Scene.ConnectedBricks.Add(brick);
                }
                else
                {
                    brick.IsBound = true;
                    brick.BoundLocation = new Point(mousePos.X, mousePos.Y);
                    Scene.MoveBrickFromPlayfield(brick);
                }
            }
        }


        private void UnbindBrick(BrickActor brick)
        {
            if (Scene.ConnectedBricks.Count == 0)
            {
                brick.IsBound = false;
                Scene.MoveBrickFromPlayfield(brick);
                brick.Location = brick.BoundLocation;
            }
            else
            {
                foreach (BrickActor connectedBrick in Scene.ConnectedBricks)
                {
                    connectedBrick.IsBound = false;
                    Scene.MoveBrickFromPlayfield(connectedBrick);
                    connectedBrick.Location = connectedBrick.BoundLocation;
                }
                Scene.ConnectedBricks.Clear();
            }
        }

        private void UpdateBoundBrickLocation(BrickActor brick, Point mousePos)
        {
            var connectedBricks = Scene.ConnectedBricks;
            if (connectedBricks.Count > 0)
            {
                if (connectedBricks.Count != 1)
                {
                    foreach (BrickActor connectedBrick in connectedBricks)
                    {
                        connectedBrick.IsBound = true;
                        connectedBrick.BoundLocation = new Point(mousePos.X - (brick.Location.X - connectedBrick.Location.X), mousePos.Y - (brick.Location.Y - connectedBrick.Location.Y));
                        Scene.MoveBrickFromPlayfield(connectedBrick);
                    }
                }
                else
                {
                    brick.IsBound = true;
                    brick.BoundLocation = new Point(mousePos.X, mousePos.Y);
                    Scene.MoveBrickFromPlayfield(brick);
                }
            }
        }
        public override void ProcessInputs(InputEvents inputs)
        {
            if (Scene.GetPlayfield[10,17] == null && Scene.BrickGrid[19,17] == null && Scene.GetPlayfield[19, 17] == null)
            {
                Console.WriteLine("gtet");
            }
            var MousePosition = inputs.MousePosition;
            var MousePress = inputs.NewPresses;
            var MouseRelease = inputs.NewReleases;

            Point MousePoint = new Point((int)Math.Floor(MousePosition.X - 5), (int)Math.Floor(MousePosition.Y - 10));
            Point MousePosAsCell = MousePoint.Reduce(Scene.LevelData.Spacing);

            if ((MousePosAsCell.X >= 0 && MousePosAsCell.X < 35) && (MousePosAsCell.Y >= 0 && MousePosAsCell.Y <= 21))
            {

                IActor cell = Scene.GetPlayfield[MousePosAsCell.X, MousePosAsCell.Y];
                Console.WriteLine(Scene.GetPlayfield.GetLength(0).ToString() + Scene.GetPlayfield.GetLength(1).ToString() + MousePosAsCell);

                if (brickToBind != null)
                {
                    UpdateBoundBrickLocation(brickToBind, MousePosAsCell);

/*                    bool canBePlaced = brickToBind.CanBePlaced();

                    if (canBePlaced)
                    {
                        Console.WriteLine("PLACE");
                    }
                    else
                    {
                        Console.WriteLine("BLOCK");
                    }*/
                    if (cell == null)
                    {
                        foreach (string keyPress in inputs.NewPresses)
                        {
                            if (keyPress == "mb.left")
                            {
                                int currentCell = 0;
                                do
                                {
                                    System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(new Point(brickToBind.BoundLocation.X + currentCell, brickToBind.BoundLocation.Y + 1), new Size(1, 1));
                                    bool isFree = Scene.CheckGridRegionFree(checkBounds);
                                    if (!isFree)
                                    {
                                        UnbindBrick(brickToBind);
                                        brickToBind = null;

                                        //inputs.ReportRelease("mb.left");
                                        Scene scene = Scene;
                                        break;
                                    }
                                    currentCell += 1;
                                }
                                while (currentCell <= brickToBind.GridSize.Width);
                            }
                        }
                    }
                }
                else
                {
                    if (cell != null)
                    {
                        foreach (string keyPress in inputs.NewPresses)
                        {
                            bool canBePicked = (cell as BrickActor).CanBePicked();

                            if (keyPress == "mb.left")
                            {
                                brickToBind = cell as BrickActor;

                                BindToMouse(brickToBind, MousePosAsCell);

                                break;
                            }
                        }
                    }
                }
                
            }
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
            foreach (IActor actor in Scene.BrickGrid)
            {
                if (actor != null)
                {
                    actor.Rendered = false;
                }
            }
            var sb = graphics.CreateSpriteBatch("menu-atlas");
            var actors = graphics.CreateSpriteBatch("actors-atlas");
            graphics.ClearViewport(Color.CornflowerBlue);
            Point BoundLocation = new Point(9999, 9999);

            // Render order
            int x;
            int y = 21;
            do
            {
                x = 0;
                do
                {
                    IActor actor = Scene.GetPlayfield[x, y];
                    BrickActor movingBrick = Scene.BrickGrid[x, y] as BrickActor;


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
                            }
                        }
                    }
                    if (movingBrick != null)
                    {
                        if (!movingBrick.Rendered)
                        {
                            ActorAnimationFrame currentFrame = movingBrick.Animation.GetCurrentFrame();
                            int locX;
                            int locY;
                            int sizY;
                            int sizX;
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

                                if (movingBrick.Location.Y != 0)
                                {
                                    locY = (yMod * 18) + 10;
                                }
                                else
                                {
                                    locY = yMod + 10;
                                }

                                if (movingBrick.GridSize.Height != 1)
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
                                movingBrick.Rendered = true;
                            }
                        }
                    }

                    if (actor != null || movingBrick != null)
                    {
                        actors.Finish();
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

