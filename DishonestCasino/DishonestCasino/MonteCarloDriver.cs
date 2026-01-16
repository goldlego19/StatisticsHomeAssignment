using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section2Casino
{
    public class MonteCarloDriver
    {
        private ICasino _casino;
        private int _t;

        public MonteCarloDriver(ICasino casino, int t)
        {
            _casino = casino;
            _t = t;
        }

        /// <summary>
        /// Simulate n times, using the Simulator strategy passed in the constructor
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<double> Simulate(int n)
        {
            List<double> results = new List<double>(n);

            for (int i = 0; i < n; i++)
            {
                // generate y_i and add it to the list of results
                results.Add(_casino.Simulate(_t));
            }

            return results;
        }

        /// <summary>
        /// Return an estimate of expected value of Random Variable, by running the simulation n times
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public double EstimateExpectedValue(int n)
        {
            List<double> results = Simulate(n);

            if (n != results.Count)
            {
                throw new ApplicationException("Number of simulations is not equal to n!");
            }

            return results.Sum() / results.Count();
        }
    }
}
