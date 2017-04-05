using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceWebApp.Models
{
    public interface IAliceRepository
    {
        void Add(Character item);
        IEnumerable<Character> GetAll();
        Character Find(long key);
        void Remove(long key);
        void Update(Character item);
    }
}
