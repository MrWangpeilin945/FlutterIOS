namespace Ryobi.Wellship.WebAPI.ExternalConnection.Utilities
{
    /// <summary>
    /// SQL生成用フォーマッター
    /// </summary>
    public static class SqlFormatter
    {
        /// <summary>
        /// SQL文で使用する値をエスケープします。
        /// </summary>
        /// <param name="input">エスケープする値</param>
        /// <returns>エスケープされた値</returns>
        public static string EscapeSqlValue(object input)
        {
            if (input == null)
            {
                return "NULL";
            }

            switch (input)
            {
                case string str:
                    return $"'{str.Replace("'", "''")}'";
                case int number:
                    return $"'{number.ToString()}'";
                case DateTime date:
                    return $"'{date:yyyy-MM-dd HH:mm:ss}'";
                case Guid guid:
                    return $"'{guid.ToString("D")}'";
                default:
                    return "NULL";
            }
        }
    }
}