using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NBlog.Web.Application;

namespace NBlog.Web.Controllers
{
    public partial class ContactController
    {
        public class IndexModel : LayoutModel
        {
            [DisplayName("Your name")]
            [Required(ErrorMessage = "Please enter your name.")]
            public string SenderName { get; set; }

            [DisplayName("Your email address")]
            [Required(ErrorMessage = "Please enter your email address.")]
            public string SenderEmail { get; set; }

            [Required(ErrorMessage = "Please enter your message.")]
            public string Message { get; set; }
        }
    }
}