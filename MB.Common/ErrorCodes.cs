namespace MB.Common
{
    public static class ErrorCodes
    {
        public const string TransactionDataIsMandatory = "ET1";
        public const string SpecifiedAccountDoesNotExist = "ET2";
        public const string SpecifiedCurrencyIsNotSupported = "ET3";
        public const string CannotSpecifyTransactionId = "ET4";
        public const string TransactionDescriptionIsMandatory = "ET5";
        public const string TransactionOriginalAmountIsMandatory = "ET6";
        public const string TransactionAmountCannotBeSpefied = "ET7";
        public const string TransactionAmountExceedsBalance = "ET8";
    }
}
