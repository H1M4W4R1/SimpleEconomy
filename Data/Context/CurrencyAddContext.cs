using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Wallets;
using Systems.SimpleEconomy.Wallets.Abstract;

namespace Systems.SimpleEconomy.Data.Context
{
    /// <summary>
    ///     Readonly struct containing context for currency adding
    /// </summary>
    public readonly ref struct CurrencyAddContext
    {
        /// <summary>
        ///     Currency to add
        /// </summary>
        public readonly CurrencyBase currency;
        
        /// <summary>
        ///     Wallet to which currency is added
        /// </summary>
        public readonly ICurrencyWallet wallet;
        
        /// <summary>
        ///     Amount of currency to add or added
        /// </summary>
        public readonly long amount;

        public CurrencyAddContext WithNewAmount(long newAmount) => new(currency, wallet, newAmount);
        
        public CurrencyAddContext(CurrencyBase currency, ICurrencyWallet wallet, long amount)
        {
            this.currency = currency;
            this.wallet = wallet;
            this.amount = amount;
        }
    }
}