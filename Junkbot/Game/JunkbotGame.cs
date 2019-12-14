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
using Newtonsoft.Json;
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
            var json = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(Environment.CurrentDirectory + @"\Content\PlayerData\player.json"));
            PlayerData = json;
        }

        internal void SavePlayerData()
        {
            var json = JsonConvert.SerializeObject(PlayerData, Formatting.Indented);
            File.WriteAllText(Environment.CurrentDirectory + @"\Content\PlayerData\player.json", json);
            Console.WriteLine(json);

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
