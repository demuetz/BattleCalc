using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using BattleCalculator.CombatUnits;
using BattleCalculator.BattleLogic;

namespace BattleCalculator.Test
{
    
    [TestFixture]
    public class BattleTests
    {
        private Dictionary<CombatUnitType, int> _attackers;
        private Dictionary<CombatUnitType, int> _defenders;
        private IList<ValidCombatUnitTypes> _attackerCasualtyOrder;
        private IList<ValidCombatUnitTypes> _defenderCasualtyOrder;
        private Battle _battle;
        private IHitGenerator _hitGenerator;

        [SetUp]
        protected void Setup()
        {
            _attackers = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateTankType(), 2 }, { TestUtils.CreateInfantryType(), 1 } };
            _defenders = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateTankType(), 1 }, { TestUtils.CreateInfantryType(), 1 } };
            _attackerCasualtyOrder = TestUtils.CreateStandardCasualtyOrder();
            _defenderCasualtyOrder = TestUtils.CreateStandardCasualtyOrder();
            _hitGenerator = Substitute.For<IHitGenerator>();
            _battle = new Battle(_attackers, _defenders, _attackerCasualtyOrder, _defenderCasualtyOrder, _hitGenerator);
        }

        [Test]
        public void ResolveRound_Called_IncrementsCounter()
        {
            int roundCountBefore = _battle.RoundCount;
            _battle.ResolveRound();

            Assert.AreEqual(roundCountBefore + 1, _battle.RoundCount);
        }

        [Test]
        public void AttackerHits_AfterConstruction_CountEquals0()
        {
            Assert.AreEqual(0, _battle.AttackerHits.Count);
        }

        [Test]
        public void ResolveRound_Called_AttackerHitsAppended()
        {
            _battle.ResolveRound();

            Assert.AreEqual(1, _battle.AttackerHits.Count);
        }

        [Test]
        public void ResolveRound_2AttackersHit_AttackerHitsForRoundIs2()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType,int>>())
                .Returns(2, 0);

            _battle.ResolveRound();

            Assert.AreEqual(2, _battle.AttackerHits[0]);
        }

        [Test]
        public void ResolveRound_1DefenderHitsAndOneInfantryIsLowestRankingAttacker_AttackingInfantryRemoved()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(0, 1);

            _battle.ResolveRound();

            Assert.AreEqual(0, _battle.SurvivingAttackers.Count( u => u.Key.Type == ValidCombatUnitTypes.Infantry));
            Assert.AreEqual(2, _battle.SurvivingAttackers.Where(u => u.Key.Type == ValidCombatUnitTypes.Tank).Sum(u => u.Value));
        }

        [Test]
        public void ResolveRound_1DefenderHitsAndOneTankIsLowestRankingAttacker_AttackingTankRemoved()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(0, 1);

            _attackers = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateInfantryType(), 2 }, { TestUtils.CreateTankType(), 1 } };
            _attackerCasualtyOrder = new List<ValidCombatUnitTypes> { ValidCombatUnitTypes.Tank, ValidCombatUnitTypes.Infantry };

            _battle = new Battle(_attackers, _defenders, _attackerCasualtyOrder, _defenderCasualtyOrder, _hitGenerator);

            _battle.ResolveRound();

            Assert.AreEqual(0, _battle.SurvivingAttackers.Count(u => u.Key.Type == ValidCombatUnitTypes.Tank));
            Assert.AreEqual(2, _battle.SurvivingAttackers.Where(u => u.Key.Type == ValidCombatUnitTypes.Infantry).Sum(u => u.Value));
        }

        [Test]
        public void ResolveRound_2DefendersHit_2LowestRankingSurvivingAttackersRemoved()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(0, 2);

            _battle.ResolveRound();

            Assert.AreEqual(1, _battle.SurvivingAttackers.Sum(u => u.Value));
            Assert.AreEqual(ValidCombatUnitTypes.Tank, _battle.SurvivingAttackers.First().Key.Type);
        }

        [Test]
        public void ResolveBattle_AllDefendersKilledInFirstRound_RoundCountEquals1()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(10, 0);

            _battle.ResolveBattle();

            Assert.AreEqual(1, _battle.RoundCount);
        }

        [Test]
        public void ResolveBattle_AllDefendersKilledInSecondRound_RoundCountEquals2()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(1, 0, 1, 0);

            _battle.ResolveBattle();

            Assert.AreEqual(2, _battle.RoundCount);
        }

        [Test]
        public void ResolveBattle_AllDefendersKilled_SurvivingDefendersIsEmpty()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(10, 0);

            _battle.ResolveBattle();

            Assert.AreEqual(0, _battle.SurvivingDefenders.Count);
        }

        [Test]
        public void ResolveBattle_BattleshipHitOnce_ReplacedWithDamagedBattleship()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(1);

            _attackers = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateBattleshipType(), 1 } };
            _defenders = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateFighterType(), 1 } };

            _battle = new Battle(_attackers, _defenders, _attackerCasualtyOrder, _defenderCasualtyOrder, _hitGenerator);

            _battle.ResolveBattle();

            Assert.AreEqual(1, _battle.SurvivingAttackers.Count);
            Assert.AreEqual(ValidCombatUnitTypes.DamagedBattleship, _battle.SurvivingAttackers.First().Key.Type);
        }

        [Test]
        public void ResolveBattle_OneOf2BattleshipsTakesHit_OneBattleshipAndOneDamagedBattleshipRemain()
        {
            _hitGenerator.GenerateHits(Arg.Any<Dictionary<CombatUnitType, int>>(), Arg.Any<Func<CombatUnitType, int>>())
                .Returns(1);

            _attackers = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateBattleshipType(), 2 } };
            _defenders = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateFighterType(), 1 } };

            _battle = new Battle(_attackers, _defenders, _attackerCasualtyOrder, _defenderCasualtyOrder, _hitGenerator);

            _battle.ResolveBattle();

            Assert.AreEqual(2, _battle.SurvivingAttackers.Count);
            Assert.AreEqual(2, _battle.SurvivingAttackers.Values.Sum());
            Assert.AreEqual(1, _battle.SurvivingAttackers.Count(a => a.Key.Type == ValidCombatUnitTypes.Battleship));
            Assert.AreEqual(1, _battle.SurvivingAttackers.Count(a => a.Key.Type == ValidCombatUnitTypes.DamagedBattleship));
        }
    }
}
