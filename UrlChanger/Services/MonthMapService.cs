namespace UrlChanger.Services
{
    public static class MonthMapService
    {
        private static Dictionary<int, string> _monthMap = new Dictionary<int, string>()
        {
            {1, "IOT1" },
            {2,"IOT2" },
            {3,"IOT3" },
            {4,"IOT4" }
        };

        public static string? MonthToSubject(int monthNum)
        {
            string? result = null;

            if(_monthMap.ContainsKey(monthNum))
            {
                result = _monthMap[monthNum];
            }

            return result;
        }
    }
}