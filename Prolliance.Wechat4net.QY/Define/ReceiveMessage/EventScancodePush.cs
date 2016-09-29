using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wechat4net.Define;

namespace Wechat4net.QY.Define.ReceiveMessage
{
    public class EventScancodePush : Base
    {
        public EventScancodePush() { }

        /// <summary>
        /// 事件类型 SCANCODE_PUSH
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// 事件KEY值，由开发者在创建菜单时设定
        /// </summary>
        public string EventKey { get; set; }

        /// <summary>
        /// 扫描类型，一般是qrcode
        /// </summary>
        [XmlProperty("ScanCodeInfo.ScanType")]
        public string ScanType { get; set; }

        /// <summary>
        /// 扫描结果，即二维码对应的字符串信息
        /// </summary>
        [XmlProperty("ScanCodeInfo.ScanResult")]
        public string ScanResult { get; set; }
    }
}
