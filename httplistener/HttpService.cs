using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace WindowsService
{
    public class HttpService
    {
        private HttpListener listeren = new HttpListener();

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            try
            {
                //指定身份验证 Anonymous匿名访问
                listeren.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                //创建IP地址
                listeren.Prefixes.Add("http://*:30000/posttype/");
                listeren.Start();
                Thread threadlistener = new Thread(new ThreadStart(ThreadStartListener));
                threadlistener.IsBackground = true;
                threadlistener.Start();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 监听连接线程
        /// </summary>
        private void ThreadStartListener()
        {
            while (true)
            {
                //int i = 0;
                // 注意: GetContext 方法将阻塞线程，直到请求到达
                HttpListenerContext context = listeren.GetContext();
                //接收到请求，用线程去处理，防止阻塞
                Thread subThread = new Thread(new ParameterizedThreadStart((currContext) =>
                {
                    var request = (HttpListenerContext)currContext;
                    try
                    {
                        //post请求
                        if (request.Request.HttpMethod.ToLower().Equals("post"))
                        {
                            string json = PostInput(request.Request);
                            Writer("传入参数为"+json, request);
                        }
                        else
                        {
                            Writer("error", request);
                        }
                    }
                    catch (Exception ex)
                    {
                        Writer("error", request);
                    }
                }));
                subThread.Start(context);
            }
        }

        /// <summary>
        /// HttpListener接收post请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string PostInput(HttpListenerRequest request)
        {
            try
            {
                System.IO.Stream s = request.InputStream;
                int count = 0;
                byte[] buffer = new byte[1024];
                StringBuilder builder = new StringBuilder();
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                return builder.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 响应内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="context"></param>
        public void Writer(string str, HttpListenerContext request)
        {
            request.Response.StatusCode = 200;
            request.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            request.Response.ContentType = "application/json";
            request.Response.ContentEncoding = Encoding.UTF8;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = "true", msg = str }));
            request.Response.ContentLength64 = buffer.Length;
            var output = request.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
