using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oddmatics.Rzxe.Game;

namespace Junkbot.Game.State
{
    public class LevelStats
    {
        internal LevelState LevelState { get; set; }
        public int TotalTrashCount { get; set; }
        public int CollectedTrashCount { get; set; }
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
    }
}
