using AuthenticationService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.DataAccess.InterfaceRepositories
{
    public interface IUserLoginRepository : IRepositoryBase<UserLogin>
    {
    }
}
