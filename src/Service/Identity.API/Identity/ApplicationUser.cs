namespace Identity.API.Identity;

public class ApplicationUser : IdentityUser<string>
{
	public string Name { get; set; } = null!;
	public string UserKbn { get; set; } = string.Empty;
	public string Name1 { get; set; } = string.Empty;
	public string Name2 { get; set; } = string.Empty;
	public string NickName { get; set; } = string.Empty;
	public string Addr { get; set; } = string.Empty;
	public string Addr1 { get; set; } = string.Empty;
	public string Addr2 { get; set; } = string.Empty;
	public string Fax { get; set; } = string.Empty;
	public string Sex { get; set; } = string.Empty;
	public string BirthYear { get; set; } = string.Empty;
	public string BirthMonth { get; set; } = string.Empty;
	public string BirthDay { get; set; } = string.Empty;
	public string MailFlg { get; set; } = string.Empty;
	public string MemberRankId { get; set; } = string.Empty;
	public DateTime? DateLastLoggedin { get; set; }
	public string UserManagementLevelId { get; set; } = string.Empty;
	public int? OrderCountOrderRealtime { get; set; }
	public string ReferredUserId { get; set; } = string.Empty;
	public bool Status { get; set; }
	public DateTimeOffset CreatedDate { get; set; }
	public DateTimeOffset? UpdatedDate { get; set; }
	public string? CreatedBy { get; set; }
	public string? UpdatedBy { get; set; }
	public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
