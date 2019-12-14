using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World
{
    public class PlayerData
    {
        public int BuildingState { get; set; }
        public int TotalMoves { get; set; }
        public PlayerLevelStats LevelStats { get; set; }
    }
    public class PlayerLevelStats
    {
        public List<PlayerLevelData> Building1 { get; set; }
        public List<PlayerLevelData> Building2 { get; set; }
        public List<PlayerLevelData> Building3 { get; set; }
        public List<PlayerLevelData> Building4 { get; set; }
        public List<PlayerLevelData> GetBuilding(int tab)
        {
            List<PlayerLevelData> list = new List<PlayerLevelData>();
            switch (tab)
            {
                case 1:
                    list = Building1;
                    break;
                case 2:
                    list = Building2;
                    break;
                case 3:
                    list = Building3;
                    break;
                case 4:
                    list = Building4;
                    break;
            }
            return list;
        }
    }

    public class PlayerLevelData : LevelListData
    {
        public int BestMoves { get; set; }
        public bool Par { get; set; }
        public bool Key { get; set; }
    }
    public class LevelList
    {
        public List<LevelListData> Building1 { get; set; }
        public List<LevelListData> Building2 { get; set; }
        public List<LevelListData> Building3 { get; set; }
        public List<LevelListData> Building4 { get; set; }
        public List<LevelListData> GetBuilding(int tab)
        {
            List<LevelListData> list = new List<LevelListData>();
            switch (tab)
            {
                case 1:
                    list = Building1;
                    break;
                case 2:
                    list = Building2;
                    break;
                case 3:
                    list = Building3;
                    break;
                case 4:
                    list = Building4;
                    break;
            }
            return list;
        }

    }
    public class LevelListData
    {
        public int Level { get; set; }
        public string Name { get; set; }
    }
}
