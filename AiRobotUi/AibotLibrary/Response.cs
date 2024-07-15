namespace AibotLibrary
{
    public class Response
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool ok { get; set; } = false;

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; } = "";

        /// <summary>
        /// 错误代码
        /// </summary>
        public string code { get; set; } = "";

        /// <summary>
        /// 返回值
        /// </summary>
        public object data { get; set; } = "";

        /// <summary>
        /// 其他信息
        /// </summary>
        public object ext { get; set; } = "";

        /// <summary>
        /// 更多信息
        /// </summary>
        public object more { get; set; } = "";

        public Response()
        {
            ok = false;
            msg = "";
            code = "";
            data = null;
        }

        public Response(bool _ok, string _msg)
        {
            this.ok = _ok;
            this.msg = _msg;
        }

        public Response(bool _ok, object _data)
        {
            this.ok = _ok;
            this.data = _data;
            this.msg = "";
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="_ok"></param>
        /// <param name="_data"></param>
        /// <param name="_msg"></param>
        public Response(bool _ok, object _data, string _msg = "")
        {
            ok = _ok;
            msg = _msg;
            data = _data;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ok"></param>
        /// <param name="_data"></param>
        /// <param name="_ext"></param> 
        public Response(bool _ok, object _data, object _ext)
        {
            ok = _ok;
            data = _data;
            ext = _ext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ok"></param>
        /// <param name="_msg"></param>
        /// <param name="_code"></param>
        /// <param name="_data"></param>
        public Response(bool _ok, string _msg, string _code, object _data)
        {
            ok = _ok;
            msg = _msg;
            code = _code;
            data = _data;
        }

        public Response(bool _ok, string _msg, string _code)
        {
            ok = _ok;
            msg = _msg;
            code = _code;
        }

        public override string ToString()
        {
            string str = string.Format("ok={0};msg={1};code={2};data={3}",
                    ok,
                    msg == null ? "" : msg,
                    code == null ? "" : code,
                    data == null ? "" : data
                );
            return str;
        }
    }
}