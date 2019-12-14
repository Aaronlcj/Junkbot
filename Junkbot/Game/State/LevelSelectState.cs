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
using System.Runtime.InteropServices;
using Junkbot.Game.Logic;
using Junkbot.Game.World;
using Junkbot.Game.World.Level;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oddmatics.Rzxe.Game.Interface;

namespace Junkbot.Game.State
{
    /// <summary>
    /// Represents the main menu game state.
    /// </summary>
    internal class LevelSelectState : GameState
    {
        System.Timers.Timer _timer;
        internal JunkbotGame JunkbotGame;
        public override InputFocalMode FocalMode
        {
            get { return InputFocalMode.Always; }
        }
        public static string[] lvl;
        private List<LevelListData> _levelList;
        public LevelSelectButtons Buttons;
        public LevelSelectBackground LevelSelectBackground;
        public LevelSelectText LevelSelectText;
        public HelpMenu HelpMenu { get; set; }
        public LevelSelectGif LevelSelectGif;

        private UxShell Shell { get; set; }


        public static AnimationStore Store;
        public override string Name
        {
            get { return "LevelSelect"; }
        }

        public LevelSelectState(JunkbotGame junkbotGame, string level)
        {
            JunkbotGame = junkbotGame;
            Shell = new UxShell();
            Buttons = new LevelSelectButtons(Shell, JunkbotGame, this);
            LevelSelectBackground = new LevelSelectBackground();
            LevelSelectGif = new LevelSelectGif();
            var key = $"Building_{Buttons.Tab}";
            Store = new AnimationStore();

            var jsonLevels = JsonConvert.DeserializeObject<LevelList>(File.ReadAllText(Environment.CurrentDirectory + @"\Content\Levels\level_list.json"));
            _levelList = jsonLevels.GetBuilding(Buttons.Tab);
            /*JToken jsonLevels = JObject.Parse(File.ReadAllText(Environment.CurrentDirectory + @"\Content\Levels\level_list.json"))[key];
            _levelList = jsonLevels.ToObject<List<string>>();*/
            LevelSelectText = new LevelSelectText(JunkbotGame, Shell, _levelList, Buttons.Tab, Buttons, this);
        }


        public override void RenderFrame(IGraphicsController graphics)
        {

            graphics.ClearViewport(Color.CornflowerBlue);
            LevelSelectBackground.Render(graphics);
            LevelSelectGif.Render(graphics);
            Buttons.Render(graphics);
            LevelSelectText.Render(graphics);

            if (HelpMenu != null)
            {
                HelpMenu.Render(graphics);
            }
        }
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }

        public override void Update(TimeSpan deltaTime, InputEvents inputs)
        {
            if (inputs != null)
            {
                Shell.HandleMouseInputs(inputs);
            }
        }
        ~LevelSelectState()
        {
            Dispose(false);

            System.Diagnostics.Trace.WriteLine("LevelSelect's destructor is called.");
        }
    }
}

