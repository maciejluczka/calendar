using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    internal abstract class Clickable
    {
        public int x, y, size;

        public abstract void onClick();
    }
}
