using System.Web;
using Wechat4net.QY;

namespace Wechat4net.Demo
{
    /// <summary>
    /// WechatAppSuite 的摘要说明
    /// </summary>
    public class WechatAppSuite : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            AppSuiteManager mgr = new AppSuiteManager();
            mgr.Processing();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}