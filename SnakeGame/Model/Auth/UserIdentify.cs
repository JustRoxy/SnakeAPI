using System;
using SnakeAPI.Model.Auth.DataHolder;

namespace SnakeAPI.Model.Auth
{
    public class UserIdentify
    {
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static IHolder<string> GetHolder(string token)
        {
            var holder = new DictionaryHolder();
            if (!holder.IsExist(token)) holder.Create(token, new Snapshot());
            return holder;
        }

        public static IHolder<string> StrongGetHolder(string token)
        {
            var holder = new DictionaryHolder();
            return !holder.IsExist(token) ? null : holder;
        }
    }
}