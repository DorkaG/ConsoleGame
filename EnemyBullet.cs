using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonzolovaHra
{
    class EnemyBullet : Bullet
    {
        public EnemyBullet(int x, int y, string look, int formerY, bool isShooting) : base(x, y, look, formerY, isShooting)
        {
        }
        public override void Shoot()
        {
            FormerY = Y;
            Y++;
            if (Y == Console.WindowHeight) IsShooting = false;
        }
    }
}
