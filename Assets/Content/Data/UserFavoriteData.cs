using System;
using System.Collections.Generic;

namespace Content.Data
{
    [Serializable]
    public class UserFavoriteData
    {
        public HashSet<int> FavoriteIDs { get; set; }
    }
}