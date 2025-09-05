using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Data;
using Systems.SimpleEconomy.Data.Context;
using Systems.SimpleEconomy.Data.Enums;
using Systems.SimpleEconomy.Operations;
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
        
        /// <summary>
        ///     Attempts to add the specified amount of currency to the wallet.
        ///     If the currency cannot be added, a failed operation result is returned.
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to add</param>
        /// <param name="flags">Flags to modify wallet behavior</param>
        /// <param name="actionSource">Source of the action</param>
        /// <returns>Operation result of the add attempt with remaining amount of currency</returns>
        public sealed override OperationResult<long> TryAdd(
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
            OperationResult<long> canAddCurrency = CanAddCurrency(context).WithData(currencyAmount);
            if (!canAddCurrency && (flags & ModifyWalletCurrencyFlags.IgnoreConditions) == 0)
            {
                // Invoke event
                if (actionSource == ActionSource.Internal) return canAddCurrency;
                OnCurrencyAddFailed(context, canAddCurrency);
                return canAddCurrency;
            }

            long currencyToAdd = currencyAmount;

            // Add currency and get remaining amount
            currencyAmount = Add(currencyAmount);

            // Invoke event
            OperationResult<long> currencyAddResult = EconomyOperations.CurrencyAdded().WithData(currencyAmount);
            if (actionSource == ActionSource.Internal) return currencyAddResult;

            // Update context
            long currencyLeft = currencyToAdd - currencyAmount;
            context = context.WithNewAmount(currencyLeft);

            OnCurrencyAdded(context, currencyAddResult);
            return currencyAddResult;
        }

        
        /// <summary>
        ///     Attempts to take the specified amount of currency from the wallet.
        ///     If the currency cannot be taken, a failed operation result is returned.
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to take</param>
        /// <param name="flags">Flags to modify wallet behavior</param>
        /// <param name="actionSource">Source of the action</param>
        /// <returns>Operation result of the take attempt with remaining amount of currency</returns>
        public sealed override OperationResult<long> TryTake(
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
            OperationResult<long> canTakeCurrency = CanTakeCurrency(context).WithData(currencyAmount);
            if (!canTakeCurrency && (flags & ModifyWalletCurrencyFlags.IgnoreConditions) == 0)
            {
                // Invoke event
                if (actionSource == ActionSource.Internal) return canTakeCurrency;
                OnCurrencyTakeFailed(context, canTakeCurrency);
                return canTakeCurrency;
            }

            long currencyTaken = math.min(Balance, currencyAmount);
            long currencyLeftToTake = currencyAmount - currencyTaken;

            // Take currency
            Balance -= currencyTaken;

            OperationResult<long> currencyTakeResult =
                EconomyOperations.CurrencyTaken().WithData(currencyLeftToTake);

            // Invoke event
            if (actionSource == ActionSource.Internal) return currencyTakeResult;
            OnCurrencyTaken(context, currencyTakeResult);
            return currencyTakeResult;
        }

        /// <summary>
        ///     Event that is called when currency is taken from the wallet
        /// </summary>
        protected virtual void OnCurrencyTaken(in CurrencyTakeContext context, in OperationResult<long> resultAmountLeft)
            => context.currency.OnCurrencyTaken(context, resultAmountLeft);

        /// <summary>
        ///     Event that is called when currency is added to the wallet
        /// </summary>
        protected virtual void OnCurrencyAdded(in CurrencyAddContext context, in OperationResult<long> resultAmountLeft)
            => context.currency.OnCurrencyAdded(context, resultAmountLeft);

        /// <summary>
        ///     Event that is called when currency take fails
        /// </summary>
        protected virtual void OnCurrencyTakeFailed(
            in CurrencyTakeContext context,
            in OperationResult<long> resultAmountExpected)
            => context.currency.OnCurrencyTakeFailed(context, resultAmountExpected);

        /// <summary>
        ///     Event that is called when currency addition fails
        /// </summary>
        protected virtual void OnCurrencyAddFailed(
            in CurrencyAddContext context,
            in OperationResult<long> resultAmountExpected)
            => context.currency.OnCurrencyAddFailed(context, resultAmountExpected);
    }

    public abstract class CurrencyWalletBase : MonoBehaviour
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

        public abstract OperationResult<long> TryTake(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External);

        public abstract OperationResult<long> TryAdd(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External);

        /// <summary>
        ///     Checks if the specified amount of currency can be taken from the wallet
        /// </summary>
        protected virtual OperationResult CanTakeCurrency(in CurrencyTakeContext context)
        {
            if (Balance < context.amount) return EconomyOperations.NotEnoughCurrency();
            return context.currency.CanBeTaken(context);
        }

        /// <summary>
        ///     Checks if the specified amount of currency can be added to the wallet
        /// </summary>
        protected virtual OperationResult CanAddCurrency(in CurrencyAddContext context) =>
            context.currency.CanBeAdded(context);
    }
}