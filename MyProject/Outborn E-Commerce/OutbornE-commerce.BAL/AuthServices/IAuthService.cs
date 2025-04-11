using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.External_Logins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.AuthServices
{
	public interface IAuthService
	{
        Task<AuthResponseModel?> ValidateUser(UserForLoginDto userForAuth);
		Task<string> CreateToken();

		Task<ExternalLoginResult> LoginWithGoogleAsync(string code);
		Task<ExternalLoginResult> LoginWithFaceBookAsync(string accessToken);

	}
}
