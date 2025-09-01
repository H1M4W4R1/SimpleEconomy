using Systems.SimpleEconomy.Data.Context;

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
        /// <returns>True if the currency was taken, false otherwise</returns>
        public bool TryTake(long currencyAmount);
        
        /// <summary>
        ///     Tries to add the specified amount of currency to the wallet
        /// </summary>
        /// <param name="currencyAmount">Amount of currency to add</param>
        /// <returns>Amount of currency that was left</returns>
        public long TryAdd(long currencyAmount);

        /// <summary>
        ///     Checks if the specified amount of currency can be taken from the wallet
        /// </summary>
        public bool CanTake(CurrencyTakeContext context);

        /// <summary>
        ///     Checks if the specified amount of currency can be added to the wallet
        /// </summary>
        public bool CanAdd(CurrencyAddContext context);
    }
}