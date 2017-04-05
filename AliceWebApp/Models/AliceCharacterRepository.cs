using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceWebApp.Models
{
    public class AliceCharacterRepository : IAliceRepository
    {
        private readonly AliceContext _ctx;

        public AliceCharacterRepository(AliceContext cont)
        {
            _ctx = cont;

            if (_ctx.Characters.Count() == 0)
            {
                Add(new Character { Name = "Alice" });
                Add(new Character { Name = "Cheshire Cat" });
            }

        }


        public void Add(Character item)
        {
            _ctx.Characters.Add(item);
            _ctx.SaveChanges();
        }

        public Character Find(long id)
        {
            return  _ctx.Characters.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Character> GetAll()
        {
            return _ctx.Characters.ToList();
        }

        public void Remove(long id)
        {
            var entity = _ctx.Characters.First(t => t.Id == id);
            _ctx.Characters.Remove(entity);
            _ctx.SaveChanges();
        }

        public void Update(Character item)
        {
            _ctx.Characters.Update(item);
            _ctx.SaveChanges();
        }
    }
}
