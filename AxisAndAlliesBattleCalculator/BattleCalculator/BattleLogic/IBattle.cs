using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.BattleLogic
{
    public interface IBattle
    {
        Dictionary<CombatUnitType,int> Attackers { get; }
        Dictionary<CombatUnitType, int> SurvivingAttackers { get; }
        List<int> AttackerHits { get; }

        Dictionary<CombatUnitType, int> Defenders { get; }
        Dictionary<CombatUnitType, int> SurvivingDefenders { get; }
        List<int> DefenderHits { get; }

        int RoundCount { get; }

        void ResolveRound();

        void ResolveBattle();
    }
}
