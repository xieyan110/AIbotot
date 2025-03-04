using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AibotLibrary.Config
{
    /// <summary>
    /// 基本配置
    /// </summary>
    public class Basic
    {
        /// <summary>
        /// 联系人通配符
        /// </summary>
        public string friend_name_like { get; set; }

        /// <summary>
        /// 群聊通配符
        /// </summary>
        public string group_name_like { get; set; }

        /// <summary>
        /// 处方分享链接的联系人列表
        /// </summary>
        public string hold_share_contact_name { get; set; }
    }
}
