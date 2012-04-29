using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BattleCalculator.BattleLogic;
using BattleCalculator.CombatUnits;
using BattleCalculator.Configuration;
using BattleCalculator.Test;

namespace BlackBoxTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*UnitTypeDefinitionsSection section = UnitTypeDefinitionsSection.Settings;

            var unitType = section.UnitTypeElements[0];

            Console.WriteLine(unitType.Name + " " + unitType.Attack);

            foreach (var unit in section.UnitTypeElements.ToList())
            {
                Console.WriteLine(unit.Name);
            }

            Console.ReadKey();
            
            var attackers = new List<ICombatUnit> { TestUtils.CreateTanks(), TestUtils.CreateTanks() };
            var defenders = new List<ICombatUnit> { TestUtils.CreateInfantry(), TestUtils.CreateInfantry(), TestUtils.CreateInfantry() };
            var battle = new Battle(attackers, defenders, new HitGenerator(new AlwaysThrow3Die()), new CombatUnitComparerByCost());
            battle.ResolveBattle();

            Console.WriteLine(battle.RoundCount);
            Console.WriteLine(battle.SurvivingAttackers.Count);
            Console.WriteLine(battle.SurvivingDefenders.Count);
            */
            Console.ReadKey();

        }
    }

    public class AlwaysThrow3Die : IDie
    {
        public int Throw()
        {
            return 3;
        }
    }
}
