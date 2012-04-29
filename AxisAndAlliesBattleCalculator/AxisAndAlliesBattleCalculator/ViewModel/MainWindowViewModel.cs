using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using BattleCalculator.CombatUnits;
using BattleCalculator.Configuration;
using System.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using BattleCalculator.BattleLogic;
using AxisAndAlliesBattleCalculator.Model;
using System.Threading.Tasks;

namespace AxisAndAlliesBattleCalculator.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Private Fields
        private IList<UnitTypeElement> _unitTypesSelectableForBattle = UnitTypeDefinitionsSection.Settings.UnitTypeElements.ToList().Where(u => u.IsReplacementType == false).ToList();
        private ObservableCollection<UnitTypeWithCounts> _unitTypesInBattle;
        private string _unitsEvaluationText; 
        private string _quickFightResultText;
        private string _simulationResultText;
        private double _simulationProgress;
        private bool _isProgressBarVisible;
        private readonly BackgroundWorker _simulationWorker;
        private Dictionary<CombatUnitType,int> _currentAttackers;
        private Dictionary<CombatUnitType, int> _currentDefenders;
        private int _simulationRuns;
        private int _selectedAttackerCasualtyIndex;
        private IHitGenerator _hitGenerator;

        private SimulationResultsViewModel _simulationResultsViewModel;
        private CasualtyOrderViewModel _attackerCasualtyOrderViewModel;
        private CasualtyOrderViewModel _defenderCasualtyOrderViewModel;
        #endregion

        #region Properties
        public ObservableCollection<UnitTypeWithCounts> UnitTypesInBattle 
        {
            get { return _unitTypesInBattle; }
            set
            {
                if (value != _unitTypesInBattle)
                {
                    _unitTypesInBattle = value;
                    base.OnPropertyChanged("UnitTypesInBattle");
                }
            } 
        }

        public string UnitsEvaluationText
        {
            get { return _unitsEvaluationText; }
            set
            {
                if (value != _unitsEvaluationText)
                {
                    _unitsEvaluationText = value;
                    base.OnPropertyChanged("UnitsEvaluationText");
                }
            }
        }

        public string QuickFightResultText
        {
            get { return _quickFightResultText; }
            set
            {
                if (value != _quickFightResultText)
                {
                    _quickFightResultText = value;
                    base.OnPropertyChanged("QuickFightResultText");
                }
            }
        }

        public string SimulationResultText
        {
            get { return _simulationResultText; }
            set
            {
                if (value != _simulationResultText)
                {
                    _simulationResultText = value;
                    base.OnPropertyChanged("SimulationResultText");
                }
            }
        }

        public double SimulationProgress
        {
            get { return _simulationProgress; }
            set
            {
                if (value != _simulationProgress)
                {
                    _simulationProgress = value;
                    base.OnPropertyChanged("SimulationProgress");
                }
            }
        }

        public bool IsProgressBarVisible
        {
            get { return _isProgressBarVisible; }
            set
            {
                if (value != _isProgressBarVisible)
                {
                    _isProgressBarVisible = value;
                    base.OnPropertyChanged("IsProgressBarVisible");
                }
            }
        }

        public int SimulationRuns
        {
            get { return _simulationRuns; }
            set
            {
                if (value != _simulationRuns)
                {
                    _simulationRuns = value;
                    base.OnPropertyChanged("SimulationRuns");
                }
            }
        }

        public int SelectedAttackerCasualtyIndex
        {
            get { return _selectedAttackerCasualtyIndex; }
            set
            {
                if (value != _selectedAttackerCasualtyIndex)
                {
                    _selectedAttackerCasualtyIndex = value;
                    base.OnPropertyChanged("SelectedAttackerCasualtyIndex");
                }
            }
        }

        public SimulationResultsViewModel SimulationResultsViewModel
        {
            get { return _simulationResultsViewModel; }
            set
            {
                if (value != _simulationResultsViewModel)
                {
                    _simulationResultsViewModel = value;
                    base.OnPropertyChanged("SimulationResultsViewModel");
                }
            }
        }

        public CasualtyOrderViewModel AttackerCasualtyOrderViewModel
        {
            get { return _attackerCasualtyOrderViewModel; }
            set
            {
                if (value != _attackerCasualtyOrderViewModel)
                {
                    _attackerCasualtyOrderViewModel = value;
                    base.OnPropertyChanged("AttackerCasualtyOrderViewModel");
                }
            }
        }

        public CasualtyOrderViewModel DefenderCasualtyOrderViewModel
        {
            get { return _defenderCasualtyOrderViewModel; }
            set
            {
                if (value != _defenderCasualtyOrderViewModel)
                {
                    _defenderCasualtyOrderViewModel = value;
                    base.OnPropertyChanged("DefenderCasualtyOrderViewModel");
                }
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            _unitTypesInBattle = GetAvailableUnitTypes();
            _currentDefenders = new Dictionary<CombatUnitType, int>();
            _currentAttackers = new Dictionary<CombatUnitType, int>();

            _simulationWorker = new BackgroundWorker();
            _simulationWorker.DoWork += HandleSimulationWorkerDoWork; 
            _simulationWorker.ProgressChanged += HandleSimulationWorkerProgressChanged;
            _simulationWorker.WorkerReportsProgress = true;

            IsProgressBarVisible = false;
            SimulationRuns = 10000;
            _hitGenerator = new HitGenerator();

            _simulationResultsViewModel = new SimulationResultsViewModel();
            _attackerCasualtyOrderViewModel = new CasualtyOrderViewModel(GetInitialAttackerCasualtyOrder());
            _defenderCasualtyOrderViewModel = new CasualtyOrderViewModel(GetInitialDefenderCasualtyOrder());
        }


        #region Commands

        RelayCommand _evaluateUnitsCommand;
        public ICommand EvaluateUnitsCommand
        {
            get
            {
                if (_evaluateUnitsCommand == null)
                {
                    _evaluateUnitsCommand = new RelayCommand(param => this.UpdateUnitEvaluationText(),
                        param => this.CanEvaluateUnits);
                }
                return _evaluateUnitsCommand;
            }
        }

        RelayCommand _clearUnitsCommand;
        public ICommand ClearUnitsCommand
        {
            get
            {
                if (_clearUnitsCommand == null)
                    _clearUnitsCommand = new RelayCommand(param => this.ClearAllUnits());
                return _clearUnitsCommand;
            }
        }

        RelayCommand _swapUnitsCommand;
        public ICommand SwapUnitsCommand
        {
            get
            {
                if (_swapUnitsCommand == null)
                    _swapUnitsCommand = new RelayCommand(param => this.SwapUnits());
                return _swapUnitsCommand;
            }
        }

        RelayCommand _resolveCombatCommand;
        public ICommand ResolveCombatCommand
        {
            get
            {
                if (_resolveCombatCommand == null)
                {
                    _resolveCombatCommand = new RelayCommand(param => this.ResolveQuickFight(),
                        param => this.CanEvaluateUnits);
                }
                return _resolveCombatCommand;
            }
        }

        RelayCommand _simulateCombatCommand;
        public ICommand SimulateCombatCommand
        {
            get
            {
                if (_simulateCombatCommand == null)
                    _simulateCombatCommand = new RelayCommand(o => SimulateCombat(), o => !_simulationWorker.IsBusy && CanEvaluateUnits);
                return _simulateCombatCommand;
            }
        }

        
        #endregion

        #region Private Methods

        private void ClearAllUnits()
        {
            foreach (var unitType in UnitTypesInBattle)
            {
                unitType.AttackerCount = 0;
                unitType.DefenderCount = 0;
            }
        }

        private void SwapUnits()
        {
            foreach (var unitType in UnitTypesInBattle)
            {
                int tmpCount = unitType.AttackerCount;
                unitType.AttackerCount = unitType.DefenderCount;
                unitType.DefenderCount = tmpCount;
            }
        }

        private void SimulateCombat()
        {
            RefreshCurrentUnits();
            UpdateUnitEvaluationText();
            
            _simulationWorker.RunWorkerAsync();
        }

        private void HandleSimulationWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            IsProgressBarVisible = true;
            SimulationResultText = "Calculating...";

            IList<IBattle> battleList = new List<IBattle>();

            for (int i = 1; i <= _simulationRuns; i++)
            {
                battleList.Add(ResolveBattleForCurrentUnits());

                (sender as BackgroundWorker).ReportProgress((int)(100.0 / _simulationRuns * i));
            }

            _simulationResultsViewModel.Update(battleList);
            SimulationResultText = CreateSimulationResultText(battleList);
            IsProgressBarVisible = false;
        }

        private string CreateSimulationResultText(IList<IBattle> battleList)
        {
            double attackerWins = (double) battleList.Count(b => b.SurvivingAttackers.Count > 0 && b.SurvivingDefenders.Count == 0) / battleList.Count * 100;
            double defenderWins = (double)battleList.Count(b => b.SurvivingDefenders.Count > 0 && b.SurvivingAttackers.Count == 0) / battleList.Count * 100;
            double allDead = (double)battleList.Count(b => b.SurvivingAttackers.Count == 0 && b.SurvivingDefenders.Count == 0) / battleList.Count * 100;
            double avgRounds = battleList.Average(b => b.RoundCount);

            return String.Format("Attacker wins: {1:0.##}%{0}Defender wins: {2:0.##}%{0}All dead: {3:0.##}%{0}Rounds avg.: {4:0.##}{0}Simulation runs: {5}",
                Environment.NewLine,
                attackerWins,
                defenderWins,
                allDead,
                avgRounds,
                _simulationRuns);
        }

        private void HandleSimulationWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.SimulationProgress = e.ProgressPercentage;
        }

        private void ResolveQuickFight()
        {
            RefreshCurrentUnits();
            
            var battle = ResolveBattleForCurrentUnits();

            string winner ;
            if (battle.SurvivingAttackers.Count > 0 && battle.SurvivingDefenders.Count == 0)
                winner = "Attacker";
            else if (battle.SurvivingDefenders.Count > 0)
                winner = "Defender";
            else
                winner = "All dead!";

            string result = String.Format("Winner: {1}{0}Rounds: {2}{0}Surviving Attackers: {3}{0}Surviving Defenders: {4}",
                Environment.NewLine,
                winner,
                battle.RoundCount,
                battle.SurvivingAttackers.Count,
                battle.SurvivingDefenders.Count);

            UpdateUnitEvaluationText();
            QuickFightResultText = result;
        }

        private Battle ResolveBattleForCurrentUnits()
        {
            var attackerCasualtyOrder = _attackerCasualtyOrderViewModel.CasualtyOrder.Select(u => u.ToCombatUnitType().Type).ToList();
            var defenderCasualtyOrder = _defenderCasualtyOrderViewModel.CasualtyOrder.Select(u => u.ToCombatUnitType().Type).ToList();

            var battle = new Battle(_currentAttackers, _currentDefenders, attackerCasualtyOrder, defenderCasualtyOrder, _hitGenerator);

            battle.ResolveBattle();
            return battle;
        }

        private void RefreshCurrentUnits()
        {
            _currentAttackers = GetCurrentUnits(_attackerCasualtyOrderViewModel.CasualtyOrder, (u) => u.AttackerCount);

            _currentDefenders = GetCurrentUnits(_defenderCasualtyOrderViewModel.CasualtyOrder, (u) => u.DefenderCount);
        }

        private Dictionary<CombatUnitType, int> GetCurrentUnits(IList<UnitTypeElement> casualtyOrder, Func<UnitTypeWithCounts, int> getUnitCount)
        {
            var units = new Dictionary<CombatUnitType,int>();
            
            var relevantCasualtyOrder = casualtyOrder
                                .Where(u => _unitTypesInBattle
                                                .Where(ub => getUnitCount(ub) > 0)
                                                .Select(ub => ub.UnitName)
                                                .Contains(u.Name));

            foreach (var unitType in relevantCasualtyOrder)
            {
                int unitCount = getUnitCount(_unitTypesInBattle.Single(u => u.UnitName == unitType.Name));

                if (unitCount > 0)
                    units.Add(
                        unitType.ToCombatUnitType(),
                        unitCount);
            }
            return units;
        }

        private void UpdateUnitEvaluationText()
        {
            RefreshCurrentUnits();
            
            UnitsEvaluationText = 
                String.Format("Units: {1} vs. {2}{0}Punch: {3} vs. {4}{0}Cost: {5} vs. {6}",
                                Environment.NewLine,
                                _currentAttackers.Sum(u => u.Value),
                                _currentDefenders.Sum(u => u.Value),
                                _currentAttackers.Sum(u => u.Value * u.Key.AttackValue),// + _hitGenerator.GetSupportableInfCount(_currentAttackers) + _hitGenerator.GetSupportableTacCount(_currentAttackers),
                                _currentDefenders.Sum(u => u.Value * u.Key.DefenseValue),
                                _currentAttackers.Sum(u => u.Value * u.Key.Cost),
                                _currentDefenders.Sum(u => u.Value * u.Key.Cost)
                                );
        }

        /// <summary>
        /// Returns true if units for battle have been entered and can be evaluated.
        /// </summary>
        private bool CanEvaluateUnits
        {
            get { return _unitTypesInBattle.Sum(u => u.AttackerCount) > 0 && _unitTypesInBattle.Sum(u => u.DefenderCount) > 0; }
        }

        private ObservableCollection<UnitTypeWithCounts> GetAvailableUnitTypes()
        {
            return new ObservableCollection<UnitTypeWithCounts>(
                _unitTypesSelectableForBattle.Select(unitType => 
                    new UnitTypeWithCounts { UnitName = unitType.Name, Attack=unitType.Attack, Defense=unitType.Defense, Cost=unitType.Cost}));
        }

        private ObservableCollection<UnitTypeElement> GetInitialAttackerCasualtyOrder()
        {
            return new ObservableCollection<UnitTypeElement>(
                UnitTypeDefinitionsSection.Settings.UnitTypeElements.ToList().OrderBy(u => u.AttackerCasualtyRanking));
        }

        private ObservableCollection<UnitTypeElement> GetInitialDefenderCasualtyOrder()
        {
            return new ObservableCollection<UnitTypeElement>(
                UnitTypeDefinitionsSection.Settings.UnitTypeElements.ToList().OrderBy(u => u.DefenderCasualtyRanking));
        }
        #endregion
    }
}
