using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Infrastructure
{
    public class LayoutModel
    {
        //[NoBinding]
        //public LayoutCommonModel Common { get; set; }
        
        [NoBinding]
        public User User { get; set; }
        
        [NoBinding]
        public Theme Theme { get; set; }
        
        [NoBinding]
        public Config Config { get; set; }

        //public class LayoutCommonModel
        //{
        //    public User User { get; set; }
        //    public Theme Theme { get; set; }
        //    public Config Config { get; set; }
        //}
    }
}