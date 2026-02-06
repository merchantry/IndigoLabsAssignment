namespace IndigoLabsAssignment.Utilities
{
    public static class EnumUtils
    {
        public static bool ToEnum<TEnum>(string? value, out TEnum enumValue)
            where TEnum : struct, Enum
        {
            if (value is null || !Enum.TryParse(value, ignoreCase: true, out enumValue))
            {
                enumValue = default;
                return false;
            }

            return true;
        }
    }
}
