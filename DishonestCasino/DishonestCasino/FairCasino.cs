using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section2Casino
{
    public class FairCasino : ICasino
    {
        private Random random;

        public FairCasino()
        {
            this.random = new Random();
        }

        public double Simulate(int t)
        {
            double totalScore = 0;
            for (int i = 0; i < t; i++)
            {
                // Fair die: 1 to 6 with equal probability
                totalScore += random.Next(1, 7);
            }
            return totalScore / t;
        }
    }
}
