using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ThoroughCheaterPlayer : PlayerBase
    {
        // values are hardcoded since the exam note didn't mention otherwise
        private int lastGuess = 40;

        public override int Cheat(int[] ignoreValues)
        {
            int num;

            do
            {
                num = lastGuess++;
            } while (lastGuess < 140 && ignoreValues.Contains(lastGuess));

            return num;
        }
    }
}
