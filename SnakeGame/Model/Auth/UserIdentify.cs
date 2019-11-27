using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnakeAPI.Model.Auth.DataHolder;

namespace SnakeAPI.Model.Auth
{
    public class UserIdentify
    {
        public static string GenerateGuid() => Guid.NewGuid().ToString();

        public static IHolder<string> GetHolder(string token)
        {
            var holder = new DictionaryHolder();
            if(!holder.IsExist(token)) holder.Create(token, new Snapshot());
            return holder;
        }
        
    }
}
