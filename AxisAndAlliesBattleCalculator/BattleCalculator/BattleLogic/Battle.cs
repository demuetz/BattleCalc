using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.BattleLogic
{
    public class Battle : IBattle
    {
        private readonly IHitGenerator _hitGenerator;

        public Dictionary<CombatUnitType, int> Attackers { get; private set; }
        public Dictionary<CombatUnitType, int> SurvivingAttackers { get; private set; }
        public List<int> AttackerHits { get; private set; }
        public IList<ValidCombatUnitTypes> AttackerCasualtyOrder { get; private set; }

        public Dictionary<CombatUnitType, int> Defenders { get; private set; }
        public Dictionary<CombatUnitType, int> SurvivingDefenders { get; private set; }
        public List<int> DefenderHits { get; private set; }
        public IList<ValidCombatUnitTypes> DefenderCasualtyOrder { get; private set; }

        public int RoundCount { get; private set; }




        public Battle(  
            IDictionary<CombatUnitType, int> attackers, 
            IDictionary<CombatUnitType, int> defenders, 
            IList<ValidCombatUnitTypes> attackerCasualtyOrder,
            IList<ValidCombatUnitTypes> defenderCasualtyOrder,
            IHitGenerator hitGenerator)
        {
            if (attackers == null || attackers.Count == 0 || attackers.Values.Sum() == 0)
                throw new ArgumentException("attackers");
            if (defenders == null || defenders.Count == 0 || defenders.Values.Sum() == 0)
                throw new ArgumentException("defenders");
            if (attackerCasualtyOrder == null || attackerCasualtyOrder.Count == 0)
                throw new ArgumentException("attackerCasualtyOrder");
            if (defenderCasualtyOrder == null || defenderCasualtyOrder.Count == 0)
                throw new ArgumentException("defenderCasualtyOrder");
            
            Attackers = new Dictionary<CombatUnitType,int>(attackers);
            SurvivingAttackers = new Dictionary<CombatUnitType, int>(attackers);
            AttackerCasualtyOrder = attackerCasualtyOrder;

            Defenders =  new Dictionary<CombatUnitType,int>(defenders);
            SurvivingDefenders = new Dictionary<CombatUnitType, int>(defenders);
            DefenderCasualtyOrder = defenderCasualtyOrder;

            AttackerHits = new List<int>();
            DefenderHits = new List<int>();
            RoundCount = 0;
            _hitGenerator = hitGenerator;
        }
        
        public void ResolveRound()
        {
            int attackerHitsForRound = _hitGenerator.GenerateHits(SurvivingAttackers, (u) => u.AttackValue);
            int defenderHitsForRound = _hitGenerator.GenerateHits(SurvivingDefenders, (u) => u.DefenseValue);

            AttackerHits.Add(attackerHitsForRound);
            DefenderHits.Add(defenderHitsForRound);

            RemoveCasualtiesInOrderOfList(SurvivingDefenders, DefenderCasualtyOrder, attackerHitsForRound);
            RemoveCasualtiesInOrderOfList(SurvivingAttackers, AttackerCasualtyOrder, defenderHitsForRound);

            RoundCount++;
        }

        private void RemoveCasualtiesInOrderOfList(Dictionary<CombatUnitType, int> units, IList<ValidCombatUnitTypes> casualtyOrder, int casualtiesToRemove)
        {
            while (casualtiesToRemove > 0 && units.Count > 0)
            {
                var unitTypeToReduce = units.OrderBy(u => casualtyOrder.IndexOf(u.Key.Type)).First();

                if (casualtiesToRemove <= unitTypeToReduce.Value)
                {
                    units[unitTypeToReduce.Key] -= casualtiesToRemove;

                    if (unitTypeToReduce.Key.OnHitReplaceWithType != null)
                        units.Add(unitTypeToReduce.Key.ToReplacementType(), casualtiesToRemove);

                    if (units[unitTypeToReduce.Key] == 0)
                        units.Remove(unitTypeToReduce.Key);

                    casualtiesToRemove = 0;
                }
                else
                {
                    casualtiesToRemove -= unitTypeToReduce.Value;

                    if (unitTypeToReduce.Key.OnHitReplaceWithType != null)
                        units.Add(unitTypeToReduce.Key.ToReplacementType(), unitTypeToReduce.Value);

                    units.Remove(unitTypeToReduce.Key);
                }
            }
        }

        public void ResolveBattle()
        {
            while (SurvivingAttackers.Values.Sum() > 0 && SurvivingDefenders.Values.Sum() > 0)
            {
                ResolveRound();
            }
        }
    }
}
