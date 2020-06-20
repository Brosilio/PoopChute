using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    public interface IHandshakeHandler
    {
        Task<bool> Handle(PoopClient context);
    }
}
