using System;
using Newtonsoft.Json;

namespace Wechat4net.QY.Define
{
    [Serializable]
    public class WechatTag
    {
        /// <summary>
        /// 标签id，整型，指定此参数时新增的标签会生成对应的标签id，不指定时则以目前最大的id自增。
        /// </summary>
        [JsonProperty("tagid")]
        public int TagID { get; set; }

        /// <summary>
        /// 标签名称，长度为1~64个字节，标签名不可与其他标签重名。
        /// </summary>
        [JsonProperty("tagname")]
        public string TagName { get; set; }

        public WechatTag()
        {
            this.TagID = 0;
            this.TagName = "";
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="tagID">标签ID</param>
        /// <param name="tagName">标签名称</param>
        public WechatTag(int tagID,string tagName)
        {
            this.TagID = tagID;
            this.TagName = TagName;
        }
    }
}
