using Junkbot.Game.World.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private IList<BrickActor> ParseRow(IList<BrickActor> row, FacingDirection direction)
        {
            var ignoredBricks = Scene.IgnoredBricks;
            IList<BrickActor> tempConnected = new List<BrickActor>();
            IList<BrickActor> selected = new List<BrickActor>();

            if (direction == FacingDirection.Up)
            {
                foreach (BrickActor connectedBrick in row)
                {
                    var currentAboveRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Up);
                    foreach (BrickActor surroundingBrick in currentAboveRow)
                    {
                        var currentBelowRow = CheckSurroundingBricks(surroundingBrick, FacingDirection.Down);
                        bool canBrickMove = CanBrickMove(currentBelowRow);
                        if (canBrickMove)
                        {
                            ignoredBricks.Add(surroundingBrick);

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
            }

            if (direction == FacingDirection.Down)
            {
                foreach (BrickActor connectedBrick in row)
                {
                    var currentBelowRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Down);

                    foreach (BrickActor surroundingBrick in currentBelowRow)
                    {
                        var currentAboveRow = CheckSurroundingBricks(connectedBrick, FacingDirection.Up);
                        bool canBrickMove = CanBrickMove(currentAboveRow);
                        if (canBrickMove)
                        {
                            ignoredBricks.Add(surroundingBrick);

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
            }

            int tempCount = tempConnected.Count;
            if (tempCount > 0)
            {
                int i = 0;
                do
                {
                    foreach (BrickActor ignoredBrick in ignoredBricks)
                    {
                        if (tempConnected.Contains(ignoredBrick))
                        {
                            tempConnected.Remove(ignoredBrick);
                        }
                    }

                    i++;
                } while (i != tempCount);


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
                    var res = IsBrickConnected(tempConnected[0]);
                    foreach (BrickActor temp in res)
                    {
                        if (temp.CanMove == true)
                        {
                            selected.Add(temp);
                        }
                    }
                }
            }
            return selected;
        }

        public IList<BrickActor> IsBrickConnected(BrickActor brick)
        {
           
            var currentAboveRow = CheckSurroundingBricks(brick, FacingDirection.Up);
            var currentBelowRow = CheckSurroundingBricks(brick, FacingDirection.Down);
            var ignoredBricks = Scene.IgnoredBricks;
            int aboveCount = currentAboveRow.Count;
            int belowCount = currentBelowRow.Count;

            bool isBelowRowGrey = IsRowAllGrey(currentBelowRow);

            var connectedBricks = Scene.ConnectedBricks;
            IList<BrickActor> tempConnected = new List<BrickActor>();
            /*            if (!isRowGrey || brick == selectedBrick)
                        {*/
            if (!isBelowRowGrey || brick == selectedBrick)
            {

                // refactor to remove ignored from row between loops
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
                    } while (i != aboveCount);
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
                    } while (i != belowCount);
                }

                // check if connected above\
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
                            var res = IsBrickConnected(currentAboveRow[0]);
                            if (res.Count > 0)
                            {
                                foreach (BrickActor temp in res)
                                {
                                    if (temp.CanMove == true)
                                    {
                                        tempConnected.Add(temp);
                                    }
                                }
                            }
                        }

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

                            var res = IsBrickConnected(currentBelowRow[0]);
                            if (res.Count > 0)
                            {
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
                            ignoredBricks.Add(currentBelowRow[0]);

                            var res = IsBrickConnected(currentBelowRow[0]);
                            if (res.Count > 0)
                            {
                                foreach (BrickActor temp in res)
                                {
                                    if (temp.CanMove == true)
                                    {
                                        tempConnected.Add(temp);
                                    }
                                }
                            }
                        }

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
                            var res = IsBrickConnected(currentAboveRow[0]);
                            if (res.Count > 0)
                            {
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
                    if (brick.Location.X == 26 && brick.Location.Y == 18)
                    {
                    }
                    tempConnected.Add(brick);

                }
                else
                {
                    if (brick.Location.X == 26 && brick.Location.Y == 18)
                    {
                    }

                    if (tempConnected.Count > 0)
                    {
                        /*bool canBrickMove = CanBrickMove(tempConnected);
                        if (canBrickMove)
                        {*/
                            tempConnected.Add(brick);
                                //}
                    }
                    else
                    {
                        if (aboveCount != currentAboveRow.Count || belowCount != currentBelowRow.Count)
                        {
                            brick.CanMove = false;
                            ignoredBricks.Add(brick);

                            tempConnected.Add(brick);
                        }
                    }
                }

            }
            else
            {
                brick.CanMove = false;
                ignoredBricks.Add(brick);
                tempConnected.Add(brick);

            }

            bool aboveStatus = ((currentAboveRow.Count == 0 || currentAboveRow.Contains(brick)) && !IsRowAllGrey(currentAboveRow)) ? true : false;
            bool belowStatus = ((currentBelowRow.Count == 0 || currentBelowRow.Contains(brick)) && !IsRowAllGrey(currentBelowRow)) ? true : false;
/*            int tempInt = currentBelowRow.Count;
            var tempRow = currentBelowRow;

            if (tempInt > 0)
            {
                do
                {

                    if (tempRow[tempInt - 1].Color.Name == "Gray")
                    {
                        currentBelowRow.Remove(tempRow[tempInt - 1]);
                    }
                    tempInt -= 1;
                }
                while (tempInt != 0);
            }*/

            /*            if ((aboveStatus && !belowStatus) || (belowStatus && !aboveStatus) || (!tempConnected.Contains(selectedBrick) && brick == selectedBrick) || (currentAboveRow.Count == 0 && currentBelowRow.Count == 0))
                        {

                            tempConnected.Add(brick);
                        }*/
/*                        if (tempConnected.Count == 0 || (currentAboveRow.Count != 0 && currentBelowRow.Count != 0))
                        {
                            brick.CanMove = false;
                        }*/

            /*}
            else
            {
                brick.CanMove = false;
            }*/

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
