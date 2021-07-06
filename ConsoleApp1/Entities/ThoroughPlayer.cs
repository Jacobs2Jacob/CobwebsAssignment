using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ThoroughPlayer : PlayerBase
    {
        // values are hardcoded since the exam note didn't mention otherwise
        private int lastGuess = 40;

        public override int Guess()
        {
            return lastGuess < 140 ? lastGuess++ : 0;
        }
    }
}
