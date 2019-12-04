using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Oddmatics.Rzxe.Game;
using Oddmatics.Rzxe.Game.Interface;
using Oddmatics.Rzxe.Windowing.Graphics;
using SharpFont.PostScript;

namespace Junkbot.Game.UI.Menus
{
    internal abstract class UIPage : IDisposable
    {
        internal JunkbotGame JunkbotGame { get; }
        public UxShell Shell { get; }
        public GameState State { get; }

        public UIPage(UxShell shell, JunkbotGame junkbotGame, GameState state)
        {
            Shell = shell;
            State = state;
            JunkbotGame = junkbotGame;
        }
        public virtual void ChangeProperty()
        { }

        public void Render(IGraphicsController graphics)
        { }

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
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
    }
}
