using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wechat4net.QY;
using Newtonsoft.Json;

namespace Wechat4net.Demo
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var aaa = JsonConvert.DeserializeObject<Wechat4net.QY.Define.Contact.QuiryTagListReturnValue>("{ \"taglist\":[{\"tagid\":1,\"tagname\":\"仍然\"}],\"errcode\":0,\"errmsg\":\"ok\"}");

            var aa = ContactManager.GetTagList();
        }
    }
}