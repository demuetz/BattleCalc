using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using BattleCalculator.CombatUnits;

namespace BattleCalculator.Configuration
{
    public class UnitTypeDefinitionsSection : ConfigurationSection
    {
        public static UnitTypeDefinitionsSection Settings
        {
            get { return (UnitTypeDefinitionsSection)ConfigurationManager.GetSection("UnitTypeDefinitions"); }
        }
        

        [ConfigurationProperty("UnitTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(UnitTypeElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public UnitTypeElementCollection UnitTypeElements
        {
            get
            {
                return (UnitTypeElementCollection)base["UnitTypes"];
            }
        }
    }   
}
