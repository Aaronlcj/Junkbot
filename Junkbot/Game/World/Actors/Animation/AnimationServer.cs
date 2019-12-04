using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Junkbot.Game.World.Actors.Animation
{
    internal class AnimationServer : IDisposable
    {
        private ActorAnimation ActiveAnimation;

        private AnimationStore AnimationStore;


        public event EventHandler SpecialFrameEntered
        {
            add { ActiveAnimation.SpecialFrameEntered += value; }
            remove { ActiveAnimation.SpecialFrameEntered -= value; }
        }


        public AnimationServer(AnimationStore store)
        {
            AnimationStore = store;
        }

        public bool IsPlaying()
        {
            return ActiveAnimation.IsPlaying;
        }
        public void StopPlaying()
        {
                 ActiveAnimation.Stop();
        }

        public ActorAnimationFrame GetCurrentFrame()
        {
            return ActiveAnimation?.CurrentFrame;
        }

        public bool CompareFrame(ActorAnimationFrame frame)
        {
            return ActiveAnimation.CompareFrame(frame);
        }
        public void GoToAndPlay(string animName)
        {
            if (animName == ActiveAnimation?.Name)
                ActiveAnimation.Restart();
            else
            {
                ActiveAnimation = AnimationStore.GetAnimation(animName);
                ActiveAnimation.Play();
            }
        }
        public void GoToAndPlayThenStop(string animName)
        {
            if (animName == ActiveAnimation?.Name)
                ActiveAnimation.Restart();
            else
            {
                ActiveAnimation = AnimationStore.GetAnimation(animName);
                ActiveAnimation.Play();
            }
        }
        public void GoToAndStop(string animName)
        {
            if (animName == ActiveAnimation?.Name)
                ActiveAnimation.Stop();
            else
                ActiveAnimation = AnimationStore.GetAnimation(animName);
        }

        public void Progress()
        {
            ActiveAnimation?.Step();
        }
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
        ~AnimationServer()
        {
            Dispose(false);
            System.Diagnostics.Trace.WriteLine("AnimationServer's destructor is called.");
        }
    }
}
