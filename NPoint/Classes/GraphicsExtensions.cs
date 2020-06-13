//---------------------------------------------------------------------
// Copyright © 2020 Phil Crompton
// based on original work
// Copyright © 2006 Raymund Sarmiento
//
// Permission is hereby granted to use this Software for any purpose
// including combining with commercial products, creating derivative
// works, and redistribution of source or binary code, without
// limitation or consideration. Any redistributed copies of this
// Software must include the above Copyright Notice.
//
// THIS SOFTWARE IS PROVIDED "AS IS". THE AUTHOR OF THIS CODE MAKES NO
// WARRANTIES REGARDING THIS SOFTWARE, EXPRESS OR IMPLIED, AS TO ITS
// SUITABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
//---------------------------------------------------------------------
using System.Drawing;
using System.Windows.Forms;

namespace NPoint
{
    public static class GraphicsExtensions
   {
      public static void Circle(this Form control, float centerX, float centerY, float radius, Color color)
      {
         using (Graphics g = control.CreateGraphics())
         using (Pen pen = new Pen(color))
         {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);

         }
      }
   }
}
