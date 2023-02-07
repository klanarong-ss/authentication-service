using AuthenticationService.DataAccess.InterfaceRepositories;
using AuthenticationService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DataContext _dataContext;
        private IUserLoginRepository _userLoginRepository;

        public UnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IUserLoginRepository UserLogin
        {
            get
            {
                if (_userLoginRepository == null)
                {
                    _userLoginRepository = new UserLoginRepository(_dataContext);
                }
                return _userLoginRepository;
            }
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }

        public void Save()
        {
            _dataContext.SaveChangesAsync();
        }
    }
}
