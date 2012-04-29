using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BattleCalculator.Configuration;


namespace AxisAndAlliesBattleCalculator.ViewModel
{
    public class CasualtyOrderViewModel : ViewModelBase
    {
        private ObservableCollection<UnitTypeElement> _casualtyOrder;

        public ObservableCollection<UnitTypeElement> CasualtyOrder
        {
            get { return _casualtyOrder; }
            set
            {
                if (value != _casualtyOrder)
                {
                    _casualtyOrder = value;
                    base.OnPropertyChanged("CasualtyOrder");
                }
            }
        }

        #region Commands

        RelayCommand _moveUnitUpInCasualtyOrderCommand;
        public ICommand MoveUnitUpInCasualtyOrderCommand
        {
            get
            {
                if (_moveUnitUpInCasualtyOrderCommand == null)
                {
                    _moveUnitUpInCasualtyOrderCommand = new RelayCommand(this.MoveUnitUpInCasualtyOrder);
                }
                return _moveUnitUpInCasualtyOrderCommand;
            }
        }

        RelayCommand _moveUnitDownInCasualtyOrderCommand;
        public ICommand MoveUnitDownInCasualtyOrderCommand
        {
            get
            {
                if (_moveUnitDownInCasualtyOrderCommand == null)
                {
                    _moveUnitDownInCasualtyOrderCommand = new RelayCommand(this.MoveUnitDownInCasualtyOrder);
                }
                return _moveUnitDownInCasualtyOrderCommand;
            }
        }
        #endregion

        public CasualtyOrderViewModel(ObservableCollection<UnitTypeElement> initialOrder)
        {
            _casualtyOrder = initialOrder;
        }

        private void MoveUnitUpInCasualtyOrder(object unitType)
        {
            int currentIndex = CasualtyOrder.IndexOf(unitType as UnitTypeElement);

            if (currentIndex > 0)
                CasualtyOrder.Move(currentIndex, currentIndex - 1);
        }

        private void MoveUnitDownInCasualtyOrder(object unitType)
        {
            int currentIndex = CasualtyOrder.IndexOf(unitType as UnitTypeElement);

            if (currentIndex < CasualtyOrder.Count -1)
                CasualtyOrder.Move(currentIndex, currentIndex + 1);
        }
    }
}
