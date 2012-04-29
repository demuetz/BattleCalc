using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleCalculator.BattleLogic
{
    public class Die : IDie
    {
        private static Random _random = new Random();
        
        public int Throw()
        {
            return _random.Next(1, 7);
        }
    }
}
