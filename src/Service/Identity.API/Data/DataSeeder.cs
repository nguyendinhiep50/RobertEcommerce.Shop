namespace Identity.API.Data;
internal static class DataSeeder
{
	private static bool Seeded = false;
	private static readonly string IdUserGuild = Guid.NewGuid().ToString();
	private static readonly string IdRoleGuild = Guid.NewGuid().ToString();

	private static readonly List<ApplicationUser> _apllicationUsers = [];
	private static readonly List<ApplicationRole> _apllicationRoles = [];
	private static readonly List<ApplicationUserRole> _apllicationUserRoles = [];
	private static readonly List<Rb_CustomerUser> _customerUser = [];

	public static void Seed(ModelBuilder builder)
	{
		if (!Seeded)
		{
			Seeded = true;
		}
		else
		{
			#region Data EC
			SeedApllicationUser(builder);
			SeedApllicationRole(builder);
			SeedApllicationUserRole(builder);
			#endregion

			#region Data Front
			SeedCustomerUser(builder);
			#endregion
		}
	}
	#region Data EC
	private static void SeedApllicationUser(ModelBuilder builder)
	{
		var superadmins = new List<ApplicationUser> {
			new ApplicationUser
			{
				Id = IdFormatter.FormatId(Constant.CONSTANT_KEY_USER_ID, 2),
				Name = "Super Admin",
				UserName = "superadmin@gmail.com",
				NormalizedUserName= "ADMIN@EXAMPLE.COM",
				PhoneNumber = "0936333401",
				Email = "superadmin@gmail.com",
				NormalizedEmail="SUPERADMIN@GMAIL.COM",
				PhoneNumberConfirmed = true,
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString("D")
			}
		};
		superadmins[0].PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(superadmins[0], "Abc123*");

		_apllicationUsers.AddRange(superadmins);

		builder.Entity<ApplicationUser>()
			.HasData(_apllicationUsers);

		builder.Entity<ApplicationUser>(b =>
		{
			b.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(ur => ur.UserId).IsRequired();
		});
	}

	private static void SeedApllicationRole(ModelBuilder builder)
	{
		var roles = new List<ApplicationRole> {
			new ApplicationRole
			{
				Id = Guid.NewGuid().ToString(),
				Name = Roles.Administrator,
				NormalizedName = Roles.Administrator.ToUpper()
			}
		};
		_apllicationRoles.AddRange(roles);

		builder.Entity<ApplicationRole>()
			.HasData(_apllicationRoles);

		builder.Entity<ApplicationRole>(b =>
		{
			b.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(ur => ur.RoleId).IsRequired();
		});
	}

	private static void SeedApllicationUserRole(ModelBuilder builder)
	{
		var userRole = new List<ApplicationUserRole>
		{
			new ApplicationUserRole
			{
				RoleId = IdRoleGuild,
				UserId = IdUserGuild
			}
		};

		_apllicationUserRoles.AddRange(userRole);

		builder.Entity<ApplicationUserRole>()
			.HasData(_apllicationUserRoles);

		builder.Entity<ApplicationUserRole>(userRole =>
		{
			userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

			userRole.HasOne(ur => ur.User)
				.WithMany(u => u.UserRoles)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			userRole.HasOne(ur => ur.Role)
				.WithMany(r => r.UserRoles)
				.HasForeignKey(ur => ur.RoleId);
		});
	}
	#endregion

	#region Data Front
	private static void SeedCustomerUser(ModelBuilder builder)
	{
		var clientUser = new List<Rb_CustomerUser> {
			new Rb_CustomerUser
			{
				Id = IdRoleGuild,
				UserKbn = "Client",
				Name = "Client User test",
				Name1 = "Client User test",
				Name2= "Client User test",
				NickName = "Robert",
				MailAddr = "ClientUserTest@gmail.com",
				MailAddr2 = "ClientUserTest@gmail.com",
				Addr = "none",
				Addr1 = "none",
				Addr2 = "none",
				Tel1 = "78645078264356",
				Tel2 = "78645078264356",
				Tel3 = "78645078264356",
				Fax = "123",
				Sex = "0",
				BirthYear = "2000",
				BirthMonth = "12",
				BirthDay = "04",
				MailFlg = "1",
				MemberRankId = "1",
				DateLastLoggedin = null,
				UserManagementLevelId = string.Empty,
				OrderCountOrderRealtime = null,
				ReferredUserId = string.Empty,
				Email = "ClientUserTest@gmail.com",
				NormalizedEmail = "CLIENTUSERTEST@GMAIL.COM",
				PhoneNumberConfirmed = true,
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString("D"),
				UserName = "ClientUserTest@gmail.com",
				NormalizedUserName= "CLIENTUSERTEST@GMAIL.COM",
			}
		};

		clientUser[0].PasswordHash = new PasswordHasher<Rb_CustomerUser>().HashPassword(clientUser[0], "Abc123*");

		_customerUser.AddRange(clientUser);

		builder.Entity<Rb_CustomerUser>()
			.HasData(_customerUser);
	}
	#endregion
}
