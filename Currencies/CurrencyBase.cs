using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleEconomy.Data;
using Systems.SimpleEconomy.Data.Context;
using UnityEngine;

namespace Systems.SimpleEconomy.Currencies
{
    /// <summary>
    ///     Base class for in-game currency or usable resource such as mana
    /// </summary>
    [AutoCreate("Currencies", CurrencyDatabase.LABEL)]
    public abstract class CurrencyBase : ScriptableObject
    {
        /// <summary>
        ///     Checks if the specified amount of currency can be added.
        /// </summary>
        protected internal virtual bool CanBeAdded(CurrencyAddContext context) => true;
        
        /// <summary>
        ///     Check if specified amount of currency can be taken.
        /// </summary>
        protected internal virtual bool CanBeTaken(CurrencyTakeContext context) => true;
        
        /// <summary>
        ///     Event that is called when currency is taken.
        /// </summary>
        protected internal virtual void OnCurrencyTaken(CurrencyTakeContext context) { }
        
        /// <summary>
        ///     Event that is called when currency is added.
        /// </summary>
        protected internal virtual void OnCurrencyAdded(CurrencyAddContext context) { }
    }
}