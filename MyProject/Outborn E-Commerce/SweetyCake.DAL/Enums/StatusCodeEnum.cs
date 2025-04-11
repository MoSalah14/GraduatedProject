using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Enums
{
    public enum StatusCodeEnum
    {
        Ok = 200,
        NotFound = 404,
        BadRequest = 400,
        Created = 201,
        ServerError = 500,
        Unauthorized  = 401
    }
}
