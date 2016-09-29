using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Wechat4net.Define;
using Wechat4net.QY.Business;
using Wechat4net.QY.Define;
using ReceiveMessage = Wechat4net.QY.Define.ReceiveMessage;
using ReplyMessage = Wechat4net.QY.Define.ReplyMessage;
using ReceiveMessageEnum = Wechat4net.Utils.Enums.QY.ReceiveMessageEnum;
using Wechat4net.QY.Utils;
using Wechat4net.Utils;

namespace Wechat4net.QY
{
    /// <summary>
    /// 微信第三方应用/应用套件管理类
    /// </summary>
    public class AppSuiteManager
    {
        #region 辅助类
        private Logger _Logger = null;
        public Logger Logger
        {
            get
            {
                if (_Logger == null)
                {
                    _Logger = new Logger(AppSettings.LogPath);
                }
                return _Logger;
            }
        }

        private Tencent.WXBizMsgCrypt _Wxcpt = null;
        private Tencent.WXBizMsgCrypt Wxcpt
        {
            get
            {
                if (_Wxcpt == null)
                {
                    _Wxcpt = new Tencent.WXBizMsgCrypt(WechatConfig.SuiteToken, WechatConfig.SuiteEncodingAESKey, WechatConfig.SuiteID);
                }
                return _Wxcpt;
            }
        }

        #endregion

        #region 实例化
        /// <summary>
        /// 实例化AppSuiteManager
        /// </summary>
        public AppSuiteManager()
        {
        }
        #endregion

        #region 属性定义

        #endregion

        #region 接收消息校验、加解密所需参数定义
        /// <summary>
        /// 微信加密签名，msg_signature结合了企业填写的token、请求中的timestamp、nonce参数、加密的消息体
        /// </summary>
        private string MsgSig { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        private string TimeStamp { set; get; }

        /// <summary>
        /// 随机数
        /// </summary>
        private string Nonce { set; get; }

        /// <summary>
        /// 加密的随机字符串，以msg_encrypt格式提供。需要解密并返回echostr明文，解密后有random、msg_len、msg、$CorpID四个字段，其中msg即为echostr明文。
        /// 首次校验时必带。正常消息请求不带。
        /// </summary>
        private string Echostr { set; get; }
        
        #endregion

        #region Processing 开始处理
        /// <summary>
        /// 开始处理
        /// </summary>
        public void Processing()
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Request == null)
            {
                throw new Exception("HttpContext信息错误");
            }
            this.MsgSig = context.Request.QueryString["msg_signature"];
            this.TimeStamp = context.Request.QueryString["timestamp"];
            this.Nonce = context.Request.QueryString["nonce"];
            this.Echostr = context.Request.QueryString["echostr"];

            if (AppSettings.IsDebug)
            {
                //DebugLog
                Logger.Info("MsgSig = " + MsgSig);
                Logger.Info("TimeStamp = " + TimeStamp);
                Logger.Info("Nonce = " + Nonce);
                Logger.Info("Echostr = " + Echostr);
            }

            if (string.IsNullOrEmpty(this.MsgSig) ||
                string.IsNullOrEmpty(this.TimeStamp) ||
                string.IsNullOrEmpty(this.Nonce))
            {
                throw new Exception("HttpContext信息错误");
            }

            int ret = 0;
            #region 微信平台开启回调模式，首次配置服务器信息时，校验服务器URL
            if (!string.IsNullOrEmpty(this.Echostr))
            {
                /*
                 ------------使用示例一：验证回调URL---------------
                 *企业开启回调模式时，企业号会向验证url发送一个get请求 
                 假设点击验证时，企业收到类似请求：
                 * GET /cgi-bin/wxpush?msg_signature=5c45ff5e21c57e6ad56bac8758b79b1d9ac89fd3&timestamp=1409659589&nonce=263014780&echostr=P9nAzCzyDtyTWESHep1vC5X9xho%2FqYX3Zpb4yKa9SKld1DsH3Iyt3tP3zNdtp%2B4RPcs8TgAE7OaBO%2BFZXvnaqQ%3D%3D 
                 * HTTP/1.1 Host: qy.weixin.qq.com

                 * 接收到该请求时，企业应
                 1.解析出Get请求的参数，包括消息体签名(msg_signature)，时间戳(timestamp)，随机数字串(nonce)以及公众平台推送过来的随机加密字符串(echostr),
                   这一步注意作URL解码。
                 2.验证消息体签名的正确性 
                 3.解密出echostr原文，将原文当作Get请求的response，返回给公众平台
                 第2，3步可以用公众平台提供的库函数VerifyURL来实现。
                 */

                string sEchoStr = "";
                ret = Wxcpt.VerifyURL(this.MsgSig, this.TimeStamp, this.Nonce, this.Echostr, ref sEchoStr);
                if (ret != 0)
                {
                    throw new Exception("ERR: VerifyURL Fail, errorCode: " + ret + "。");
                }
                //ret==0表示验证成功，sEchoStr参数表示明文，用户需要将sEchoStr作为get请求的返回参数，返回给企业号。

                context.Response.ContentType = "text/plain";
                context.Response.Write(sEchoStr);
                return;
            }
            #endregion

            //获取post请求内容
            StreamReader postData = new StreamReader(context.Request.InputStream);
            string postDataString = postData.ReadToEnd();

            if (AppSettings.IsDebug) Logger.Info("postDataString = " + postDataString);

            string sMsg = "";  // 解析之后的明文
            //解密
            ret = Wxcpt.DecryptMsg(MsgSig, TimeStamp, Nonce, postDataString, ref sMsg);
            if (ret != 0)
            {
                throw new Exception("ERR: Decrypt Fail, errorCode: " + ret);
            }

            if (AppSettings.IsDebug) Logger.Info("sMsg = " + sMsg);

            //解析XML
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sMsg);
            var root = doc.FirstChild;
            var infoType = root["InfoType"].InnerText;

            if (AppSettings.IsDebug) Logger.Info("infoType = " + infoType);

            var isResponseSuccess = false;
            #region 根据接收消息类型执行相应事件
            switch (infoType)
            {
                case "suite_ticket "://推送suite_ticket协议
                    /*<xml>
		                <SuiteId><![CDATA[wxfc918a2d200c9a4c]]></SuiteId>
		                <InfoType> <![CDATA[suite_ticket]]></InfoType>
		                <TimeStamp>1403610513</TimeStamp>
		                <SuiteTicket><![CDATA[asdfasfdasdfasdf]]></SuiteTicket>
	                </xml>*/
                    break;
                case "change_auth"://变更授权的通知
                    break;
                case "cancel_auth"://取消授权的通知
                    break;
                case "create_auth"://授权成功推送auth_code事件
                    break;
                case "contact_sync"://通讯录变更通知
                    break;
                default:
                    break;
            }
            #endregion

            /*
            * 响应回调
            * 应用提供商在收到推送消息后需要返回字符串success,
            * 授权时返回值不是 success 时，会把返回内容当作错误信息显示（需要以UTF8编码）
            */
            context.Response.ContentType = "text/plain";
            if (isResponseSuccess)
            {
                context.Response.Write("success");
            }
            else
            {
                context.Response.Write("false");
            }
        }
        #endregion

    }

}
