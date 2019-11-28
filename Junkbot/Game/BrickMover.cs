using Junkbot.Game.World.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Helpers;

namespace Junkbot.Game
{
    class BrickMover
    {
        public Scene Scene;
        public BrickActor selectedBrick { get; set; }

        public BrickMover(Scene scene)
        {
            selectedBrick = null;
            Scene = scene;

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

        private bool IsBlocked (IList<BrickActor> brickRow)
        {
            foreach (BrickActor brick in brickRow)
            {
                if (!Scene.IgnoredBricks.Contains(brick))
                {
                    return false;
                }
            }
            return true;
        }

        private IList<BrickActor> ParseRow(IList<BrickActor> row, FacingDirection direction)
        {

            var ignoredBricks = Scene.IgnoredBricks;
            IList<BrickActor> connectedBricks = new List<BrickActor>();
            IList<BrickActor> selected = new List<BrickActor>();
            var rowWithoutIgnoredBricks = RemoveIgnoredBricks(row);
            bool rowStatus = true;
            var currentRow = row;
            IList<BrickActor> whichRow = new List<BrickActor>();
            FacingDirection altDirection = direction;
            switch (direction)
            {
                case FacingDirection.Up:
                    altDirection = FacingDirection.Down;
                    break;
                case FacingDirection.Down:
                    altDirection = FacingDirection.Up;
                    break;
            }

            if (row.Count > 0)
            {
                foreach (BrickActor brick in rowWithoutIgnoredBricks)
                {
                    var rowToCheck = CheckSurroundingBricks(brick, direction);
                    bool isAboveRowGrey = IsRowAllGrey(rowToCheck);
                    if (!isAboveRowGrey)
                    {
                        foreach (BrickActor surroundingBrick in rowToCheck)
                        {
                            var surroundingBrickBelowRow = CheckSurroundingBricks(surroundingBrick, altDirection);
                            bool canBrickMove = CanBrickMove(surroundingBrickBelowRow);
                            if (!canBrickMove)
                            {
                                surroundingBrick.CanMove = false;
                            }

                            if (!connectedBricks.Contains(surroundingBrick) && (surroundingBrick.Color.Name != "Gray"))
                            {
                                connectedBricks.Add(surroundingBrick);
                            }
                        }
                    }
                    else
                    {
                        if (brick.Color.Name != "Gray")
                        {
                            brick.CanMove = false;
                            ignoredBricks.Add(brick);
                            selected.Add(brick);
                        }
                    }
                    ignoredBricks.Add(brick);
                }
            }

            int tempCount = connectedBricks.Count;
            if (tempCount > 0)
            {
                if (connectedBricks.Count >= 2)
                {
                    var newTempList = ParseRow(connectedBricks, direction);
                    foreach (BrickActor temp in newTempList)
                    {
                        selected.Add(temp);
                    }

                    foreach (BrickActor temp in row)
                    {
                        currentRow = CheckSurroundingBricks(temp, direction);
                        bool canBrickMove = CanBrickMove(currentRow);

                        if (!canBrickMove)
                        {
                            temp.CanMove = false;
                        }
                        selected.Add(temp);
                    }
                }
                
                if (connectedBricks.Count == 1)
                {
                    var res = ParseBrick(connectedBricks[0], direction);
                    foreach (BrickActor temp in res)
                    {
                        selected.Add(temp);
                    }

                    foreach (BrickActor temp in rowWithoutIgnoredBricks)
                    {
                        currentRow = CheckSurroundingBricks(temp, direction);
                        bool canBrickMove = CanBrickMove(currentRow);

                        if (!canBrickMove)
                        {
                            temp.CanMove = false;
                        }
                        selected.Add(temp);
                    }
                }
            }
            return selected;
        }

        public IList<BrickActor> ParseBrick(BrickActor brick, FacingDirection direction)
        {
            var aboveRow = CheckSurroundingBricks(brick, FacingDirection.Up);
            var belowRow = CheckSurroundingBricks(brick, FacingDirection.Down);
            var currentAboveRow = RemoveIgnoredBricks(aboveRow);
            var currentBelowRow = RemoveIgnoredBricks(belowRow);
            var ignoredBricks = Scene.IgnoredBricks;
            IList<BrickActor> temporaryConnectedBricks = new List<BrickActor>();
            IList<BrickActor> connectedBricks = new List<BrickActor>();
            bool rowStatus = true;
            var currentRow = currentAboveRow;
            IList<BrickActor> whichRow = new List<BrickActor>();
            FacingDirection altDirection = direction;
            ignoredBricks.Add(brick);

            switch (direction)
            {
                case FacingDirection.Up:
                    currentRow = currentAboveRow;
                    altDirection = FacingDirection.Down;
                    whichRow = aboveRow;
                    break;
                case FacingDirection.Down:
                    currentRow = currentBelowRow;
                    altDirection = FacingDirection.Up;
                    whichRow = belowRow;


                    break;
            }

            if (currentRow.Count > 0)
            {
                rowStatus = IsRowAllGrey(currentRow);
                if (!rowStatus)
                {
                    foreach (BrickActor surroundingBrick in currentRow)
                    {
                        var surroundingBrickRow = CheckSurroundingBricks(surroundingBrick, altDirection);
                        bool canBrickMove = CanBrickMove(surroundingBrickRow);
                        if (surroundingBrick.Color.Name == "Gray")
                        {
                            break;
                        }
                        if (!canBrickMove)
                        {
                            surroundingBrick.CanMove = false;
                        }
                        if (!temporaryConnectedBricks.Contains(surroundingBrick))
                        {
                            temporaryConnectedBricks.Add(surroundingBrick);
                        }
                    }
                }
                else
                {
                    if (brick.Color.Name != "Gray")
                    {
                        brick.CanMove = false;
                        ignoredBricks.Add(brick);
                        connectedBricks.Add(brick);
                    }
                }
            }
            else
            {
                if (whichRow.Count > 0)
                {
                    brick.CanMove = false;
                }
            }
            

            int totalTemporaryConnectedBricks = temporaryConnectedBricks.Count;
            if (totalTemporaryConnectedBricks > 0)
            {
                if (!rowStatus)
                {
                    if (temporaryConnectedBricks.Count >= 2)
                    {
                        var returnedBricks = ParseRow(temporaryConnectedBricks, direction);
                        foreach (BrickActor brickToAdd in returnedBricks)
                        {
                            connectedBricks.Add(brickToAdd);
                        }
                        currentRow = CheckSurroundingBricks(brick, direction);
                        bool canBrickMove = CanBrickMove(currentRow);

                        if (!canBrickMove)
                        {
                            brick.CanMove = false;
                        }
                    }

                    if (temporaryConnectedBricks.Count == 1)
                    {
                        var returnedBricks = ParseBrick(temporaryConnectedBricks[0], direction);
                        foreach (BrickActor brickToAdd in returnedBricks)
                        {
                            connectedBricks.Add(brickToAdd);
                        }
                        currentRow = CheckSurroundingBricks(brick, direction);
                        bool canBrickMove = CanBrickMove(currentRow);

                        if (!canBrickMove)
                        {
                            brick.CanMove = false;
                        }
                    }
                }
                else
                {
                    var canBrickMove = CanBrickMove(currentRow);
                    if (!canBrickMove)
                    {
                        brick.CanMove = false;
                    }
                }
            }
            else
            {
                if (rowStatus)
                {
                    // flips parse direction - unsure if needed
                    
                    /* bool isBlocked = currentRow == currentAboveRow ? IsBlocked(belowRow) : IsBlocked(aboveRow);
                    if (!isBlocked)
                    {
                        var isConnected = IsBrickConnected(brick);
                        foreach (BrickActor temp in isConnected)
                        {
                            selected.Add(temp);
                        }
                    }
                    else
                    {  */
                        if (currentRow.Count > 0)
                        {
                            brick.CanMove = false;
                        }
                    //}
                }
            }

            if (connectedBricks.Count == 0)
            {
                if (currentRow.Count > 0)
                {
                    brick.CanMove = false;
                }
            }

            if (!connectedBricks.Contains(brick))
            {
                connectedBricks.Add(brick);
            }

            return connectedBricks;
        }

        private IList<BrickActor> RemoveIgnoredBricks(IList<BrickActor> row)
        {
            int count = row.Count;
            IList<BrickActor> newRow = new List<BrickActor>(row);
            var ignoredBricks = Scene.IgnoredBricks;

            if (count > 0)
            {
                int i = 0;
                do
                {
                    if (ignoredBricks.Contains(row[i]))
                    {
                        newRow.Remove(row[i]);
                    }
                    i++;
                } while (i != count);
            }
            return newRow;
        }
       
        public IList<BrickActor> IsBrickConnected(BrickActor brick)
        {
            var aboveRow = CheckSurroundingBricks(brick, FacingDirection.Up);
            var belowRow = CheckSurroundingBricks(brick, FacingDirection.Down);
            var currentAboveRow = RemoveIgnoredBricks(aboveRow);
            var currentBelowRow = RemoveIgnoredBricks(belowRow);
            var ignoredBricks = Scene.IgnoredBricks;
            bool isAboveRowGrey = IsRowAllGrey(currentAboveRow);
            bool isBelowRowGrey = IsRowAllGrey(currentBelowRow);

            IList<BrickActor> connectedBricks = new List<BrickActor>();
            /// need to refactor asap - can easily reduce 75% of this loop
            if (currentAboveRow.Count > 0 || currentBelowRow.Count > 0)
            {
                if (currentAboveRow.Count >= currentBelowRow.Count)
                {
                    if (!isAboveRowGrey)
                    {
                        if (currentAboveRow.Count >= 2)
                        {
                            var returnedBricks = ParseRow(currentAboveRow, FacingDirection.Up);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }

                        if (currentAboveRow.Count == 1)
                        {
                            var returnedBricks = ParseBrick(currentAboveRow[0], FacingDirection.Up);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }
                    }

                    currentBelowRow = RemoveIgnoredBricks(currentBelowRow);

                    if (!isBelowRowGrey)
                    {
                        if (currentBelowRow.Count >= 2)
                        {
                            var returnedBricks = ParseRow(currentBelowRow, FacingDirection.Down);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }

                        if (currentBelowRow.Count == 1)
                        {
                            ignoredBricks.Add(currentBelowRow[0]);

                            var returnedBricks = ParseBrick(currentBelowRow[0], FacingDirection.Down);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }

                        }
                    }
                }

                if (currentBelowRow.Count > currentAboveRow.Count)
                {
                    if (!isBelowRowGrey)
                    {
                        if (currentBelowRow.Count >= 2)
                        {
                            var returnedBricks = ParseRow(currentBelowRow, FacingDirection.Down);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }

                        if (currentBelowRow.Count == 1)
                        {
                            var returnedBricks = ParseBrick(currentBelowRow[0], FacingDirection.Down);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }
                    }

                    currentAboveRow = RemoveIgnoredBricks(currentAboveRow);

                    if (!isAboveRowGrey)
                    {
                        if (currentAboveRow.Count >= 2)
                        {
                            var returnedBricks = ParseRow(currentAboveRow, FacingDirection.Up);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }

                        if (currentAboveRow.Count == 1)
                        {
                            var returnedBricks = ParseBrick(currentAboveRow[0], FacingDirection.Up);
                            foreach (BrickActor brickToAdd in returnedBricks)
                            {
                                connectedBricks.Add(brickToAdd);
                            }
                        }
                    }
                }
            }
            connectedBricks.Add(brick);
            return connectedBricks;
        }

        private IList<BrickActor> CheckSurroundingBricks(BrickActor brick, FacingDirection direction)
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
                do
                {
                    if ((x + currentCellX >= 0 && y + dx >= 0) && (x + currentCellX <= 34 && y + dx <= 21))
                    {
                        IActor cell = Scene.GetPlayfield[x + currentCellX, y + dx];
                        if (cell != null && !currentRow.Contains(cell as BrickActor) && !Scene.ConnectedBricks.Contains(cell as BrickActor))
                        {
                            currentRow.Add(cell as BrickActor);
                        }

                        currentCellX++;
                    }
                }
                while ((currentCellX) != brick.GridSize.Width);

                return currentRow;
            }

        public void UpdateSelectedBrickLocation(Point mousePos)
        {
            var connectedBricks = Scene.ConnectedBricks;
            Array.Clear(Scene.SelectedGrid, 0, Scene.SelectedGrid.Length - 1);

            foreach (BrickActor connectedBrick in connectedBricks)
            {
                connectedBrick.MovingLocation = new Point(mousePos.X - (selectedBrick.Location.X - connectedBrick.Location.X), mousePos.Y - (selectedBrick.Location.Y - connectedBrick.Location.Y));
                MoveBrickSelectedGrid(connectedBrick);
            }
        }
        public void MoveBrickSelectedGrid(BrickActor brick)
        {
            var movingLocationCells = new List<Point>();

            foreach (Rectangle rect in brick.BoundingBoxes)
            {
                movingLocationCells.AddRange((new Rectangle(brick.MovingLocation.Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
            }

            if (brick.Selected)
            {
                foreach (Point cell in movingLocationCells)
                {
                    Scene.SelectedGrid[cell.X, cell.Y] = brick;
                }
            }
        }
        public void MoveBrickFromPlayfield(BrickActor brick)
        {
            var locationCells = new List<Point>();
            var movingLocationCells = new List<Point>();

            foreach (Rectangle rect in brick.BoundingBoxes)
            {
                locationCells.AddRange((new Rectangle(brick.Location.Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
                movingLocationCells.AddRange((new Rectangle(brick.MovingLocation.Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
            }

            if (brick.Selected)
            {
                foreach (Point cell in locationCells)
                {
                    Scene.SelectedGrid[cell.X, cell.Y] = brick;
                    Scene.GetPlayfield[cell.X, cell.Y] = null;
                }
            }
            else
            {
                foreach (Point cell in locationCells)
                {
                    Scene.SelectedGrid[cell.X, cell.Y] = null;
                }
                foreach (Point cell in movingLocationCells)
                {
                    Scene.GetPlayfield[cell.X, cell.Y] = brick;
                }
            }
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
    }
}
