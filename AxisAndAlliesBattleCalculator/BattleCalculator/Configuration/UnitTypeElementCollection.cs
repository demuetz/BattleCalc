using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BattleCalculator.Configuration
{
    public class UnitTypeElementCollection : ConfigurationElementCollection
    {
        #region Indexers
        public UnitTypeElement this[int index]
        {
            get { return (UnitTypeElement)base.BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new UnitTypeElement this[string name]
        {
            get { return (UnitTypeElement)BaseGet(name); }
        }
        #endregion
 
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as UnitTypeElement).Name;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new UnitTypeElement();
        }

        /// <summary>
        /// Convenience method to facilitate querying the unit types.
        /// </summary>
        public IList<UnitTypeElement> ToList()
        {
            var a = new UnitTypeElement[this.Count];
            this.CopyTo(a, 0);
            return a.ToList();
        }
    }
}
    