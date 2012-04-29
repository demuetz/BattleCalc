using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.BattleLogic
{
    public interface IHitGenerator
    {
        int GenerateHits(Dictionary<CombatUnitType, int> units, Func<CombatUnitType, int> getCombatValue);
    }
}
