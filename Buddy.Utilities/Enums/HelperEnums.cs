namespace Buddy.Utilities.Enums
{
    public struct HelperEnums
    {
        public enum DBExecType
        {
            ExecuteNonQuery = 1,
            ExecuteScalar = 2,
            DataAdapter = 3
        }
        public enum ErrorCode: int
        {
            Zero = 0,
            Exception = 100
        }
    }
}
