using System;

namespace Systems.SimpleEconomy.Data.Enums
{
    [Flags]
    public enum ModifyWalletCurrencyFlags
    {
        None = 0,
        
        /// <summary>
        ///     Currency will be added without checking conditions, may cause overflow
        /// </summary>
        IgnoreConditions = 1 << 0,
    }
}