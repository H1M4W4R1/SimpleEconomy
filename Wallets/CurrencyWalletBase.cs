using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Data;
using Systems.SimpleEconomy.Data.Context;
using Systems.SimpleEconomy.Wallets.Abstract;
using UnityEngine;
using UnityEngine.Assertions;

namespace Systems.SimpleEconomy.Wallets
{
    /// <summary>
    ///     Wallet for a single currency type
    /// </summary>
    /// <typeparam name="CurrencyType"></typeparam>
    public abstract class CurrencyWalletBase<CurrencyType> : CurrencyWalletBase
        where CurrencyType : CurrencyBase, new()
    {
        /// <summary>
        ///     Adds the specified amount of currency to the wallet
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to add</param>
        /// <returns>Remaining amount of currency that could not be added</returns>
        protected virtual long Add(long currencyAmount)
        {
            Balance += currencyAmount;
            return 0;
        }
        
        public sealed override long TryAdd(long currencyAmount)
        {
            // Get currency from database
            CurrencyType currency = CurrencyDatabase.GetExact<CurrencyType>();
            Assert.IsNotNull(currency, "Currency was not found in database.");
            
            // Create context
            CurrencyAddContext context = new(currency, this, currencyAmount);
            
            // Ensure that currency can be added
            if (!currency.CanBeAdded(context)) return currencyAmount;
            if (!CanAdd(context)) return currencyAmount;
            
            // Add currency and get remaining amount
            currencyAmount = Add(currencyAmount);
            
            // Invoke event
            OnCurrencyAdded(context);
            currency.OnCurrencyAdded(context);
            
            return currencyAmount;
        }

        public sealed override bool TryTake(long currencyAmount)
        {
            // Get currency from database
            CurrencyType currency = CurrencyDatabase.GetExact<CurrencyType>();
            Assert.IsNotNull(currency, "Currency was not found in database.");

            // Create context
            CurrencyTakeContext context = new(currency, this, currencyAmount);

            // Ensure that currency can be taken
            if (!currency.CanBeTaken(context)) return false;
            if (!CanTake(context)) return false;
            
            // Take currency
            Balance -= currencyAmount;
            
            // Invoke event
            OnCurrencyTaken(context);
            currency.OnCurrencyTaken(context);
            
            return true;
        }
        
        /// <summary>
        ///     Event that is called when currency is taken from the wallet
        /// </summary>
        protected virtual void OnCurrencyTaken(CurrencyTakeContext context) { }
        
        /// <summary>
        ///     Event that is called when currency is added to the wallet
        /// </summary>
        protected virtual void OnCurrencyAdded(CurrencyAddContext context) { }
    }

    public abstract class CurrencyWalletBase : MonoBehaviour, ICurrencyWallet
    {
        /// <summary>
        ///     Balance of the wallet
        /// </summary>
        public long Balance { get; protected set; }

        /// <summary>
        ///     Checks if the wallet has the specified amount of currency
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to check</param>
        /// <returns>True if the wallet has the specified amount of currency, false otherwise</returns>
        public virtual bool Has(long currencyAmount) => Balance >= currencyAmount;
        public abstract bool TryTake(long currencyAmount);
        public abstract long TryAdd(long currencyAmount);

        /// <summary>
        ///     Checks if the specified amount of currency can be taken from the wallet
        /// </summary>
        public virtual bool CanTake(CurrencyTakeContext context) => Balance >= context.amount;

        /// <summary>
        ///     Checks if the specified amount of currency can be added to the wallet
        /// </summary>
        public virtual bool CanAdd(CurrencyAddContext context) => true;
    }
}