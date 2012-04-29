using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.BattleLogic
{
    public class HitGenerator : IHitGenerator
    {
        private readonly IDie _die = new Die();

        public HitGenerator()
        {

        }
        public HitGenerator(IDie die)
        {
            _die = die;
        }

        public int GenerateHits(Dictionary<CombatUnitType, int> units, Func<CombatUnitType,int> getCombatValue)
        {
            int hits = 0;

            foreach (var unitType in units)
            {
                int nSupportable = GetSupportableUnitsCount(units, unitType.Key, unitType.Value);
                
                for (int i = 0; i < unitType.Value; i++)
                {
                    int combatValue = getCombatValue(unitType.Key);

                    if (nSupportable > 0)
                    {
                        combatValue++;
                        nSupportable--;
                    }
                    
                    if (_die.Throw() <= combatValue)
                        hits ++;
                }
            }
            return hits;
        }

        private static int GetSupportableUnitsCount(Dictionary<CombatUnitType, int> units, CombatUnitType unitType, int unitCount)
        {
            if (unitType.SupportableByTypes == null)
                return 0;
            
            var supportTypes = units.Where(u => unitType.SupportableByTypes.Contains(u.Key.Type));
            
            if (supportTypes != null && supportTypes.Count() > 0)
                return Math.Min(supportTypes.Sum(u => u.Value), unitCount);

            return 0;
        }
    }
}
