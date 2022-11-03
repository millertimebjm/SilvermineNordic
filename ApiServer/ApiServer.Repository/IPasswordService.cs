using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Repository
{
    public interface IPasswordService
    {
        string ComputeHash(byte[] bytesToHash, byte[] salt);
        string GenerateSalt();
    }
}
