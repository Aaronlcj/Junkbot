﻿using Junkbot.Game.World.Actors;
using Junkbot.Game.World.Actors.Animation;
using Junkbot.Game.World.Level;
using Junkbot.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game
{
    internal class Scene
    {
        public Size CellSize { get; private set; }
        public IList<JunkbotDecalData> Decals
        {
            get { return _Decals.AsReadOnly(); }
        }

        private List<JunkbotDecalData> _Decals;


        public IList<BrickActor> ConnectedBricks { get; set; }

        public IList<IActor> MobileActors
        {
            get { return _MobileActors.AsReadOnly(); }
        }
        private List<IActor> _MobileActors;

        public IList<BrickActor> ImmobileBricks
        {
            get { return _ImmobileBricks.AsReadOnly(); }
        }
        private List<BrickActor> _ImmobileBricks;

        public Size Size { get; private set; }

        public JunkbotLevelData LevelData { get; private set; }


        private AnimationStore AnimationStore;

        private IActor[,] PlayField;

        public IActor[,] GetPlayfield
        {
            get { return PlayField; }
        }

        public IActor[,] BrickGrid { get; set; }




        public Scene(JunkbotLevelData levelData, AnimationStore store)
        {
            _MobileActors = new List<IActor>();
            _ImmobileBricks = new List<BrickActor>();
            _Decals = new List<JunkbotDecalData>();
            AnimationStore = store;
            PlayField = new IActor[levelData.Size.Width + 1, levelData.Size.Height];
            BrickGrid = new IActor[levelData.Size.Width + 1, levelData.Size.Height];
            CellSize = levelData.Spacing;
            Size = levelData.Size;
            LevelData = levelData;
            ConnectedBricks = new List<BrickActor>();



            foreach (JunkbotPartData part in levelData.Parts)
            {
                IActor actor = null;
                Color color = Color.FromName(levelData.Colors[part.ColorIndex]);
                Point location = part.Location; // Subtract one to get zero-indexed location

                switch (levelData.Types[part.TypeIndex])
                {
                    case "brick_01":
                        actor = new BrickActor(store, this, location, color, BrickSize.One);
                        break;

                    case "brick_02":
                        actor = new BrickActor(store, this, location, color, BrickSize.Two);
                        break;

                    case "brick_03":
                        actor = new BrickActor(store, this, location, color, BrickSize.Three);
                        break;

                    case "brick_04":
                        actor = new BrickActor(store, this, location, color, BrickSize.Four);
                        break;

                    case "brick_06":
                        actor = new BrickActor(store, this, location, color, BrickSize.Six);
                        break;

                    case "brick_08":
                        actor = new BrickActor(store, this, location, color, BrickSize.Eight);
                        break;

                    case "minifig":
                        actor = new JunkbotActor(store, this, new Point(location.X, location.Y), (part.AnimationName == "walk_l" ? FacingDirection.Left : FacingDirection.Right));
                        break;
                    case "haz_climber":
                        actor = new BotActor(store, this, location, (part.AnimationName == "walk_r" ? FacingDirection.Right : FacingDirection.Left));
                        break;

                    case "flag":
                        actor = new BinActor(store, location);
                        break;

                    default:
                        Console.WriteLine("Unknown actor: " + levelData.Types[part.TypeIndex]);
                        continue;
                }

                actor.Location = location.Subtract(new Point(1, actor.GridSize.Height));
                UpdateActorGridPosition(actor, actor.Location);

                actor.LocationChanged += Actor_LocationChanged;
                if (actor is BrickActor)
                {
/*                    (actor as BrickActor).BoundLocationChanged += Actor_BoundLocationChanged;
*/                    (actor as BrickActor).BoundLocation = location.Subtract(new Point(1, actor.GridSize.Height));

                }

                if (actor is BrickActor)
                {
                    var brick = (BrickActor)actor;
                    _ImmobileBricks.InsertSorted((BrickActor)actor);
                }
                else
                    _MobileActors.Add(actor);
            }
        }

        private void Actor_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (!(sender is BrickActor && (sender as BrickActor).IsBound))
            {
            UpdateActorGridPosition((IActor)sender, e.NewLocation, e.OldLocation);
            }
        }

        private void Actor_BoundLocationChanged(object sender, LocationChangedEventArgs e)
        {
            BrickGrid[e.NewLocation.X, e.NewLocation.Y] = sender as IActor;
            PlayField[e.OldLocation.X, e.OldLocation.Y] = null;

            /*            UpdateActorGridPosition((IActor)sender, e.NewLocation, e.OldLocation);
            */
        }

        public void MoveBrickFromPlayfield(BrickActor brick)
        {
            var locationCells = new List<Point>();
            var boundlocationCells = new List<Point>();

            foreach (Rectangle rect in brick.BoundingBoxes)
            {
                locationCells.AddRange((new Rectangle(brick.Location.Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
                boundlocationCells.AddRange((new Rectangle(brick.BoundLocation.Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
            }

            if (brick.IsBound)
            {
                foreach (Point cell in locationCells)
                {
                    BrickGrid[cell.X, cell.Y] = brick;
                    PlayField[cell.X, cell.Y] = null;
                }
            }

            else
            {
                foreach (Point cell in locationCells)
                {
                    BrickGrid[cell.X, cell.Y] = null;
                }
                foreach (Point cell in boundlocationCells)
                {
                    PlayField[cell.X, cell.Y] = brick;
                }

            }
        }
    
        public bool CheckGridRegionFree(Rectangle region)
        {
            Point[] cellsToCheck = region.ExpandToGridCoordinates();

            foreach (Point cell in cellsToCheck)
            {

       /*        int t1 = PlayField.GetLength(0);
                    int t2 = PlayField.GetLength(1);*/
                if ((cell.X < 0 || cell.X > 34) || (cell.Y < 0 || cell.Y > 21))
                    return false;

                if (PlayField[cell.X, cell.Y] != null)
                    return false;
            }

            return true;
        }

        public bool CheckFloorExists(JunkbotActor actor)
        {
            Point actorPos = actor.Location;
            int directionModifier = actorPos.X;

            switch (actor.FacingDirection)
            {
                case FacingDirection.Right:
                    directionModifier += 1;
                    break;
                case FacingDirection.Left:
                    directionModifier -= 0;
                    break;
            }
            Point cellToCheck = new Point(directionModifier, actorPos.Y + 4);

            if (PlayField[cellToCheck.X, cellToCheck.Y] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckBotFloorExists(BotActor actor)
        {
            Point actorPos = actor.Location;
            int directionModifier = actorPos.X;

            switch (actor.FacingDirection)
            {
                case FacingDirection.Right:
                    directionModifier += 2;
                    break;
                case FacingDirection.Left:
                    directionModifier -= -1;
                    break;
            }
            Point cellToCheck = new Point(actorPos.X, actorPos.Y + 2);

            if (PlayField[cellToCheck.X, cellToCheck.Y] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void UpdateActors()
        {
            foreach (IActor actor in _MobileActors)
            {
                actor.Update();
            }
        }


        private void AssertGridInSync(IActor actor, Point[] cellsToCheck)
        {
            foreach (Point cell in cellsToCheck)
            {
/*                bool test = (BrickGrid[(actor as BrickActor).BoundLocation.X, (actor as BrickActor).BoundLocation.Y] != actor);
*/                if (PlayField[cell.X, cell.Y] != actor)
                {
                   /* if (BrickGrid[cell.X, cell.Y] != actor)
                    {
                        throw new Exception("Scene.VerifyGridInSync: Grid out of sync!! X:" + cell.X.ToString() + ", Y:" + cell.Y.ToString());
                    }*/
                }
            }
        }

        private void AssignGridCells(IActor actor, Point[] cells)
        {
            foreach (Point cell in cells)
            {
                if (cell.X < 35)
                {
                    PlayField[cell.X, cell.Y] = actor;
                    ;
                }
                if (cell.X == 35 && cell.Y == 21)
                {
                    PlayField[cell.X, cell.Y] = null;
                }
            }
                // Bomb out if the cell is not free (naughty actor!)
                //
                /*   if (PlayField[cell.X, cell.Y] != null)

                       throw new Exception("Scene.AssignGridCells: Attempted to assign an occupied cell!! X:" + cell.X.ToString() + ", Y:" + cell.Y.ToString());
   */
            
        }

        public bool BoundaryCheck(Rectangle region)
        {
            Point[] cellsToCheck = region.ExpandToGridCoordinates();
            foreach (Point cell in cellsToCheck)
            {
                if (cell.X < 0 || cell.X >= PlayField.GetLength(0) || cell.Y < 0 || cell.Y >= PlayField.GetLength(1))
                {
                    return true;
                }
            }
            return false;

        }
        private bool CheckBoundaryCollision(IActor actor, Point[] cells)
        {
            if (cells.Length >= 1)
            {
                if (cells[1].X == 36)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        private void ClearGridCells(Point[] cells)
        {
            foreach (Point cell in cells)
            {
                PlayField[cell.X, cell.Y] = null;
            }
        }

        private void UpdateActorGridPosition(IActor actor, Point newPos, Point? oldPos = null)
        {
            

            // Update new cells
            //

            var newCells = new List<Point>();

            foreach (Rectangle rect in actor.BoundingBoxes)
            {
                newCells.AddRange((new Rectangle(newPos.Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
            }
/*            bool gridEdge = CheckBoundaryCollision(actor, newCells.ToArray());
*/
            // If oldPos has been specified, verify and clear
            //
            if (oldPos != null)
            {
                var oldCells = new List<Point>();

                foreach (Rectangle rect in actor.BoundingBoxes)
                {
                    oldCells.AddRange((new Rectangle(((Point)oldPos).Add(rect.Location), rect.Size)).ExpandToGridCoordinates());
                }

                var oldCellsArr = oldCells.ToArray();

                AssertGridInSync(actor, oldCellsArr);
                /*                   if (!gridEdge)
                 *                   
                */
                /*if (actor is JunkbotActor)
                {
                    if (!BoundaryCheck((actor as JunkbotActor).GetCheckBounds()))
                    {*/
                        ClearGridCells(oldCellsArr);
                  /*  }
                }*/
            }
           /* if (actor is JunkbotActor)
            {
                bool bnd = BoundaryCheck((actor as JunkbotActor).GetCheckBounds());
                if (!bnd)
                {
                    AssignGridCells(actor, newCells.ToArray());
                }
                else
                {
                    Console.WriteLine("");
                    BoundaryCheck((actor as JunkbotActor).GetCheckBounds());
                }
            }
            else
            {*/
                AssignGridCells(actor, newCells.ToArray());
            /*}*/

        }

        /*   if (!gridEdge)
           {*/
        /*}*/
    


        public static Scene FromLevel(string[] lvlFile, AnimationStore store)
        {
            var levelData = new JunkbotLevelData();
            var parts = new List<JunkbotPartData>();
            var decals = new List<JunkbotDecalData>();
            foreach (string line in lvlFile)
            {
                // Try retrieving the data
                //
                string[] definition = line.Split('=');

                if (definition.Length != 2)
                    continue; // Not a definition

                // Retrieve key and value
                //
                string key = definition[0].ToLower().Trim();
                string value = definition[1];

                switch (key)
                {
                    case "colors":
                        levelData.Colors = value.ToLower().Split(',');
                        break;

                    case "hint":
                        levelData.Hint = value;
                        break;

                    case "par":
                        levelData.Par = Convert.ToUInt16(value);
                        break;

                    case "parts":
                        string[] partsDefs = value.ToLower().Split(',');

                        foreach (string def in partsDefs)
                        {
                            string[] partData = def.Split(';');

                            if (partData.Length != 7)
                            {
                                Console.WriteLine("Invalid part data encountered");
                                continue;
                            }

                            var part = new JunkbotPartData();

                            part.Location = new Point(
                                Convert.ToInt32(partData[0]),
                                Convert.ToInt32(partData[1])
                                );

                            part.TypeIndex = (byte)(Convert.ToByte(partData[2]) - 1); // Minus one to convert to zero-indexed index

                            part.ColorIndex = (byte)(Convert.ToByte(partData[3]) - 1); // Minus one to convert to zero-indexed index

                            part.AnimationName = partData[4].ToLower();

                            parts.Add(part);
                        }

                        break;

                    case "scale":
                        levelData.Scale = Convert.ToByte(value);
                        break;

                    case "size":
                        string[] sizeCsv = value.Split(',');

                        if (sizeCsv.Length != 2)
                        {
                            Console.WriteLine("Invalid playfield size encountered");
                            continue;
                        }

                        levelData.Size = new Size(
                            Convert.ToInt32(sizeCsv[0]),
                            Convert.ToInt32(sizeCsv[1])
                            );

                        break;

                    case "spacing":
                        string[] spacingCsv = value.Split(',');

                        if (spacingCsv.Length != 2)
                        {
                            Console.WriteLine("Invalid playfield spacing encountered");
                            continue;
                        }

                        levelData.Spacing = new Size(
                            Convert.ToInt32(spacingCsv[0]),
                            Convert.ToInt32(spacingCsv[1])
                            );

                        break;

                    case "title":
                        levelData.Title = value;
                        break;

                    case "types":
                        var types = new List<string>();

                        if (levelData.Types != null)
                            types.AddRange(levelData.Types);

                        types.AddRange(value.ToLower().Split(','));

                        levelData.Types = types.ToArray();

                        break;
                    case "decals":
                        string[] decalsDef = value.Split(','); //Splits up each decal in a row.

                        foreach (string def in decalsDef)
                        {
                            string[] decalData = def.Split(';'); //first two define X and Y, then the Decal type
                            if (decalData.Length != 3)
                            {
                                Console.WriteLine("Invalid decal data encountered");
                                continue;
                            }
                            var decal = new JunkbotDecalData(); //a new struct for storing decal data: its sprite and position.
                            decal.Location = new Point(
                                Convert.ToInt32(decalData[0]),
                                Convert.ToInt32(decalData[1])
                                );
                            decal.Decal = decalData[2]; //If I'm not mistaken, this will pass the relevant information from the level to the decal entry.
                                                        //Now, all that remains is to get stuff sorted out.
                            decals.Add(decal);
                        }
                        break;
                    case "backdrop":
                        levelData.Backdrop = value;
                        break;
                }
            }

            levelData.Parts = parts.AsReadOnly();
            levelData.Decals = decals.AsReadOnly();
            return new Scene(levelData, store);
        }
    }
}
