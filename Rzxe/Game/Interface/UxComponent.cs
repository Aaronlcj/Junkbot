using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oddmatics.Rzxe.Game
{
    public abstract class UxComponent
    {
        public Rectangle Bounds { get; set; }

        public bool Enabled { get; set; }

        public string Name { get; set; }

        public bool Selectable { get; }

        public int ZIndex { get; set; }


        public virtual void OnClick() { }

        public virtual void OnMouseDown() { }
        
        public virtual void OnMouseEnter() { }

        public virtual void OnMouseLeave() { }

        public virtual void OnMouseUp() { }
    }
}
