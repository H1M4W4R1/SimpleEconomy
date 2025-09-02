using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Wallets;
using Systems.SimpleEconomy.Wallets.Abstract;

namespace Systems.SimpleEconomy.Data.Context
{
    /// <summary>
    ///     Readonly struct containing context for currency taking
    /// </summary>
    public readonly ref struct CurrencyTakeContext
    {
        /// <summary>
        ///     Currency to take
        /// </summary>
        public readonly CurrencyBase currency;
        
        /// <summary>
        ///     Wallet from which currency is taken
        /// </summary>
        public readonly ICurrencyWallet wallet;
        
        /// <summary>
        ///     Amount of currency that was expected to be taken
        /// </summary>
        public readonly long amount;

        public CurrencyTakeContext(CurrencyBase currency, ICurrencyWallet wallet, long amount)
        {
            this.currency = currency;
            this.wallet = wallet;
            this.amount = amount;
        }
    }
}