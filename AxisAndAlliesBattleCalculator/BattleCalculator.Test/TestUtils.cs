using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.Test
{
    public static class TestUtils
    {
        public static CombatUnitType CreateInfantryType()
        {
            return new CombatUnitType(1, 2, 3, ValidCombatUnitTypes.Infantry) 
                        { SupportableByTypes = new List<ValidCombatUnitTypes> { ValidCombatUnitTypes.Artillery } };
        }

        public static CombatUnitType CreateTankType()
        {
            return new CombatUnitType(3, 3, 6, ValidCombatUnitTypes.Tank);
        }

        public static CombatUnitType CreateArtilleryType()
        {
            return new CombatUnitType(2, 2, 4, ValidCombatUnitTypes.Artillery);
        }

        public static CombatUnitType CreateFighterType()
        {
            return new CombatUnitType(3, 4, 10, ValidCombatUnitTypes.Fighter);
        }

        public static CombatUnitType CreateTacticalBomberType()
        {
            return new CombatUnitType(3, 3, 11, ValidCombatUnitTypes.TacticalBomber) 
                    { SupportableByTypes = new List<ValidCombatUnitTypes> { ValidCombatUnitTypes.Tank, ValidCombatUnitTypes.Fighter } };
        }

        public static CombatUnitType CreateBattleshipType()
        {
            return new CombatUnitType(4, 4, 20, ValidCombatUnitTypes.Battleship) { OnHitReplaceWithType = ValidCombatUnitTypes.DamagedBattleship };
        }

        public static IList<ValidCombatUnitTypes> CreateStandardCasualtyOrder()
        {
            return new List<ValidCombatUnitTypes>(){
                ValidCombatUnitTypes.Battleship,
                ValidCombatUnitTypes.Infantry,
                ValidCombatUnitTypes.Artillery,
                ValidCombatUnitTypes.Tank,
                ValidCombatUnitTypes.Fighter,
                ValidCombatUnitTypes.TacticalBomber
            };
        }

    }
}
