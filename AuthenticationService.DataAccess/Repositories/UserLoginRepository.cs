using AuthenticationService.DataAccess.InterfaceRepositories;
using AuthenticationService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.DataAccess.Repositories
{
    internal class UserLoginRepository : RepositoryBase<UserLogin>, IUserLoginRepository
    {
        public UserLoginRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}
