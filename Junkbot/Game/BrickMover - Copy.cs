using Junkbot.Game.World.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game
{
    class BrickMoverCopy
    {
        public Scene Scene;
        public BrickActor selectedBrick { get; set; }

        public BrickMoverCopy(Scene scene)
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
        private IList<BrickActor> ParseRow(IList<BrickActor> row, FacingDirection direction)
            {

                var ignoredBricks = Scene.IgnoredBricks;
                IList<BrickActor> tempConnected = new List<BrickActor>();
                IList<BrickActor> selected = new List<BrickActor>();
                var newRow = RemoveIgnoredBricks(row);
                bool rowStatus = true;
                var currentRow = row;
                if (row.Count > 0)
                {
                    if (direction == FacingDirection.Up)
                    {
                        foreach (BrickActor connectedBrick in newRow)
                        {
                            var currentAboveRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Up);
                            var currentBelowRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Down);
                            bool isAboveRowGrey = IsRowAllGrey(currentAboveRow);
                            if (!isAboveRowGrey)
                            {
                                foreach (BrickActor surroundingBrick in currentAboveRow)
                                {
                                    var surroundingBrickBelowRow =
                                        CheckSurroundingBricks(surroundingBrick, FacingDirection.Down);
                                    bool canBrickMove = CanBrickMove(surroundingBrickBelowRow);
                                    if (canBrickMove)
                                    {

                                        if (surroundingBrick.Color.Name != "Gray")
                                        {
                                            if (!tempConnected.Contains(surroundingBrick))
                                            {
                                                tempConnected.Add(surroundingBrick);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (!tempConnected.Contains(surroundingBrick))
                                        {
                                            surroundingBrick.CanMove = false;
                                            tempConnected.Add(surroundingBrick);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var canBrickMove = CanBrickMove(currentAboveRow);
                                /*                            if (!canBrickMove)
                                                            {*/
                                connectedBrick.CanMove = false;
                                //}
                                ignoredBricks.Add(connectedBrick);

                                selected.Add(connectedBrick);
                            }
                            ignoredBricks.Add(connectedBrick);
                        }
                    }

                    if (direction == FacingDirection.Down)
                    {
                        foreach (BrickActor connectedBrick in newRow)
                        {
                            var currentAboveRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Up);
                            var currentBelowRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Down);
                            bool isBelowRowGrey = IsRowAllGrey(currentBelowRow);
                            if (!isBelowRowGrey)
                            {
                                foreach (BrickActor surroundingBrick in currentBelowRow)
                                {
                                    var surroundingBrickAboveRow =
                                        CheckSurroundingBricks(connectedBrick, FacingDirection.Up);
                                    bool canBrickMove = CanBrickMove(surroundingBrickAboveRow);
                                    if (canBrickMove)
                                    {

                                        if (surroundingBrick.Color.Name != "Gray")
                                        {
                                            if (!tempConnected.Contains(surroundingBrick))
                                            {
                                                tempConnected.Add(surroundingBrick);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (!tempConnected.Contains(surroundingBrick))
                                        {
                                            surroundingBrick.CanMove = false;
                                            tempConnected.Add(surroundingBrick);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var canBrickMove = CanBrickMove(currentAboveRow);
    /*                            if (!canBrickMove)
                                {*/
                                connectedBrick.CanMove = false;
                                //}

                                ignoredBricks.Add(connectedBrick);

                                selected.Add(connectedBrick);
                            }
                            ignoredBricks.Add(connectedBrick);
                        }
                    }
                }

                int tempCount = tempConnected.Count;
                if (tempCount > 0)
                {
                    if (tempConnected.Count >= 2)
                    {
                        var newTempList = ParseRow(tempConnected, direction);
                        foreach (BrickActor temp in newTempList)
                        {
                            if (temp.CanMove == true)
                            {
                                selected.Add(temp);
                            }
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
                

                    if (tempConnected.Count == 1)
                    {
                        var res = ParseBrick(tempConnected[0], direction);
                        foreach (BrickActor temp in res)
                        {
                            if (temp.CanMove == true)
                            {
                                selected.Add(temp);
                                foreach (BrickActor toAdd in newRow)
                                {
                                    selected.Add(toAdd);
                                }
                            }
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
                }
                return selected;
            }

        public IList<BrickActor> ParseBrick(BrickActor brick, FacingDirection direction)
            {
                var aboveRow = CheckSurroundingBricks(brick, FacingDirection.Up);
                var belowRow = CheckSurroundingBricks(brick, FacingDirection.Down);
                var currentAboveRow = RemoveIgnoredBricks(aboveRow);
                var currentBelowRow = RemoveIgnoredBricks(belowRow);
                bool isAboveRowGrey = IsRowAllGrey(currentAboveRow);
                bool isBelowRowGrey = IsRowAllGrey(currentBelowRow);
                var ignoredBricks = Scene.IgnoredBricks;
                IList<BrickActor> tempConnected = new List<BrickActor>();
                IList<BrickActor> selected = new List<BrickActor>();
                bool rowStatus = true;
                var currentRow = currentAboveRow;
                ignoredBricks.Add(brick);

                if (direction == FacingDirection.Up)
                {
                    currentRow = currentAboveRow;
                    if (currentRow.Count > 0)
                    {
                        rowStatus = IsRowAllGrey(currentAboveRow);
                        if (!rowStatus)
                        {
                            currentRow = currentAboveRow;
                            rowStatus = IsRowAllGrey(currentAboveRow);

                            foreach (BrickActor surroundingBrick in currentAboveRow)
                            {
                                var surroundingBrickBelowRow =
                                    CheckSurroundingBricks(surroundingBrick, FacingDirection.Down);
                                bool canBrickMove = CanBrickMove(surroundingBrickBelowRow);
                                if (canBrickMove)
                                {
                                    if (surroundingBrick.Color.Name != "Gray")
                                    {
                                        if (!tempConnected.Contains(surroundingBrick))
                                        {
                                            tempConnected.Add(surroundingBrick);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (!tempConnected.Contains(surroundingBrick))
                                    {
                                        surroundingBrick.CanMove = false;
                                        tempConnected.Add(surroundingBrick);
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool isBelowClear = belowRow.Count > 0 ? false : true;
                            if (!isBelowClear)
                            {
                                brick.CanMove = false;
                            }
                        }
                    }
                }

                if (direction == FacingDirection.Down)
                {
                    currentRow = currentBelowRow;
                    if (currentRow.Count > 0)
                    {
                        rowStatus = IsRowAllGrey(currentBelowRow);
                        if (!rowStatus)
                        {
                            foreach (BrickActor surroundingBrick in currentBelowRow)
                            {
                                var surroundingBrickAboveRow = CheckSurroundingBricks(surroundingBrick, FacingDirection.Up);
                                bool canBrickMove = CanBrickMove(surroundingBrickAboveRow);
                                if (canBrickMove)
                                {
                                    if (surroundingBrick.Color.Name != "Gray")
                                    {
                                        if (!tempConnected.Contains(surroundingBrick))
                                        {
                                            tempConnected.Add(surroundingBrick);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool isAboveClear = aboveRow.Count > 0 ? false : true;
                            if (!isAboveClear)
                            {
                                brick.CanMove = false;
                            }
                        }
                    }
                    else
                    {

                    }
                }

                if (brick.Location.X == 25 && brick.Location.Y == 18)
                {
                }
                int tempCount = tempConnected.Count;

                if (tempCount > 0)
                {
                    if (!rowStatus)
                    {
                        if (tempConnected.Count >= 2)
                        {
                            var newTempList = ParseRow(tempConnected, direction);
                            foreach (BrickActor temp in newTempList)
                            {
                                if (temp.CanMove == true)
                                {

                                    selected.Add(temp);
                                }
                            }
                        }

                        if (tempConnected.Count == 1)
                        {
                            var res = ParseBrick(tempConnected[0], direction);
                            foreach (BrickActor temp in res)
                            {
                                if (temp.CanMove == true)
                                {
                                    selected.Add(temp);
                                }
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
                        var isConnected = IsBrickConnected(brick);
                        foreach (BrickActor temp in isConnected)
                        {
                            if (temp.CanMove == true)
                            {

                                selected.Add(temp);
                            }
                        }
                    }
                }

                if (selected.Count == 0)
                {
                    brick.CanMove = false;
                }
                if (!selected.Contains(brick))
                {
                    selected.Add(brick);
                }
                return selected;
            }


        private IList<BrickActor> RemoveIgnoredBricks(IList<BrickActor> row)
            {
                int count = row.Count;
                IList<BrickActor> newRow = new List<BrickActor>(row);
                var ignoredBricks = Scene.IgnoredBricks;

                // refactor to remove ignored from row between loops
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
                var ignoredBricksCount = Scene.IgnoredBricks.Count;

                int aboveCount = currentAboveRow.Count;
                int belowCount = currentBelowRow.Count;
                bool isAboveRowGrey = IsRowAllGrey(currentAboveRow);

                bool isBelowRowGrey = IsRowAllGrey(currentBelowRow);

                var connectedBricks = Scene.ConnectedBricks;
                IList<BrickActor> tempConnected = new List<BrickActor>();
                /*            if (!isRowGrey || brick == selectedBrick)
                            {*/

                /*currentAboveRow = RemoveIgnoredBricks(currentAboveRow);
                currentBelowRow = RemoveIgnoredBricks(currentBelowRow);*/

                // check if connected above or below
                if (currentAboveRow.Count > 0 || currentBelowRow.Count > 0)
                {
                    if (currentAboveRow.Count >= currentBelowRow.Count)
                    {

                        if (currentAboveRow.Count >= 2)
                        {
                            var newTempList = ParseRow(currentAboveRow, FacingDirection.Up);
                            foreach (BrickActor temp in newTempList)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }

                        if (currentAboveRow.Count == 1)
                        {
                            var res = ParseBrick(currentAboveRow[0], FacingDirection.Up);
                            foreach (BrickActor temp in res)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }



                        currentBelowRow = RemoveIgnoredBricks(currentBelowRow);


                        if (currentBelowRow.Count >= 2)
                        {
                            var newTempList = ParseRow(currentBelowRow, FacingDirection.Down);
                            foreach (BrickActor temp in newTempList)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }

                        if (currentBelowRow.Count == 1)
                        {
                            ignoredBricks.Add(currentBelowRow[0]);

                            var res = ParseBrick(currentBelowRow[0], FacingDirection.Down);
                            foreach (BrickActor temp in res)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }

                        }
                    }
                    if (currentBelowRow.Count > currentAboveRow.Count)
                    {
                        if (currentBelowRow.Count >= 2)
                        {
                            var newTempList = ParseRow(currentBelowRow, FacingDirection.Down);
                            foreach (BrickActor temp in newTempList)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }

                        if (currentBelowRow.Count == 1)
                        {
                            var res = ParseBrick(currentBelowRow[0], FacingDirection.Down);
                            foreach (BrickActor temp in res)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }


                        currentAboveRow = RemoveIgnoredBricks(currentAboveRow);

                        if (currentAboveRow.Count >= 2)
                        {
                            var newTempList = ParseRow(currentAboveRow, FacingDirection.Up);
                            foreach (BrickActor temp in newTempList)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }

                        if (currentAboveRow.Count == 1)
                        {
                            var res = ParseBrick(currentAboveRow[0], FacingDirection.Up);
                            foreach (BrickActor temp in res)
                            {
                                if (temp.CanMove == true)
                                {
                                    tempConnected.Add(temp);
                                }
                            }
                        }
                    }
                }




                tempConnected.Add(brick);

                bool aboveStatus =
                    ((currentAboveRow.Count == 0 || currentAboveRow.Contains(brick)) && !IsRowAllGrey(currentAboveRow))
                        ? true
                        : false;
                bool belowStatus =
                    ((currentBelowRow.Count == 0 || currentBelowRow.Contains(brick)) && !IsRowAllGrey(currentBelowRow))
                        ? true
                        : false;
                return tempConnected;
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

        public void UpdateSelectedBrickLocation(Point mousePos)
        {
            var connectedBricks = Scene.ConnectedBricks;
            if (connectedBricks.Count > 0)
            {
                foreach (BrickActor connectedBrick in connectedBricks)
                {
                    connectedBrick.MovingLocation = new Point(mousePos.X - (selectedBrick.Location.X - connectedBrick.Location.X), mousePos.Y - (selectedBrick.Location.Y - connectedBrick.Location.Y));

                }
            }
            selectedBrick.MovingLocation = new Point(mousePos.X, mousePos.Y);

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
