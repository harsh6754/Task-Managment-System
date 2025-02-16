using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IRegisterLoginInterface
    {
        Task<int> Register(t_Register register);
        Task<t_Register> Login(t_Login login);
    }
}