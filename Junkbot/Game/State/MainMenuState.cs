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
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Junkbot.Game.Logic;
using Junkbot.Game.World.Level;
using Microsoft.Win32.SafeHandles;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.State
{
    /// <summary>
    /// Represents the main menu game state.
    /// </summary>
    internal class MainMenuState : GameState
    {
        System.Timers.Timer _timer;

        private JunkbotGame JunkbotGame { get; set; }
        public override InputFocalMode FocalMode
        {
            get { return InputFocalMode.Always; }
        }
        public static string[] lvl;
        public Scene Scene;
        public ISpriteBatch _actors;
        private ISpriteBatch Junkbot;
        public Intro Intro;
        private UxShell Shell { get; set; }
        public MainMenuButtons Buttons;
        public MainMenuBackground MainMenuBackground;
        public static AnimationStore Store;
        public bool IntroPlayed;

        public override string Name
        {
            get { return "MainMenu"; }
        }

        public MainMenuState(string level, JunkbotGame junkbotGame, bool introPlayed = true)
        {
            JunkbotGame = junkbotGame;
            lvl = File.ReadAllLines(Environment.CurrentDirectory + $@"\Content\Levels\{level}.txt");
            Store = new AnimationStore();
            Scene = Scene.FromLevel(lvl, Store);
            SetTimer();
            Shell = new UxShell();
            //Intro = new Intro(Shell, JunkbotGame);
            IntroPlayed = introPlayed;
            Buttons = new MainMenuButtons(Shell, JunkbotGame, this);
            MainMenuBackground = new MainMenuBackground();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Scene != null)
            {
                Scene.UpdateActors();
            }
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
                BrickMover.SelectedBrick = brick;
                Scene.IgnoredBricks.Add(brick);
                var tempConnected = Scene.BrickMover.IsBrickConnected(brick);
                bool isConnected = tempConnected.Count > 1 ? true : false;

                if (isConnected)
                { 
                    foreach (BrickActor brickToAdd in tempConnected)
                    {
                        if (brickToAdd.CanMove)
                        {
                            if (!Scene.ConnectedBricks.Contains(brickToAdd))
                            {
                                Scene.ConnectedBricks.Add(brickToAdd);
                            }

                        }

                        brickToAdd.CanMove = true;
                    }

                    foreach (BrickActor connectedBrick in Scene.ConnectedBricks)
                    {
                        connectedBrick.Selected = true;
                        BrickMover.MoveBrickFromPlayfield(connectedBrick);
                    }
                }
                else
                {
                    brick.Selected = true;
                    Scene.ConnectedBricks.Add(brick);
                    BrickMover.MoveBrickFromPlayfield(brick);
                }
                
                Scene.IgnoredBricks.Clear();
            }
        }

        private void UnbindBrick()
        {
            foreach (BrickActor connectedBrick in Scene.ConnectedBricks)
            {
                connectedBrick.Selected = false;
                connectedBrick.CanMove = true;
                BrickMover.MoveBrickFromPlayfield(connectedBrick);
                connectedBrick.Location = connectedBrick.MovingLocation;
            }
            Scene.ConnectedBricks.Clear();
            Scene.IgnoredBricks.Clear();

        }

        private void ResetRenderStatus()
        {
            foreach (var actor in Scene.GetPlayfield)
            {
                if (actor != null)
                {
                    actor.Rendered = false;
                }
            }
            foreach (var actor in Scene.SelectedGrid)
            {
                if (actor != null)
                {
                    actor.Rendered = false;
                }
            }
        }

        private void ParseGridRenderOrder()
        {
            var testList = new List<BrickActor>();
            int y = 21;
            do
            {
                int x = 0;
                do
                {

                    IActor actor = Scene.GetPlayfield[x, y];
                   
                    BrickActor selectedBrick = Scene.SelectedGrid[x, y] as BrickActor;
                    // check playfield
                    if (actor != null)
                    {
                        if (!actor.Rendered)
                        {
                            if (actor is BrickActor)
                            {
                                DrawBrick(actor as BrickActor, testList);
                            }

                            // draw junkbot
                            if (actor is JunkbotActor)
                            {
                                IActor junkbot = Scene.MobileActors[0];
                                DrawJunkbot(junkbot as JunkbotActor);
                            }

                            actor.Rendered = true;
                        }
                    }
                    // check moving bricks
                    if (selectedBrick != null)
                    {
                        if (!selectedBrick.Rendered)
                        {
                            DrawBrick(selectedBrick, testList);
                        }

                        selectedBrick.Rendered = true;
                    }
                    x += 1;
                }
                while (x != 34);
                y -= 1;
            }
            while (y != 0);
        }

        private void DrawBrick(BrickActor brick, List<BrickActor> testList)
        {
                int locX, locY, sizY, sizX;
                var currentFrame = brick.Animation.GetCurrentFrame();
                int xMod = brick.Location.X;
                int yMod = brick.Location.Y;
                if (brick.Location != brick.MovingLocation)
                {

                }
                if (brick.BoundingBoxes.Count <= 1)
                {
                    if (Scene.ConnectedBricks.Contains(brick))
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

                    if (brick.Location.Y != 0)
                    {
                        locY = (yMod * 18) + 10;
                    }
                    else
                    {
                        locY = yMod + 10;
                    }

                    if (brick.GridSize.Height != 1)
                    {
                        sizY = (brick.GridSize.Height - 1) * 18 + 14;
                    }
                    else
                    {
                        sizY = brick.GridSize.Height * 32;
                    }
                    sizX = (brick.GridSize.Width - 1) * 15 + 26;
                    _actors.Draw(
                     currentFrame.SpriteName,
                     new Rectangle(
                         new Point(locX, locY), new Size(sizX, sizY)
                         )
                     );
                }
                testList.Add(brick);
        }

        private void DrawJunkbot(JunkbotActor junkbot)
        {
            int locX, locY, sizY, sizX;
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
            _actors.Draw(
                junkbot.Animation.GetCurrentFrame().SpriteName,
                new Rectangle(
                    frameOffset, junkbot.Animation.GetCurrentFrame().SpriteSize
                )
            );
        }

        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public override void Dispose()
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
                Scene.Dispose();
                //Scene = null;
                
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
        ~MainMenuState()
        {
            Dispose(false);
            System.Diagnostics.Trace.WriteLine("MainMenu's destructor is called.");
        }
        public override void RenderFrame(IGraphicsController graphics)
        {
            ResetRenderStatus();

            //var mainMenu = graphics.CreateSpriteBatch("main-menu-atlas");
            _actors = graphics.CreateSpriteBatch("level-atlas");
            //Junkbot = graphics.CreateSpriteBatch("junkbot-animation-atlas");
            MainMenuBackground.RenderFrame(graphics);

            graphics.ClearViewport(Color.CornflowerBlue);

            // Render order
            ParseGridRenderOrder();
            _actors.Finish();

            MainMenuBackground.Render(graphics);
            Buttons.Render(graphics);
            
            /*if (Intro.Gif == null && IntroPlayed == false)
            {
                Intro.LoadIntro(graphics);
            }
            if (Intro.Gif != null && IntroPlayed == false)
            {
                Intro.Render(graphics);
            }*/
        }

        public override void Update(TimeSpan deltaTime, InputEvents inputs)
        {

            if (inputs != null)
            {
                Shell.HandleMouseInputs(inputs);

                var MousePosition = inputs.MousePosition;
                Point MousePoint = new Point((int)Math.Floor(MousePosition.X - 5),
                    (int)Math.Floor(MousePosition.Y - 10));
                Point MousePosAsCell = MousePoint.Reduce(Scene.LevelData.Spacing);
                BrickActor selectedBrick = BrickMover.SelectedBrick;
                if ((MousePosAsCell.X >= 0 && MousePosAsCell.X < 35) &&
                    (MousePosAsCell.Y >= 0 && MousePosAsCell.Y <= 21))
                {

                    IActor cell = Scene.GetPlayfield[MousePosAsCell.X, MousePosAsCell.Y];
                    if (selectedBrick != null)
                    {
                        BrickMover.UpdateSelectedBrickLocation(MousePosAsCell);
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
                                            var checkBounds = new System.Drawing.Rectangle(
                                                new Point(selectedBrick.MovingLocation.X + currentCell,
                                                    selectedBrick.MovingLocation.Y + 1), new Size(1, 1));
                                            placeBrick = Scene.CheckGridRegionFree(checkBounds);
                                        }

                                        if (placeBrick)
                                        {
                                            BrickMover.UpdateSelectedBrickLocation(MousePosAsCell);
                                            UnbindBrick();
                                            BrickMover.SelectedBrick = null;
                                            break;
                                        }

                                        currentCell += 1;
                                    } while (currentCell <= selectedBrick.GridSize.Width);
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
                                if (keyPress == "mb.left" && (cell as BrickActor).Color.Name != "Gray")
                                {
                                    selectedBrick = cell as BrickActor;
                                    BindBrick(selectedBrick, MousePosAsCell);
                                    break;
                                }
                            }
                        }
                    }
                }

                /*Console.WriteLine(Scene.GetPlayfield.GetLength(0).ToString() +
                                  Scene.GetPlayfield.GetLength(1).ToString() + MousePosAsCell);*/
            }
        }
        
    }
}

