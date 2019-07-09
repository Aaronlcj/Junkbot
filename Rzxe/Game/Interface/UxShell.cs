using Oddmatics.Rzxe.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oddmatics.Rzxe.Game
{
    public class UxShell
    {
        protected Dictionary<int, List<UxComponent>> Components { get; set; }


        public UxShell()
        {
            Components = new Dictionary<int, List<UxComponent>>();
        }


        public bool HandleInputs(InputEvents inputs)
        {
            UxComponent component = MouseHitTest(inputs.MousePosition);

            if (component != null)
            {
                //
                // TODO: Do input handling here
                //

                return true;
            }

            return false;
        }


        private UxComponent MouseHitTest(PointF mousePos)
        {
            //
            // TODO: Run through UxComponents bounds, starting at the highest Z index
            //       until finding one that contains mousePos
            //

            return null;
        }
    }
}
