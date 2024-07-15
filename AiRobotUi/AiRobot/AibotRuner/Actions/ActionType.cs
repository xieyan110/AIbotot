using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aibot
{
    using System.ComponentModel;

    public enum ActionType
    {
        /// <summary>
        /// 开始
        /// </summary>
        [Description("启动服务")]
        StartServer,

        /// <summary>
        /// 公共
        /// </summary>
        [Description("通用服务")]
        CommonServer,

        /// <summary>
        /// http 相关
        /// </summary>
        [Description("HTTP 相关服务")]
        HttpServer,

        /// <summary>
        /// windows 专属
        /// </summary>
        [Description("Windows 专属服务")]
        WindowsServer,

        /// <summary>
        /// 安卓 专属
        /// </summary>
        [Description("Android 专属服务")]
        AndroidServer,

        /// <summary>
        /// 其他的
        /// </summary>
        [Description("其他服务")]
        OtherServer,
    }
}
