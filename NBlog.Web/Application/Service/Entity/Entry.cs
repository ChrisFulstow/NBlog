using MongoDB.Bson.Serialization.Attributes;
using PetaPoco;
using System;

namespace NBlog.Web.Application.Service.Entity
{
	[PrimaryKey("Id")]
	public class Entry
	{
		// Id for PetaPoco SqlRepository support
		public int Id { get; set; }
		[BsonId]
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public DateTime DateCreated { get; set; }
		public string Markdown { get; set; }
		public bool? IsPublished { get; set; }
		public bool? IsCodePrettified { get; set; }

		public override string ToString()
		{
			return Title;
		}
	}
}