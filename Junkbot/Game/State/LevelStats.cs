using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Junkbot.Game.World;
using Oddmatics.Rzxe.Game;

namespace Junkbot.Game.State
{
    public class LevelStats
    {
        public string Title { get; set; }
        internal LevelState LevelState { get; set; }
        public int TotalTrashCount { get; set; }
        public int CollectedTrashCount { get; set; }
        public int Moves { get; set; }
        public ushort Par { get; set; }
        public LevelStats()
        {
            TotalTrashCount = 0;
            CollectedTrashCount = 0;
        }

        internal void SetLevelState(LevelState state)
        {
            LevelState = state;
        }

        public bool CheckTrashStatus()
        {
            if (CollectedTrashCount == TotalTrashCount)
            {
                LevelComplete(false);
                LevelState.UpdatePlayerData(Moves, Moves <= Par ? true : false);
                return true;
            }

            return false;
        }

        public void JunkbotHit()
        {
            LevelComplete(true);
        }

        private void LevelComplete(bool failStatus)
        {
            LevelState.CreateEndLevelCard(failStatus);
        }

        private void UpdatePlayerData()
        {
            

        }
    }
}
