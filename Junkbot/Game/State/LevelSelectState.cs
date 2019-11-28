using Junkbot.Helpers;
using Junkbot.Game.World.Actors;
using Junkbot.Game.World.Actors.Animation;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Input;
using Oddmatics.Rzxe.Windowing.Graphics;
using System;
using System.Drawing;
using System.IO;
using Pencil.Gaming;
using System.Collections.Generic;
using Junkbot.Game.World.Level;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Junkbot.Game.State
{
    /// <summary>
    /// Represents the main menu game state.
    /// </summary>
    internal class LevelSelectState : GameState
    {
        System.Timers.Timer _timer;

        public override InputFocalMode FocalMode
        {
            get { return InputFocalMode.Always; }
        }
        public static string[] lvl;
        private List<string> _levelList;
        public Scene Scene;
        public BrickMover BrickMover;
        public LevelSelectButtons Buttons;
        public LevelSelectBackground LevelSelectBackground;
        public LevelSelectText LevelSelectText;
        public HelpMenu HelpMenu;
        public LevelSelectGif LevelSelectGif;

        public static AnimationStore Store = new AnimationStore();
        public override string Name
        {
            get { return "LevelSelect"; }
        }

        public LevelSelectState(string level)
        {
            Buttons = new LevelSelectButtons();
            LevelSelectBackground = new LevelSelectBackground();
            HelpMenu = new HelpMenu();
            LevelSelectGif = new LevelSelectGif();
            var key = $"Building_{Buttons.Tab}";

            JToken jsonLevels = JObject.Parse(File.ReadAllText(Environment.CurrentDirectory + @"\Content\Levels\level_list.json"))[key];
            _levelList = jsonLevels.ToObject<List<string>>();
            LevelSelectText = new LevelSelectText(_levelList, Buttons);

            lvl = File.ReadAllLines(Environment.CurrentDirectory + $@"\Content\Levels\{level}.txt");
            Scene = Scene.FromLevel(lvl, Store);
            SetTimer();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Scene.UpdateActors();
        }
        public void SetTimer()
        {
            _timer = new System.Timers.Timer(25);
            _timer.Elapsed += Timer_Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public override void ProcessInputs(InputEvents inputs)
        {
        }

        public override void RenderFrame(IGraphicsController graphics)
        {
            graphics.ClearViewport(Color.CornflowerBlue);
            LevelSelectBackground.Render(graphics);
            LevelSelectGif.Render(graphics);
            Buttons.Render(graphics); 
            LevelSelectText.Render(graphics);

            //HelpMenu.Render(graphics);
        }
    }
}

