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

        Task<List<t_State>> GetAllStates();
        Task<t_State> GetStateById(int id);
        
        Task<List<t_State>> GetStatesByCountryId(int countryId);
        Task<List<t_District>> GetDistrictsByStateId(int stateId);
        Task<t_District> GetDistrictById(int id);

        Task<List<t_Country>> GetAllCountries();
        Task<t_Country> GetCountryById(int id);
    }
}