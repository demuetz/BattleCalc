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
    public class HitGeneratorTests
    {
        private IDie _die;
        private IHitGenerator _hitGenerator;
        private Dictionary<CombatUnitType, int> _units;

        [SetUp]
        protected void SetUp()
        {
            _die = Substitute.For<IDie>();
            _hitGenerator = new HitGenerator(_die);
            _units = new Dictionary<CombatUnitType,int>() { {TestUtils.CreateInfantryType(), 1 } };
        }
        
        [Test]
        public void GenerateHits_1InfDefendingThrowOf2_Returns1()
        {
            _die.Throw().Returns(2);

            Assert.AreEqual(1, _hitGenerator.GenerateHits(_units, (u) => u.DefenseValue));
        }

        [Test]
        public void GenerateHits_1InfDefendingThrowOf3_Returns0()
        {
            _die.Throw().Returns(3);

            Assert.AreEqual(0, _hitGenerator.GenerateHits(_units, (u) => u.DefenseValue));
        }

        [Test]
        public void GenerateHits_1Inf1ArtAttackingAndDieThrowsOf2_Returns2()
        {
            _die.Throw().Returns(2);

            _units = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateInfantryType(), 1 }, { TestUtils.CreateArtilleryType(), 1 } };

            Assert.AreEqual(2, _hitGenerator.GenerateHits(_units, (u) => u.AttackValue), "Inf and Art should hit on 2");
        }

        [Test]
        public void GenerateHits_2Inf1ArtAttackingAndDieThrowsOf2_Returns2()
        {
            _die.Throw().Returns(2);

            _units = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateInfantryType(), 2 }, { TestUtils.CreateArtilleryType(), 1 } };

            Assert.AreEqual(2, _hitGenerator.GenerateHits(_units, (u) => u.AttackValue), "Art and one Inf should hit on 2, the other Inf on 1");
        }

        [Test]
        public void GenerateHits_1Tac1FighterAttackingAndDieThrowOf4_Returns1()
        {
            _die.Throw().Returns(4);

            _units = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateTacticalBomberType(), 1 }, { TestUtils.CreateFighterType(), 1 } };

            Assert.AreEqual(1, _hitGenerator.GenerateHits(_units, (u) => u.AttackValue), "Tac should hit on 4 with Fighter");
        }

        [Test]
        public void GenerateHits_1Tac1TankAttackingAndDieThrowOf4_Returns1()
        {
            _die.Throw().Returns(4);

            _units = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateTacticalBomberType(), 1 }, { TestUtils.CreateTankType(), 1 } };

            Assert.AreEqual(1, _hitGenerator.GenerateHits(_units, (u) => u.AttackValue), "Tac should hit on 4 with Tank");
        }

        [Test]
        public void GenerateHits_2Tacs1FighterAttackingAndDieThrowOf4_Returns1()
        {
            _die.Throw().Returns(4);

            _units = new Dictionary<CombatUnitType, int>() { { TestUtils.CreateTacticalBomberType(), 1 }, { TestUtils.CreateFighterType(), 1 } };

            Assert.AreEqual(1, _hitGenerator.GenerateHits(_units, (u) => u.AttackValue), "Only one Tac should hit on 4");
        }
    }
}
