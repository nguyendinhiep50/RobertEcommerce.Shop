﻿namespace Identity.API.Model.Dtos
{
	public class UserLoginResponse
	{
		public string UserName { get; set; } = null!;
		public string Token { get; set; } = null!;
	}
}