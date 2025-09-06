using JetBrains.Annotations;
using Systems.SimpleCore.Storage;
using Systems.SimpleCore.Storage.Databases;
using Systems.SimpleEconomy.Currencies;

namespace Systems.SimpleEconomy.Data
{
    /// <summary>
    ///     Database of currencies
    /// </summary>
    public sealed class CurrencyDatabase : AddressableDatabase<CurrencyDatabase, CurrencyBase>
    {
        public const string LABEL = "SimpleEconomy.Currencies";
        
        [NotNull] protected override string AddressableLabel => LABEL;
    }
}