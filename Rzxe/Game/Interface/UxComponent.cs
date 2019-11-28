﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oddmatics.Rzxe.Game.Interface
{
    public abstract class UxComponent
    {
        public RectangleF Bounds { get; set; }

        public bool Enabled { get; set; }

        public PointF Location { get; set; }

        public string Name { get; set; }

        public bool Selectable { get; }

        public SizeF Size { get; set; }

        public int ZIndex { get; set; }

        public UxComponent(string name, SizeF size, PointF location)
        {
            Name = name;
            Size = size;
            Location = location;
            Enabled = true;
            Selectable = true;
            Bounds = new RectangleF(Location, Size);
            ZIndex = 1;
        }
        public virtual void OnClick() { }

        public virtual void OnMouseDown() { }

        public virtual void OnMouseEnter()
        {
        }

        public virtual void OnMouseLeave()
        {
        }

        public virtual void OnMouseUp() { }
    }
}
