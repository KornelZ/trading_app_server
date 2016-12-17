using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGSA.Model.Repositories;
using LGSA_Server.Model;

namespace LGSA.Model.UnitOfWork
{
    public class DbUnitOfWork : IUnitOfWork
    {
        private MainDatabaseEntities _context;
        private DbContextTransaction _transaction;
        private IRepository<users_Authetication> _authenticationRepository;
        private IRepository<product> _productRepository;
        private IRepository<buy_Offer> _buyOfferRepository;
        private IRepository<dic_condition> _conditionRepository;
        private IRepository<dic_Genre> _genreRepository;
        private IRepository<dic_Product_type> _productTypeRepository;
        private IRepository<sell_Offer> _sellOfferRepository;
        private IRepository<transactions> _transactionRepository;
        public virtual MainDatabaseEntities Context
        {
            get { return _context; }
        }

        public virtual IRepository<users_Authetication> AuthenticationRepository
        {
            get { return _authenticationRepository; }
        }

        public virtual IRepository<product> ProductRepository
        {
            get { return _productRepository; }
        }
        public virtual IRepository<buy_Offer> BuyOfferRepository
        {
            get { return _buyOfferRepository; }
        }
        public virtual IRepository<sell_Offer> SellOfferRepository
        {
            get { return _sellOfferRepository; }
        }
        public virtual IRepository<dic_condition> ConditionRepository
        {
            get { return _conditionRepository; }
        }
        public virtual IRepository<dic_Genre> GenreRepository
        {
            get { return _genreRepository; }
        }
        public virtual IRepository<dic_Product_type> ProductTypeRepository
        {
            get { return _productTypeRepository; }
        }
        public virtual IRepository<transactions> TransactionRepository
        {
            get { return _transactionRepository; }
        }
        public DbUnitOfWork()
        {
            _context = new MainDatabaseEntities();
            //_context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
            _authenticationRepository = new AuthenticationRepository(_context);
            _productRepository = new ProductRepository(_context);
            _buyOfferRepository = new BuyOfferRepository(_context);
            _sellOfferRepository = new SellOfferRepository(_context);
            _conditionRepository = new Repository<dic_condition>(_context);
            _genreRepository = new Repository<dic_Genre>(_context);
            _productTypeRepository = new Repository<dic_Product_type>(_context);
            _transactionRepository = new TransactionRepository(_context);
        }
        public virtual void Commit()
        {
            _transaction?.Commit();
        }
        public virtual void Rollback()
        {
            _transaction?.Rollback();
        }
        public virtual void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }  
        public virtual async Task<int> Save()
        {
            return  await _context.SaveChangesAsync();
        }

        public virtual void StartTransaction()
        {
            if(_transaction == null)
            {
                _transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            }
        }
    }
}
