namespace Prototype.Data
{
    public class AppData
    {
        public AppData()
        {
            User = new UserData();
        }

        // Data
        public UserData User;
        public GameBalanceData GameBalance;
    }
}
