using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleEconomy.Data.Context;
using Systems.SimpleEconomy.Data.Enums;

namespace Systems.SimpleEconomy.Wallets.Abstract
{
    public interface ICurrencyWallet
    {
        /// <summary>
        ///     Balance of the wallet
        /// </summary>
        public long Balance { get; }

        /// <summary>
        ///     Tries to take the specified amount of currency from the wallet
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to take</param>
        /// <param name="flags">Flags that modify the behavior of the method</param>
        /// <param name="actionSource">Source of the action</param>
        /// <returns>Amount of currency that was left</returns>
        public OperationResult<long> TryTake(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External);

        /// <summary>
        ///     Tries to add the specified amount of currency to the wallet
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to add</param>
        /// <param name="flags">Flags that modify the behavior of the method</param>
        /// <param name="actionSource">Source of the action</param>
        /// <returns>Amount of currency that was left</returns>
        public OperationResult<long> TryAdd(
            long currencyAmount,
            ModifyWalletCurrencyFlags flags = ModifyWalletCurrencyFlags.None,
            ActionSource actionSource = ActionSource.External);

        /// <summary>
        ///     Checks if the specified amount of currency can be taken from the wallet
        /// </summary>
        public OperationResult CanTakeCurrency(CurrencyTakeContext context);

        /// <summary>
        ///     Checks if the specified amount of currency can be added to the wallet
        /// </summary>
        public OperationResult CanAddCurrency(CurrencyAddContext context);
    }
}