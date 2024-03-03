using System;
using System.Collections.Generic;

namespace Content.Data
{
    [Serializable]
    public class UserConfigData
    {
        public List<UserData> Data { get; set; }
    }
}