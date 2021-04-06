using Minded.Validation;

namespace MB.Common
{
    public class CodedValidationEntry : ValidationEntry
    {
        public CodedValidationEntry(string propertyName, string error, string errorCode) : base(propertyName, error) {
            base.ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return string.Format(this.ErrorMessage, PropertyName);
        }
    }
}
