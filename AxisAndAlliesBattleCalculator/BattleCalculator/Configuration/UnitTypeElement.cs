using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.Configuration
{
    public class UnitTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey=true)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("attack", IsRequired = true, DefaultValue=1)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 6, MinValue = 0)]
        public int Attack
        {
            get { return (int)this["attack"]; }
            set { this["attack"] = value; }
        }

        [ConfigurationProperty("defense", IsRequired = true, DefaultValue = 1)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 6, MinValue = 1)]
        public int Defense
        {
            get { return (int)this["defense"]; }
            set { this["defense"] = value; }
        }

        [ConfigurationProperty("cost", IsRequired = true, DefaultValue = 1)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 30, MinValue = 1)]
        public int Cost
        {
            get { return (int)this["cost"]; }
            set { this["cost"] = value; }
        }

        [ConfigurationProperty("isReplacementType", IsRequired = false, DefaultValue = false)]
        public bool IsReplacementType
        {
            get { return (bool)this["isReplacementType"]; }
            set { this["isReplacementType"] = value; }
        }

        [ConfigurationProperty("attackerCasualtyRanking", IsRequired = true, DefaultValue = 1)]
        [IntegerValidator(ExcludeRange = false, MinValue = 1)]
        public int AttackerCasualtyRanking
        {
            get { return (int)this["attackerCasualtyRanking"]; }
            set { this["attackerCasualtyRanking"] = value; }
        }

        [ConfigurationProperty("defenderCasualtyRanking", IsRequired = true, DefaultValue = 1)]
        [IntegerValidator(ExcludeRange = false, MinValue = 1)]
        public int DefenderCasualtyRanking
        {
            get { return (int)this["defenderCasualtyRanking"]; }
            set { this["defenderCasualtyRanking"] = value; }
        }

        [ConfigurationProperty("supportableByTypes", IsRequired = false, DefaultValue = "")]
        public String SupportableByTypes
        {
            get { return (String)this["supportableByTypes"]; }
            set { this["supportableByTypes"] = value; }
        }

        [ConfigurationProperty("onHitReplaceWithType", IsRequired = false, DefaultValue = "")]
        public String OnHitReplaceWithType
        {
            get { return (String)this["onHitReplaceWithType"]; }
            set { this["onHitReplaceWithType"] = value; }
        }

        public CombatUnitType ToCombatUnitType()
        {
            var type = GetTypeForUnitName(Name);

            var res = new CombatUnitType(Attack, Defense, Cost, type);

            if (!String.IsNullOrEmpty(SupportableByTypes))
            {
                IList<ValidCombatUnitTypes> supportTypes = new List<ValidCombatUnitTypes>();
                
                foreach (var supportTypeName in SupportableByTypes.Split(','))
                    supportTypes.Add(GetTypeForUnitName(supportTypeName));

                res.SupportableByTypes = supportTypes;
            }

            if (!String.IsNullOrEmpty(OnHitReplaceWithType))
            {
                res.OnHitReplaceWithType = GetTypeForUnitName(OnHitReplaceWithType);
            }

            return res;
        }

        private ValidCombatUnitTypes GetTypeForUnitName(string name)
        {
            return (ValidCombatUnitTypes)Enum.Parse(typeof(ValidCombatUnitTypes), name.Replace(" ", ""));
        }
    }
}
