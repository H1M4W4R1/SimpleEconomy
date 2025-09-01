using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Wallets;
using Systems.SimpleEconomy.Wallets.Abstract;

namespace Systems.SimpleEconomy.Data.Context
{
    public readonly ref struct CurrencyAddContext
    {
        public readonly CurrencyBase currency;
        public readonly ICurrencyWallet wallet;
        public readonly long amount;

        public CurrencyAddContext(CurrencyBase currency, ICurrencyWallet wallet, long amount)
        {
            this.currency = currency;
            this.wallet = wallet;
            this.amount = amount;
        }
    }
}