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
using System.Collections.Generic;

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
        public BrickMover BrickMover;
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
            BrickMover = new BrickMover(Scene);
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

        private void BindBrick(BrickActor brick, Point mousePos)
        {
            if (brick != null)
            {
                BrickMover.selectedBrick = brick;
                Scene.IgnoredBricks.Add(brick);
                var tempConnected = BrickMover.IsBrickConnected(brick);
                bool isConnected = tempConnected.Count > 1 ? true : false;

                if (isConnected)
                {

                    foreach (BrickActor brickToAdd in tempConnected)
                    {
                        if (Scene.ConnectedBricks.Contains(brickToAdd))
                        {
                            if (brickToAdd.Size == BrickSize.Two)
                            {

                            }
                        }
                        Scene.ConnectedBricks.Add(brickToAdd);
                    }

                    foreach (BrickActor connectedBrick in Scene.ConnectedBricks)
                    {
                        if (connectedBrick != brick)
                        {
                            connectedBrick.Selected = true;
                            Scene.MoveBrickFromPlayfield(connectedBrick);
                        }
                    }
                }
                brick.Selected = true;
                Scene.MoveBrickFromPlayfield(brick);
                Scene.IgnoredBricks.Clear();
            }
        }


        private void UnbindBrick()
        {
            foreach (BrickActor connectedBrick in Scene.ConnectedBricks)
            {
                connectedBrick.Selected = false;
                connectedBrick.CanMove = true;
                Scene.MoveBrickFromPlayfield(connectedBrick);
                connectedBrick.Location = connectedBrick.MovingLocation;
            }
            Scene.ConnectedBricks.Clear();
        }

        
        public override void ProcessInputs(InputEvents inputs)
        {
            var MousePosition = inputs.MousePosition;
            Point MousePoint = new Point((int)Math.Floor(MousePosition.X - 5), (int)Math.Floor(MousePosition.Y - 10));
            Point MousePosAsCell = MousePoint.Reduce(Scene.LevelData.Spacing);
            BrickActor selectedBrick = BrickMover.selectedBrick;
            if ((MousePosAsCell.X >= 0 && MousePosAsCell.X < 35) && (MousePosAsCell.Y >= 0 && MousePosAsCell.Y <= 21))
            {

                IActor cell = Scene.GetPlayfield[MousePosAsCell.X, MousePosAsCell.Y];
                Console.WriteLine(Scene.GetPlayfield.GetLength(0).ToString() + Scene.GetPlayfield.GetLength(1).ToString() + MousePosAsCell);

                if (selectedBrick != null)
                {
                    BrickMover.UpdateSelectedBrickLocation(MousePosAsCell);

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
                                bool placeBrick;
                                do
                                {
                                    if (Scene.ConnectedBricks.Count > 0)
                                    {
                                        placeBrick = BrickMover.PlaceBrick(Scene.ConnectedBricks);
                                    }
                                    else
                                    {
                                        System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(new Point(selectedBrick.MovingLocation.X + currentCell, selectedBrick.MovingLocation.Y + 1), new Size(1, 1));
                                        placeBrick = Scene.CheckGridRegionFree(checkBounds);
                                    }
                                    if (placeBrick)
                                    {
                                        BrickMover.UpdateSelectedBrickLocation(MousePosAsCell);
                                        UnbindBrick();
                                        BrickMover.selectedBrick = null;
                                        Scene scene = Scene;
                                        break;
                                    }

                                    //check if brick can be placed - refactor to cover connected bricks
                                    currentCell += 1;
                                }
                                while (currentCell <= selectedBrick.GridSize.Width);
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
                            //bool canBePicked = (cell as BrickActor).CanBePicked();
                            if (keyPress == "mb.left" && (cell as BrickActor).Color.Name != "Gray")
                            {
                                selectedBrick = cell as BrickActor;
                                BindBrick(selectedBrick, MousePosAsCell);
                                break;
                            }
                            else
                            {

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
            foreach (IActor actor in Scene.SelectedGrid)
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
                    BrickActor selectedBrick = Scene.SelectedGrid[x, y] as BrickActor;
                    if (actor != null)
                    {
                        if (!actor.Rendered)
                        {
                            {
                                int locX, locY, sizY, sizX;                                
                                if (actor is BrickActor)
                                {
                                    var brick = actor as BrickActor;
                                    var currentFrame = brick.Animation.GetCurrentFrame();
                                    int xMod = brick.Location.X;
                                    int yMod = brick.Location.Y;
                                    if (brick.BoundingBoxes.Count <= 1)
                                    {
                                        if (brick.MovingLocation != brick.Location)
                                        {
                                            xMod = brick.MovingLocation.X;
                                            yMod = brick.MovingLocation.Y;
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
                    if (selectedBrick != null)
                    {
                        if (!selectedBrick.Rendered)
                        {
                            ActorAnimationFrame currentFrame = selectedBrick.Animation.GetCurrentFrame();
                            int locX;
                            int locY;
                            int sizY;
                            int sizX;
                            int xMod = selectedBrick.MovingLocation.X;
                            int yMod = selectedBrick.MovingLocation.Y;
                            if (selectedBrick.BoundingBoxes.Count <= 1)
                            {
                                if (selectedBrick.Location.X != 0)
                                {
                                    locX = (xMod * 15);
                                }
                                else
                                {
                                    locX = xMod;
                                }

                                if (selectedBrick.Location.Y != 0)
                                {
                                    locY = (yMod * 18) + 10;
                                }
                                else
                                {
                                    locY = yMod + 10;
                                }

                                if (selectedBrick.GridSize.Height != 1)
                                {
                                    sizY = (selectedBrick.GridSize.Height - 1) * 18 + 14;
                                }
                                else
                                {
                                    sizY = selectedBrick.GridSize.Height * 32;
                                }
                                sizX = (selectedBrick.GridSize.Width - 1) * 15 + 26;
                                actors.Draw(
                                 currentFrame.SpriteName,
                                 new Rectangle(
                                     new Point(locX, locY), new Size(sizX, sizY)
                                     )
                                 );
                                selectedBrick.Rendered = true;
                            }
                        }
                    }

                    if (actor != null || selectedBrick != null)
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

