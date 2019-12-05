using Junkbot.Game.State;
using Junkbot.Game.World.Actors.Animation;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Input;
using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Junkbot.Game.UI.Menus.Help;
using Junkbot.Game.World;
using Newtonsoft.Json.Linq;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace Junkbot.Game
{
    /// <summary>
    /// Represents the main Junkbot game engine.
    /// </summary>
    internal sealed class JunkbotGame : IGameEngine
    {
        public IGameEngineParameters Parameters
        {
            get { return _Parameters; }
        }

        private IGameEngineParameters _Parameters;

        public GameState CurrentGameState
        {
            get;
            set;
        }

        internal PlayerData PlayerData { get; set; }

        public JunkbotGame()
        {
            _Parameters = new JunkbotEngineParameters();
        }


        public void Begin()
        {
            LoadPlayerData();
            string level = "loading_level";

            CurrentGameState = new MainMenuState(level, this);
        }

        internal void LoadPlayerData()
        {
            JObject jsonLevels = JObject.Parse(File.ReadAllText(Environment.CurrentDirectory + @"\Content\PlayerData\player.json"));
            JToken levelStatsJson = jsonLevels["LevelStats"];
            PlayerData = new PlayerData()
            {
                BuildingState = (int)jsonLevels["BuildingState"],
                TotalMoves = (int)jsonLevels["TotalMoves"],
                LevelStats = levelStatsJson.Select(building => new PlayerBuildingData
                {
                    Levels = building.Children<JObject>().Properties().Select(level => new PlayerLevelData
                        {
                            Name = level.Name,
                            BestMoves = (int) level.Value["BestMoves"],
                            Key = (bool) level.Value["Key"],
                            Par = (bool) level.Value["Par"]
                        }
                    ).ToList()
                }).ToList()
            };
        }


        public void RenderFrame(IGraphicsController graphics)
        {
            graphics.ClearViewport(Color.CornflowerBlue);

            CurrentGameState.RenderFrame(graphics);
        }

        public void Update(TimeSpan deltaTime, InputEvents inputs)
        {
            CurrentGameState.Update(deltaTime, inputs);
        }
    }
}
