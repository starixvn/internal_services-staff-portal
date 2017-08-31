namespace SSP.Models.Account
{
    using SSP.Models.Function;
    using System.Collections.Generic;

    public class UserSession
    {
        public UserSession()
        {
            Functions = new List<Function>();
        }

        public int UserId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public IEnumerable<Function> Functions { get; set; }
    }
}