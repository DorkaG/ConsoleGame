using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonzolovaHra
{
    class Enemy : Rival
    {
        public Enemy(int x, int y, string look, int formerX) : base(x, y, look, formerX)
        {
        }

        int positionCounter = 0;            //zajistuje, aby se nepritel pohyboval po urcitou dobu (treba 10x) jednim smerem
        int number = 0;

        public void MoveEnemy(Object stateInfo)
        {
            FormerX = X;

            if (positionCounter == 0)               //vygenerovani nahodneho smeru pohybu na zacatku a pak po kazdych 10 pohybech
            {
                Random randomNumber = new Random();
                number = randomNumber.Next(1, 3);
            }

            if (number == 1)
            {
                if (X < Console.WindowWidth-6) X++;
                else if (X == Console.WindowWidth-6) X = Console.WindowWidth - 7;

                if (positionCounter < 10) positionCounter++;
                else positionCounter = 0;
            }
            if (number == 2)
            {
                if (X >= 1) X--;
                else if (X < 1) X = 1;
                if (positionCounter < 10) positionCounter++;
                else positionCounter = 0;
            }

        }
    }
}
