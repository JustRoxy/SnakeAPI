using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnakeAPI.Model.Auth.DataHolder
{
    public class DictionaryHolder : IHolder<string>
    {

        public static readonly Dictionary<string, Snapshot> Holder = new Dictionary<string, Snapshot>();

        public void Create(string token, Snapshot snapshot)
        {
            if(!IsExist(token)) Holder.Add(token, snapshot);
            else throw new Exception("Adding new game while another game in progress");
        }

        public Snapshot Get(string token)
        {
            if (IsExist(token)) return Holder[token];
            throw new Exception("Getting no existing game");
            
        }

        public void Delete(string token)
        {
            if (IsExist(token)) Holder.Remove(token);
            else throw new Exception("Deleting not existed record");
        }

        public void Edit(string token, Snapshot snapshot)
        {
            if (IsExist(token)) Holder[token] = snapshot;
            else throw new Exception("No record to edit");
        }

        public bool IsExist(string token) => Holder.ContainsKey(token);
    }
}
