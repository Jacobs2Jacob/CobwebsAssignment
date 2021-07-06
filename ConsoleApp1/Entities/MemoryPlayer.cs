using System.Collections.Generic;

namespace ConsoleApp1
{
    public class MemoryPlayer : PlayerBase
    {
        private List<int> memory = new List<int>();

        public override int Guess()
        {
            int num;

            do
            {
                num = base.Guess();
            } while (memory.Contains(num));

            memory.Add(num);
            return num;
        }
    }
}
