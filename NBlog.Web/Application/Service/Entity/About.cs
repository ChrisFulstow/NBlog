using PetaPoco;

namespace NBlog.Web.Application.Service.Entity
{
	[PrimaryKey("Id")]
	public class About
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Content { get; set; }
	}
}