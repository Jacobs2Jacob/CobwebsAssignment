using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CheaterPlayer : PlayerBase
    {
        public override int Cheat(int[] ignoreValues)
        {
            return base.Cheat(ignoreValues);
        }
    }
}
