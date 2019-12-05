using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Junkbot.Game.World
{
    class PlayerData
    {
        internal int BuildingState { get; set; }
        internal int TotalMoves { get; set; }
        internal List<PlayerBuildingData> LevelStats { get; set; }
    }
    class PlayerLevelStats
    {
        internal PlayerBuildingData Building_1 { get; set; }
        internal PlayerBuildingData Building_2 { get; set; }
        internal PlayerBuildingData Building_3 { get; set; }
        internal PlayerBuildingData Building_4 { get; set; }

    }

    class PlayerBuildingData
    {
        internal List<PlayerLevelData> Levels { get; set; }
    }

    class PlayerLevelData
    {
        internal string Name { get; set; }
        internal int BestMoves { get; set; }
        internal bool Par { get; set; }
        internal bool Key { get; set; }
    }
}
