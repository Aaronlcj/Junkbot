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
        public static AnimationStore Store = new AnimationStore();
        private BrickActor selectedBrick = null;
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

        private void BindBrick(BrickActor brick, Point mousePos)
        {
            if (brick != null)
            {
                Scene.IgnoredBricks.Add(brick);
                var tempConnected = IsBrickConnected(brick);
                bool isConnected = tempConnected.Count > 0 ? true : false;

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
                Scene.MoveBrickFromPlayfield(connectedBrick);
                connectedBrick.Location = connectedBrick.MovingLocation;
            }
            Scene.ConnectedBricks.Clear();

        }

        private bool CanBrickMove(IList<BrickActor> bricks)
        {
            foreach (BrickActor brick in bricks)
            {
                if (brick.CanMove == true)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsRowAllGrey(IList<BrickActor> brickRow)
        {
            foreach (BrickActor brick in brickRow)
            {
                if (brick.Color.Name != "Gray")
                {
                    return false;
                }
            }
            return true;
        }
        private IList<BrickActor> ParseRow(IList<BrickActor> row, FacingDirection direction)
        {
            var ignoredBricks = Scene.IgnoredBricks;
            IList<BrickActor> tempConnected = new List<BrickActor>();
            IList<BrickActor> selected = new List<BrickActor>();

            if (direction == FacingDirection.Up)
            {
                foreach (BrickActor connectedBrick in row)
                {
                    var currentBelowRow = CheckSurroudingBricks(connectedBrick, FacingDirection.Down);
                    bool canBrickMove = CanBrickMove(currentBelowRow);
                    if (canBrickMove)
                    {
                        ignoredBricks.Add(connectedBrick);

                        if (connectedBrick.Color.Name != "Gray")
                        {
                            if (!tempConnected.Contains(connectedBrick))
                            {
                                tempConnected.Add(connectedBrick);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                foreach (BrickActor tempBrick in tempConnected)
                {
                    var newTemp = IsBrickConnected(tempBrick);
                    if (newTemp.Count > 0)
                    {
                        foreach (BrickActor temp in newTemp)
                        {
                            selected.Add(temp);
                        }
                    }
                    else
                    {
                        tempBrick.CanMove = false;
                    }
                }
            }
            if (direction == FacingDirection.Down)
            {


                foreach (BrickActor connectedBrick in row)
                {
                    var currentBelowRow = CheckSurroudingBricks(connectedBrick, FacingDirection.Down);
                    bool canBrickMove = CanBrickMove(currentBelowRow);
                    if (canBrickMove)
                    {
                        ignoredBricks.Add(connectedBrick);

                        if (connectedBrick.Color.Name != "Gray")
                        {
                            if (!tempConnected.Contains(connectedBrick))
                            {
                                tempConnected.Add(connectedBrick);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                foreach (BrickActor tempBrick in tempConnected)
                {
                    var newTemp = IsBrickConnected(tempBrick);
                    if (newTemp.Count > 0)
                    {
                        foreach (BrickActor temp in newTemp)
                        {
                            selected.Add(temp);
                        }
                    }
                    else
                    {
                        tempBrick.CanMove = false;
                    }
                }
            }
            return selected;
        }
        private IList<BrickActor> IsBrickConnected(BrickActor brick)
        {
            if (brick.Location.X == 28)
            {

            }
            var currentAboveRow = CheckSurroudingBricks(brick, FacingDirection.Up);
            var currentBelowRow = CheckSurroudingBricks(brick, FacingDirection.Down);
            var ignoredBricks = Scene.IgnoredBricks;
            int aboveCount = currentAboveRow.Count;
            int belowCount = currentBelowRow.Count;
            bool isRowGrey = IsRowAllGrey(currentBelowRow);

            var connectedBricks = Scene.ConnectedBricks;
            IList<BrickActor> tempConnected = new List<BrickActor>();
            if (!isRowGrey || brick == selectedBrick)
            {
                if (aboveCount > 0)
                {
                    int i = 0;
                    do
                    {
                        foreach (BrickActor ignoredBrick in ignoredBricks)
                        {
                            if (currentAboveRow.Contains(ignoredBrick))
                            {
                                currentAboveRow.Remove(ignoredBrick);
                            }
                        }
                        i++;
                    }
                    while (i != aboveCount);
                }
                if (belowCount > 0)
                {
                    int i = 0;
                    do
                    {
                        foreach (BrickActor ignoredBrick in ignoredBricks)
                        {
                            if (currentBelowRow.Contains(ignoredBrick))
                            {
                                currentBelowRow.Remove(ignoredBrick);
                            }
                        }
                        i++;
                    }
                    while (i != belowCount);
                }
                // check if connected above
                if (currentAboveRow.Count > 0)
                {
                    var newTempList = ParseRow(currentAboveRow, FacingDirection.Up);
                    foreach (BrickActor temp in newTempList)
                    {
                        tempConnected.Add(temp);
                    }
                }

                // check if connected below if nothing above
                if (currentBelowRow.Count > 0)
                {
                    var newTempList = ParseRow(currentBelowRow, FacingDirection.Down);
                    foreach (BrickActor temp in newTempList)
                    {
                        tempConnected.Add(temp);
                    }
                }
                bool aboveStatus = ((currentAboveRow.Count == 0 || currentAboveRow.Contains(brick)) && !IsRowAllGrey(currentAboveRow)) ? true : false;
                bool belowStatus = ((currentBelowRow.Count == 0 || currentBelowRow.Contains(brick)) && !IsRowAllGrey(currentBelowRow)) ? true : false;

                /*            if ((aboveStatus && !belowStatus) || (belowStatus && !aboveStatus) || (!tempConnected.Contains(selectedBrick) && brick == selectedBrick) || (currentAboveRow.Count == 0 && currentBelowRow.Count == 0))
                            {

                                tempConnected.Add(brick);
                            }*/
                if (tempConnected.Count > 0 || (currentAboveRow.Count == 0 && currentBelowRow.Count == 0) || (aboveCount > 0 && belowCount == 0) || (belowCount > 0 && aboveCount == 0))
                {
                    tempConnected.Add(brick);

                }
            }
            else
            {
                brick.CanMove = false;
            }
            return tempConnected;
        }

        private IList<BrickActor> CheckSurroudingBricks(BrickActor brick, FacingDirection direction)
        {
            if (brick.Location.X == 13 && brick.Location.Y == 17)
            { 
            }
            var brickPos = brick.Location;
            int x = brickPos.X;
            int y = brickPos.Y;
            int currentCellX = 0;
            int dx = direction == FacingDirection.Up ? -1 : 1;
            IList<BrickActor> currentRow = new List<BrickActor>();
            // check if top of brick is connected
            do
            {
                if ((x + currentCellX >= 0 && y + dx >= 0) && (x + currentCellX <= 34 && y + dx <= 21))
                {
                    IActor cell = Scene.GetPlayfield[x + currentCellX, y + dx];
                    if (cell != null && !currentRow.Contains(cell as BrickActor) && !Scene.ConnectedBricks.Contains(cell as BrickActor))
                    {
                        BrickActor cellA = cell as BrickActor;
                        /*if (cellA.Color.Name != "Gray")
                        {*/
                            currentRow.Add(cell as BrickActor);
                        //}
                    }
                
                    currentCellX++;
                }
            }
            while ((currentCellX) != brick.GridSize.Width);
            return currentRow;
        }
    
        

        private void UpdateSelectedBrickLocation(BrickActor brick, Point mousePos)
        {
            var connectedBricks = Scene.ConnectedBricks;
            if (connectedBricks.Count > 0)
            {
                foreach (BrickActor connectedBrick in connectedBricks)
                {
                    connectedBrick.MovingLocation = new Point(mousePos.X - (brick.Location.X - connectedBrick.Location.X), mousePos.Y - (brick.Location.Y - connectedBrick.Location.Y));
                    
                }
            }
                brick.MovingLocation = new Point(mousePos.X, mousePos.Y);

        }

        public bool PlaceBrick(IList<BrickActor> connectedBricks)
        {
            List<Point> connectedCells = new List<Point>();
            bool blocked = false;
            bool connectAbove = false;
            bool connectBelow = false;

            foreach (BrickActor brick in connectedBricks)
            {
                var brickPos = brick.MovingLocation;
                int x = brickPos.X;
                int y = brickPos.Y;
                int index = 0;

                if ((x + index >= 0 && y - 1 >= 0) && (x + index <= 34 && y - 1 <= 21))
                {
                    do
                    {
                        connectedCells.Add(new Point(brick.MovingLocation.X + index, brick.MovingLocation.Y));
                        index++;
                    }
                    while (index != brick.GridSize.Width);
                }
            }

            foreach (Point cell in connectedCells)
            {
                blocked = (Scene.GetPlayfield[cell.X, cell.Y] != null) ? true : false;
                if (Scene.GetPlayfield[cell.X, cell.Y - 1] != null)
                {
                    if (!connectedCells.Contains(new Point(cell.X, cell.Y - 1)))
                    {
                        connectAbove = true;
                    }
                }
                if (Scene.GetPlayfield[cell.X, cell.Y + 1] != null)
                {
                    if (!connectedCells.Contains(new Point(cell.X, cell.Y + 1)))
                    {
                        connectBelow = true;
                    }
                }
            }
            if (connectAbove && connectBelow)
            { 
                return false; 
            }

            if (!blocked && ((connectAbove && !connectBelow) || (connectBelow && !connectAbove)))
            {
                return true;
            }
            return false;
        } 
        public override void ProcessInputs(InputEvents inputs)
        {
            var MousePosition = inputs.MousePosition;
            Point MousePoint = new Point((int)Math.Floor(MousePosition.X - 5), (int)Math.Floor(MousePosition.Y - 10));
            Point MousePosAsCell = MousePoint.Reduce(Scene.LevelData.Spacing);

            if ((MousePosAsCell.X >= 0 && MousePosAsCell.X < 35) && (MousePosAsCell.Y >= 0 && MousePosAsCell.Y <= 21))
            {

                IActor cell = Scene.GetPlayfield[MousePosAsCell.X, MousePosAsCell.Y];
                Console.WriteLine(Scene.GetPlayfield.GetLength(0).ToString() + Scene.GetPlayfield.GetLength(1).ToString() + MousePosAsCell);

                if (selectedBrick != null)
                {
                    UpdateSelectedBrickLocation(selectedBrick, MousePosAsCell);

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
                                        placeBrick = PlaceBrick(Scene.ConnectedBricks);
                                    }
                                    else
                                    {
                                        System.Drawing.Rectangle checkBounds = new System.Drawing.Rectangle(new Point(selectedBrick.MovingLocation.X + currentCell, selectedBrick.MovingLocation.Y + 1), new Size(1, 1));
                                        placeBrick = Scene.CheckGridRegionFree(checkBounds);
                                    }
                                    if (placeBrick)
                                    {
                                        UpdateSelectedBrickLocation(selectedBrick, MousePosAsCell);
                                        UnbindBrick();
                                        selectedBrick = null;
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

