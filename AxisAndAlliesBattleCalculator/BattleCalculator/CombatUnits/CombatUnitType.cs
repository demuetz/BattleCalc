using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleCalculator.CombatUnits
{
    public class CombatUnitType
    {
        public virtual int AttackValue { get; set; }
        public virtual int DefenseValue { get; set; }
        public virtual int Cost { get; set; }
        public virtual ValidCombatUnitTypes Type { get; set; }

        public virtual IEnumerable<ValidCombatUnitTypes> SupportableByTypes { get; set; }

        public virtual ValidCombatUnitTypes? OnHitReplaceWithType { get; set; }

        public virtual  bool CanBombardOnceOnInvasion { get; set; }

        public virtual bool CanSurpriseAttack { get; set; }
        public virtual IEnumerable<ValidCombatUnitTypes> SurpriseAttackNegatedByTypes { get; set; }

        public virtual bool CanRetreatEarly { get; set; }
        public virtual IEnumerable<ValidCombatUnitTypes> RetreatEarlyNegatedByTypes { get; set; }


        public CombatUnitType(int attackValue, int defenseValue, int cost, ValidCombatUnitTypes type)
        {
            AttackValue = attackValue;
            DefenseValue = defenseValue;
            Cost = cost;
            Type = type;
        }


        public override string ToString()
        {
            return String.Format("{0}: A:{1}, D:{2}, C:{3}", Type, AttackValue, DefenseValue, Cost);
        }

        public CombatUnitType ToReplacementType()
        {
            if (OnHitReplaceWithType == null)
                throw new InvalidOperationException("This type has no replacement type.");

            return new CombatUnitType(AttackValue, DefenseValue, Cost, (ValidCombatUnitTypes)OnHitReplaceWithType) 
            { 
                SupportableByTypes = this.SupportableByTypes,
                CanBombardOnceOnInvasion = this.CanBombardOnceOnInvasion,
                CanSurpriseAttack = this.CanSurpriseAttack,
                SurpriseAttackNegatedByTypes = this.SurpriseAttackNegatedByTypes,
                CanRetreatEarly = this.CanRetreatEarly,
                RetreatEarlyNegatedByTypes = this.RetreatEarlyNegatedByTypes
            };
        }
    }
}
