using System;

namespace Content.Data
{
    [Serializable]
    public class UserData
    {
        public int ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Ip_Address { get; set; }
    }
}