using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Data;
using Systems.SimpleEconomy.Data.Context;
using Systems.SimpleEconomy.Data.Enums;
using Systems.SimpleEconomy.Wallets.Abstract;
using Unity.Mathematics;
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

        public sealed override long TryAdd(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External)
        {
            // Get currency from database
            CurrencyType currency = CurrencyDatabase.GetExact<CurrencyType>();
            Assert.IsNotNull(currency, "Currency was not found in database.");

            // Create context
            CurrencyAddContext context = new(currency, this, currencyAmount);

            // Ensure that currency can be added
            if (!CanAddCurrency(context) && (flags & ModifyWalletCurrencyFlags.IgnoreConditions) == 0)
            {
                // Invoke event
                if (actionSource == ActionSource.Internal) return currencyAmount;
                OnCurrencyAddFailed(context);
                return currencyAmount;
            }

            long currencyToAdd = currencyAmount;
            
            // Add currency and get remaining amount
            currencyAmount = Add(currencyAmount);
            
            // Invoke event
            if (actionSource == ActionSource.Internal) return currencyAmount;
            
            // Update context
            long currencyLeft = currencyToAdd - currencyAmount;
            context = context.WithNewAmount(currencyLeft);
            
            OnCurrencyAdded(context);
            return currencyAmount;
        }


        public sealed override bool TryTake(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External
        )
        {
            // Get currency from database
            CurrencyType currency = CurrencyDatabase.GetExact<CurrencyType>();
            Assert.IsNotNull(currency, "Currency was not found in database.");

            // Create context
            CurrencyTakeContext context = new(currency, this, currencyAmount);

            // Ensure that currency can be taken
            if (!CanTakeCurrency(context) && (flags & ModifyWalletCurrencyFlags.IgnoreConditions) == 0)
            {
                // Invoke event
                if (actionSource == ActionSource.Internal) return false;
                OnCurrencyTakeFailed(context);
                return false;
            }

            long currencyTaken = math.min(Balance, currencyAmount);
            
            // Take currency
            Balance -= currencyTaken;

            // Invoke event
            if (actionSource == ActionSource.Internal) return true;
            context = context.WithNewAmount(currencyTaken);
            OnCurrencyTaken(context);
            return true;
        }

        /// <summary>
        ///     Event that is called when currency is taken from the wallet
        /// </summary>
        protected virtual void OnCurrencyTaken(CurrencyTakeContext context)
            => context.currency.OnCurrencyTaken(context);

        /// <summary>
        ///     Event that is called when currency is added to the wallet
        /// </summary>
        protected virtual void OnCurrencyAdded(CurrencyAddContext context)
            => context.currency.OnCurrencyAdded(context);
        
        /// <summary>
        ///     Event that is called when currency take fails
        /// </summary>
        protected virtual void OnCurrencyTakeFailed(CurrencyTakeContext context)
            => context.currency.OnCurrencyTakeFailed(context);
        
        /// <summary>
        ///     Event that is called when currency addition fails
        /// </summary>
        protected virtual void OnCurrencyAddFailed(CurrencyAddContext context)
            => context.currency.OnCurrencyAddFailed(context);
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

        public abstract bool TryTake(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External);

        public abstract long TryAdd(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External);

        /// <summary>
        ///     Checks if the specified amount of currency can be taken from the wallet
        /// </summary>
        public virtual bool CanTakeCurrency(CurrencyTakeContext context) =>
            Balance >= context.amount && context.currency.CanBeTaken(context);

        /// <summary>
        ///     Checks if the specified amount of currency can be added to the wallet
        /// </summary>
        public virtual bool CanAddCurrency(CurrencyAddContext context) =>
            context.currency.CanBeAdded(context);
    }
}