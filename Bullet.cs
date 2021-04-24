using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonzolovaHra
{
    abstract class Bullet : ConsoleObject
    {
        public int FormerY;
        public bool IsShooting;

        public Bullet(int x, int y, string look, int formerY, bool isShooting) : base(x, y, look)
        {
            FormerY = formerY;
            IsShooting = isShooting;
        }

        public abstract void Shoot();
    }
}
