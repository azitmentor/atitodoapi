using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Atitodoapi.Controllers
{
	public class MyControllerBase : ControllerBase
	{
		public int? UserId
		{
			get
			{
				var accessToken = Request.Headers[HeaderNames.Authorization];
				var jwt = accessToken.ToString().Replace("Bearer ", "");
				var tokenHandler = new JwtSecurityTokenHandler();

				var tkn = tokenHandler.ReadJwtToken(jwt);

				var cl1 = tkn.Claims.FirstOrDefault(p => p.Type == "UserId");
				int userid;
				if (cl1 == null || string.IsNullOrWhiteSpace(cl1.Value) || !int.TryParse(cl1.Value, out userid))
				{
					return null;
				}
				return userid;
			}
		}
	}
}
