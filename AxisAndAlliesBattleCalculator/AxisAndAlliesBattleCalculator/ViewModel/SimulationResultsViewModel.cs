using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using AxisAndAlliesBattleCalculator.Model;
using BattleCalculator.BattleLogic;
using System.Windows;
using BattleCalculator.CombatUnits;

namespace AxisAndAlliesBattleCalculator.ViewModel
{
    public class SimulationResultsViewModel : ViewModelBase
    {
        private IList<IBattle> _battleList;
        
        public Dictionary<int, double> AttackerLosses { get; private set; }
        public Dictionary<int, double> DefenderLosses { get; private set; }

        public bool ResultsAvailable { get; private set; }

        private KeyValuePair<int, double> _selectedAttackerOutcome;
        public KeyValuePair<int, double> SelectedAttackerOutcome
        {
            get { return _selectedAttackerOutcome; }
            set
            {
                if (value.Key != _selectedAttackerOutcome.Key || value.Value != _selectedAttackerOutcome.Value)
                {
                    _selectedAttackerOutcome = value;
                    UpdateSelectedAttackerOutcomeText(value.Key, value.Value);
                    base.OnPropertyChanged("SelectedAttackerOutcome");
                }
            }
        }

        private void UpdateSelectedAttackerOutcomeText(int casualtyCost, double outcomePercentage)
        {
            double cumulatedPercentage = AttackerLosses.Where(l => l.Key <= casualtyCost).Sum(l => l.Value);

            var survivors = _battleList.First(b => GetAttackerLossesForBattle(b) == casualtyCost).SurvivingAttackers;

            var unitsText = new StringBuilder();

            if (survivors.Count > 0)
            {
                unitsText.AppendLine("Survivors:");

                foreach (var unitType in survivors)
                {
                    unitsText.AppendLine(" - " + unitType.Key.Type.ToString() + ": " + unitType.Value);
                }
            }
            else
                unitsText.AppendLine("No survivors.");

            MessageBox.Show("Percentage: " + outcomePercentage + Environment.NewLine + 
                    "Cumulated Percentage: " + cumulatedPercentage + Environment.NewLine + unitsText);
        }

        public SimulationResultsViewModel()
        {
        }

        public void Update(IList<IBattle> battleList)
        {
            _battleList = battleList;
            

            AttackerLosses = battleList.Select(b => GetAttackerLossesForBattle(b))
                .GroupBy(i => i).OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => GetPercentage(g.Count(), battleList.Count));
            base.OnPropertyChanged("AttackerLosses");

            DefenderLosses = battleList.Select(b => GetDefenderLossesForBattle(b))
                .GroupBy(i => i).OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => GetPercentage(g.Count(), battleList.Count));
            base.OnPropertyChanged("DefenderLosses");

            ResultsAvailable = true;
            base.OnPropertyChanged("ResultsAvailable");
        }

        private int GetAttackerLossesForBattle(IBattle battle)
        {
            return battle.Attackers.Sum(a => a.Key.Cost * a.Value) - battle.SurvivingAttackers.Sum(a => a.Key.Cost * a.Value);
        }

        private int GetDefenderLossesForBattle(IBattle battle)
        {
            return battle.Defenders.Sum(a => a.Key.Cost * a.Value) - battle.SurvivingDefenders.Sum(a => a.Key.Cost * a.Value);
        }

        private Dictionary<CombatUnitType, int> GetUnitDifference(Dictionary<CombatUnitType, int> baseUnits, Dictionary<CombatUnitType, int> unitsToSubtract)
        {
            var unitDifference = new Dictionary<CombatUnitType, int>();

            foreach (var unitType in baseUnits.Where(u => u.Value > 0))
            {
                if (!unitsToSubtract.ContainsKey(unitType.Key))
                    unitDifference.Add(unitType.Key, unitType.Value);
                else if (unitsToSubtract[unitType.Key] - unitType.Value > 0)
                    unitDifference.Add(unitType.Key, unitsToSubtract[unitType.Key] - unitType.Value);
            }
            return unitDifference;
        }

        private double GetPercentage(int frequency, int total)
        { 
            return (double) frequency / total * 100;
        }
    }
}
