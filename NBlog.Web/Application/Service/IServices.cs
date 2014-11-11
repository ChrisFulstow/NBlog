namespace NBlog.Web.Application.Service
{
	public interface IServices
	{
		IEntryService Entry { get; }
		IUserService User { get; }
		IConfigService Config { get; }
		IMessageService Message { get; }
		ICloudService Cloud { get; }
		IThemeService Theme { get; }
		IAboutService About { get; }
	}
}