using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveWindowLib
{
   public class MouseInfo : IOperation
    {
       public int X { get; set; }
       public int Y { get; set; }
       public override string ToString()
       {
           return X + "|" + Y;
       }

       public enum MouseStatus
       {
           Moving,
           NotMoving
       }
    }
}
