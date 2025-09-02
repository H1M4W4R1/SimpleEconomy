using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleCore.Operations;
using Systems.SimpleEconomy.Data;
using Systems.SimpleEconomy.Data.Context;
using UnityEngine;

namespace Systems.SimpleEconomy.Currencies
{
    /// <summary>
    ///     Base class for in-game currency or usable resource such as mana
    /// </summary>
    [AutoCreate("Currencies", CurrencyDatabase.LABEL)] public abstract class CurrencyBase : ScriptableObject
    {
        /// <summary>
        ///     Checks if the specified amount of currency can be added.
        /// </summary>
        public virtual OperationResult CanBeAdded(CurrencyAddContext context) => (OperationResult) true;

        /// <summary>
        ///     Check if specified amount of currency can be taken.
        /// </summary>
        public virtual OperationResult CanBeTaken(CurrencyTakeContext context) => (OperationResult) true;

        /// <summary>
        ///     Event that is called when currency is taken.
        /// </summary>
        protected internal virtual void OnCurrencyTaken(
            CurrencyTakeContext context,
            OperationResult<long> resultAmountLeft)
        {
        }

        /// <summary>
        ///     Event that is called when currency take fails.
        /// </summary>
        protected internal virtual void OnCurrencyTakeFailed(
            CurrencyTakeContext context,
            OperationResult<long> resultAmountExpected)
        {
        }

        /// <summary>
        ///     Event that is called when currency is added.
        /// </summary>
        protected internal virtual void OnCurrencyAdded(
            CurrencyAddContext context,
            OperationResult<long> resultAmountLeft)
        {
        }

        /// <summary>
        ///     Event that is called when currency addition fails.
        /// </summary>
        protected internal virtual void OnCurrencyAddFailed(
            CurrencyAddContext context,
            OperationResult<long> resultAmountExpected)
        {
        }
    }
}