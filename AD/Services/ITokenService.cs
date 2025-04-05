using AD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Services
{
    public interface ITokenService
    {
        Task<AuthResponse?> ValidateUserAndGenerateToken(AuthRequest request);
    }
}
