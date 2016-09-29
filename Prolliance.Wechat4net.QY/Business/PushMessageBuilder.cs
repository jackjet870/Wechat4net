using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PushMessageEnum = Wechat4net.Utils.Enums.QY.PushMessageEnum;
using PushMessage = Wechat4net.QY.Define.PushMessage;

namespace Wechat4net.QY.Business
{
    /// <summary>
    /// 推送消息构建类
    /// </summary>
    internal static class PushMessageBuilder
    {
        #region BuildJson 构建推送信息Json字符串
        /// <summary>
        /// 构建推送信息Json字符串
        /// </summary>
        /// <param name="pushMessage">推送信息对象</param>
        /// <returns>未加密的Json字符串</returns>
        public static string BuildJson(PushMessage.Base pushMessage)
        {
            if (pushMessage == null)
            {
                return "";
            }
            if (pushMessage.messageType == PushMessageEnum.Unknow)
            {
                throw new Exception("未知的推送消息类型");
            }

            return JsonConvert.SerializeObject(pushMessage.GetData());
        }

        #endregion
    }
}
