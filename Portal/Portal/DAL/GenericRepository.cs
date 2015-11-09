using System.Data.Entity;
using System.Linq;

namespace Portal.DAL
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }

        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entityToInsert)
        {
            _dbSet.Add(entityToInsert);
        }

        public virtual void Delete(object id)
        {
            //var param = new ObjectParameter(":p0", id);
            //_dbSet.SqlQuery("Delete * from _dbSet where Id = :p0", param);

            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);

            //var entityToDelete = new TEntity(){Id = id};
            //_dbSet.Attach(entityToDelete);
            //_context.Entry(entityToDelete).State = EntityState.Deleted;
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            //_context.Entry(typeof(TEntity)).State = EntityState.Modified;
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}