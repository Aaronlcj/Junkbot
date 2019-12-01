using Junkbot.Game.World.Actors;
using Junkbot.Game.World.Actors.Animation;
using Junkbot.Game.World.Level;
using Junkbot.Helpers;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Input;
using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.State
{
    /// <summary>
    /// Represents the main menu game state.
    /// </summary>
    internal class LevelState : GameState
    {
        System.Timers.Timer _timer;

        public override InputFocalMode FocalMode
        {
            get { return InputFocalMode.Always; }
        }
        public static string[] lvl; 
        public Scene Scene;
        //public JunkbotSidebar Sidebar;
        public ISpriteBatch _actors;
        public ISpriteBatch Junkbot;
        public static AnimationStore Store = new AnimationStore();
        private UxShell Shell { get; set; }

        public LevelState(string level)
        {
            Shell = new UxShell();
            lvl = File.ReadAllLines(Environment.CurrentDirectory + $@"\Content\Levels\{level}.txt");
            Scene = Scene.FromLevel(lvl, Store);
            SetTimer();
            //Sidebar = new JunkbotSidebar(Scene.LevelData);
        }
        public override string Name
        {
            get { return "DemoGame"; }
        }
        private void Timer_Tick(object sender, EventArgs e)
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
        private void ResetActorRenderStatus()
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
                    if (Scene.SelectedGrid[x, y] is BrickActor selectedBrick)
                    {
                        // check playfield
                        if (actor != null)
                        {
                            if (!actor.Rendered)
                            {
                                if (actor is BrickActor)
                                {
                                    DrawBrick(actor as BrickActor, testList);
                                }

                                if (actor is JunkbotActor)
                                {
                                    DrawJunkbot(actor as JunkbotActor);
                                }

                                if (actor is BinActor)
                                {
                                    DrawBin(actor as BinActor);
                                }
                            }

                            actor.Rendered = true;
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
            Junkbot.Draw(
                junkbot.Animation.GetCurrentFrame().SpriteName,
                new Rectangle(
                    frameOffset, junkbot.Animation.GetCurrentFrame().SpriteSize
                )
            );
        }

        private void DrawBin(BinActor binActor)
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
                locX = bin.Location.X + 4;
            }

            if (bin.Location.Y != 0)
            {
                locY = (bin.Location.Y) * 18 + 9;
            }
            else
            {
                locY = bin.Location.Y;
            }

            int sizX = 31;
            int sizY = 47;
            _actors.Draw(
                "bin",
                new Rectangle(
                    new Point(locX, locY), new Size(sizX, sizY)
                )
            );
        }
        public override void RenderFrame(IGraphicsController graphics)
        {
            ResetActorRenderStatus();

            _actors = graphics.CreateSpriteBatch("actors-atlas");
            Junkbot = graphics.CreateSpriteBatch("junkbot-animation-atlas");
            var background = graphics.CreateSpriteBatch("background-atlas");
            var decals = graphics.CreateSpriteBatch("decals-atlas");
            //var sidebar = graphics.CreateSpriteBatch("sidebar-atlas");
            //var buttons = graphics.CreateSpriteBatch("sidebar-buttons-atlas");

            graphics.ClearViewport(Color.CornflowerBlue);
            if (Scene.LevelData.Backdrop != null)
            {
                background.Draw(
                    Scene.LevelData.Backdrop,
                    new Rectangle(
                        new Point(-6, 0), new Size(536, 420)
                    )
                );
                background.Finish();
            }

            if (Scene.LevelData.Decals != null)
            {
                foreach (JunkbotDecalData decal in Scene.LevelData.Decals)
                {
                    Pencil.Gaming.MathUtils.Rectanglei decalMap = decals.GetSpriteUV(decal.Decal);
                    int locY = 0;
                    switch (decal.Decal)
                    {
                        case "door":
                            locY = 182;
                            break;
                        case "window":
                            locY -= 14;
                            break;
                        case "fusebox_pipes_l":
                            locY += 10;
                            break;
                        case "terminal_chart":
                            locY -= 28;
                            break;
                        case "sign_keepout":
                            locY -= 14;
                            break;
                    }

                    decals.Draw(
                        decal.Decal,
                        new Rectangle(
                            new Point(decal.Location.X - (decalMap.Width / 2),
                                decal.Location.Y - (decalMap.Height / 2) - locY),
                            new Size(decalMap.Width, decalMap.Height)
                        )
                    );
                    if (decal.Decal == "fusebox_pipes_l")
                    {
                        break;
                    }
                }

                decals.Finish();
            }
            ParseGridRenderOrder();
            _actors.Finish();
            Junkbot.Finish();
            /*foreach (IActor item in Scene.ImmobileBricks)
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
                    }
                }
            }



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
                            locY = (climb_bot.Location.Y + 1) * 18;
                        }
                        else
                        {
                            locY = climb_bot.Location.Y;
                        }

                        int sizX = ((climb_bot.GridSize.Width) * 15) + 5;
                        int sizY = (climb_bot.GridSize.Height * 18) + 2;
                        if (climb_bot.Animation.IsPlaying())
                        {
                            actors.Draw(
                                climb_bot.Animation.GetCurrentFrame().SpriteName,
                                new Rectangle(
                                    new Point(locX + 6, locY), new Size(sizX, sizY)
                                )
                            );
                        }
                        else
                        {
                            actors.Draw(
                                "climbbot_walk_l_1",
                                new Rectangle(
                                    new Point(locX + 6, locY), new Size(sizX, sizY)
                                )
                            );
                        }

                        actors.Finish();
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
                            locX = bin.Location.X + 4;
                        }

                        if (bin.Location.Y != 0)
                        {
                            locY = (bin.Location.Y) * 18 + 9;
                        }
                        else
                        {
                            locY = bin.Location.Y;
                        }

                        int sizX = 31;
                        int sizY = 47;
                        actors.Draw(
                            "bin",
                            new Rectangle(
                                new Point(locX, locY), new Size(sizX, sizY)
                            )
                        );
                        actors.Finish();
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
                    }

                    actors.Finish();
                }
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
                    /*Console.WriteLine(Scene.GetPlayfield.GetLength(0).ToString() +
                                      Scene.GetPlayfield.GetLength(1).ToString() + MousePosAsCell);
*/

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
            }
        }
        private void BindBrick(BrickActor brick, Point mousePos)
        {
            if (brick != null)
            {
                BrickMover.SelectedBrick = brick;
                Scene.IgnoredBricks.Add(brick);
                var tempConnected = BrickMover.IsBrickConnected(brick);
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
    }
}
//Sidebar.Render(graphics);



/*public override void RenderFrame(IGraphicsController graphics)
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

