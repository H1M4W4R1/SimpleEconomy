using Systems.SimpleCore.Operations;

namespace Systems.SimpleEconomy.Operations
{
    public static class EconomyOperations
    {
        public const ushort SYSTEM_ECONOMY = 0x0001;


        public const ushort ERROR_NOT_ENOUGH_CURRENCY = 1;

        public const ushort SUCCESS_CURRENCY_ADDED = 1;
        public const ushort SUCCESS_CURRENCY_TAKEN = 2;
    
        public static OperationResult NotEnoughCurrency()
            => OperationResult.Error(SYSTEM_ECONOMY, ERROR_NOT_ENOUGH_CURRENCY);

        public static OperationResult CurrencyAdded() => OperationResult.Success(SYSTEM_ECONOMY, SUCCESS_CURRENCY_ADDED);
        public static OperationResult CurrencyTaken() => OperationResult.Success(SYSTEM_ECONOMY, SUCCESS_CURRENCY_TAKEN);

        public static OperationResult Permitted() => OperationResult.Success(SYSTEM_ECONOMY, OperationResult.SUCCESS_PERMITTED);
    }
}