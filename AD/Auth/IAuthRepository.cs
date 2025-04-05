using AD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Auth
{
    public interface IAuthRepository
    {
        public string Login(ClientCredentials  credentials);
    }
}
