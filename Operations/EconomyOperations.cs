using Systems.SimpleCore.Operations;

namespace Systems.SimpleEconomy.Operations
{
    public static class EconomyOperations
    {
        public const int NOT_ENOUGH_CURRENCY = 1;
        
        public static OperationResult NotEnoughCurrency() => new(NOT_ENOUGH_CURRENCY);
        public static OperationResult CurrencyAdded() => OperationResult.GenericSuccess;
        public static OperationResult CurrencyTaken() => OperationResult.GenericSuccess;
        
        public static OperationResult Permitted() => OperationResult.GenericSuccess;
    }
}