using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using BattleCalculator.BattleLogic;
using AxisAndAlliesBattleCalculator.ViewModel;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.Test
{
    [TestFixture]
    public class SimulationResultsViewModelTests
    {
        private IList<IBattle> _battleList;

        [SetUp]
        protected void Setup()
        {
            _battleList = new List<IBattle>() { Substitute.For<IBattle>(), Substitute.For<IBattle>()};
            _battleList[0].Attackers.Returns(GetListWith2Infantry());
            _battleList[0].Defenders.Returns(GetListWith2Infantry());
            _battleList[0].SurvivingAttackers.Returns(new Dictionary<CombatUnitType, int>());
            _battleList[0].SurvivingDefenders.Returns(new Dictionary<CombatUnitType, int>());
            _battleList[1].Attackers.Returns(GetListWith2Infantry());
            _battleList[1].Defenders.Returns(GetListWith2Infantry());
            _battleList[1].SurvivingAttackers.Returns(new Dictionary<CombatUnitType, int>());
            _battleList[1].SurvivingDefenders.Returns(new Dictionary<CombatUnitType, int>());
        }

        [Test]
        public void Update_TwoBattlesWithNoAttackerLosses_AttackerLossesHasOneEntry()
        {
            var vm = new SimulationResultsViewModel();
            _battleList[0].SurvivingAttackers.Returns(GetListWith2Infantry());
            _battleList[1].SurvivingAttackers.Returns(GetListWith2Infantry());

            vm.Update(_battleList);

            Assert.AreEqual(new Dictionary<int, double>() { {0,100.0} }, vm.AttackerLosses);
        }

        [Test]
        public void Update_TwoBattlesWithDifferentOutComes_AttackerLossesHasTwoEntries()
        {
            var vm = new SimulationResultsViewModel();
            _battleList[0].SurvivingAttackers.Returns(GetListWith2Infantry());

            vm.Update(_battleList);

            Assert.AreEqual(new Dictionary<int, double>() { { 6, 50.0 }, { 0, 50.0 } }, vm.AttackerLosses);
        }

        [Test]
        public void Update_TwoBattlesWithFewerLossesFirst_HigherLossesAreStillListedFirst()
        {
            var vm = new SimulationResultsViewModel();
            _battleList[1].SurvivingAttackers.Returns(GetListWith2Infantry());

            vm.Update(_battleList);

            Assert.AreEqual(new Dictionary<int, double>() { { 6, 50.0 }, { 0, 50.0 } }, vm.AttackerLosses);
        }

        private Dictionary<CombatUnitType, int> GetListWith2Infantry()
        {
            return new Dictionary<CombatUnitType, int>() { { TestUtils.CreateInfantryType(), 2 } };
        }
    }
}
