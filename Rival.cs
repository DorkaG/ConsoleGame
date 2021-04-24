using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonzolovaHra
{
    class Rival : ConsoleObject
    {
        public int FormerX;
        public Rival(int x, int y, string look, int formerX) : base(x, y, look)
        {
            FormerX = formerX;
        }
    }
}
