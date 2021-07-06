using System;
using System.Linq;

namespace ConsoleApp1
{
    public abstract class PlayerBase
    {
        public virtual int Guess()
        {
            // values are hardcoded since the exam note didn't mention otherwise
            return new Random().Next(40, 140);
        }

        public virtual int Cheat(int[] ignoreValues)
        {
            int num;

            do
            {
                num = Guess();
            } while (ignoreValues.Contains(num));

            return num;
        }

        public string Name { get; set; }
    }
}
