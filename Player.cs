using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonzolovaHra
{
    class Player : Rival
    {
        public int Points;
        public int Life;
        public Player(int x, int y, string look, int formerX, int points, int life) : base(x, y, look, formerX)
        {
            Points = points;
            Life = life;
        }
        public void MovePlayer(int number)
        {
            FormerX = X;
            if (X <= Console.WindowWidth && X >= 1) X += number;
            else if (X > Console.WindowWidth) X = Console.WindowWidth;
            else if (X < 1) X = 1;
        }
    }
}
