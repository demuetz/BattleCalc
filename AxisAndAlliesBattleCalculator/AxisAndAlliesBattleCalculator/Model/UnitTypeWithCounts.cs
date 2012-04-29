using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BattleCalculator.CombatUnits;

namespace AxisAndAlliesBattleCalculator.Model
{
    public class UnitTypeWithCounts : DependencyObject
    {

        public string UnitName
        {
            get { return (string)GetValue(UnitNameProperty); }
            set { SetValue(UnitNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitNameProperty =
            DependencyProperty.Register("UnitName", typeof(string), typeof(UnitTypeWithCounts), new UIPropertyMetadata());


        public int AttackerCount
        {
            get { return (int)GetValue(AttackerCountProperty); }
            set { SetValue(AttackerCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttackerCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttackerCountProperty =
            DependencyProperty.Register("AttackerCount", typeof(int), typeof(UnitTypeWithCounts), new UIPropertyMetadata(0));

        public int DefenderCount
        {
            get { return (int)GetValue(DefenderCountProperty); }
            set { SetValue(DefenderCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefenderCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefenderCountProperty =
            DependencyProperty.Register("DefenderCount", typeof(int), typeof(UnitTypeWithCounts), new UIPropertyMetadata(0));

        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Cost { get; set; }
        
    }
}
