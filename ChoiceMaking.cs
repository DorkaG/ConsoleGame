using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonzolovaHra
{
    class ChoiceMaking
    {
        public Dictionary<int, Action> Functions;
        public ChoiceMaking(Dictionary<int, Action> functions)
        {
            Functions = functions;
        }
        public void Choose()
        {
            bool choiceDone = false;

            do
            {
                string choice = Console.ReadLine();
                int number;
                bool isNumber = int.TryParse(choice, out number);

                if (!isNumber || !Functions.ContainsKey(number))
                {
                    var keyList = Functions.Select(p => p.Key);
                    string keys = String.Join(", nebo ", keyList);
                    Console.WriteLine("Stiskněte " + keys + ".");
                }
                else
                {
                    choiceDone = true;
                    Functions[number]();                    
                }
            } while (!choiceDone);
        }
    }
}
