using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section2Casino
{
    public class DishonestCasino : ICasino
    {
        private double p1; // Prob. of staying with Fair die
        private double p2; // Prob. of staying with Loaded die
        private Random random;

        public DishonestCasino(double p1, double p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.random = new Random();
        }

        private int RollFair()
        {
            return random.Next(1, 7);
        }

        /// <summary>
        /// Loaded die logic:
        /// 1: 50% chance (0.5)
        /// 2-6: 10% chance each (0.1)
        /// </summary>
        /// <returns></returns>
        private int RollLoaded()
        {
            if (random.NextDouble() < 0.5)
            {
                return 1;
            }
            else
            {
                return random.Next(2, 7);
            }
        }


        public double Simulate(int t)
        {
            double totalScore = 0;

            // "The dishonest casino starts with one of the two dice at random"
            bool currentDieIsFair = (random.NextDouble() < 0.5);

            for (int i = 0; i < t; i++)
            {
                int roll;

                if (currentDieIsFair)
                {
                    roll = RollFair();
                    // Check switching logic after rolling Fair
                    // "roll the fair die again with a probability p1" 
                    if (random.NextDouble() >= p1)
                    {
                        currentDieIsFair = false; // Switch to Loaded
                    }
                }
                else
                {
                    roll = RollLoaded();
                    // Check switching logic after rolling Loaded
                    // "roll the unfair die again with a probability p2" 
                    if (random.NextDouble() >= p2)
                    {
                        currentDieIsFair = true; // Switch to Fair
                    }
                }

                totalScore += roll;
            }

            return totalScore / t;
        }
    }
}
