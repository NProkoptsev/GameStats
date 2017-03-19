using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using GameStats.Server.Entities;
using GameStats.Server.Storage;
using Newtonsoft.Json;

namespace GameStats.Server.Routing
{
    internal class ServersEndpointInfoRoute : IRoute
    {
        public void Process(Router router, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var segments = context.Request.RawUrl.Split('/').Skip(1).ToArray();
            var endpoint = HttpUtility.UrlDecode(segments[1], context.Request.ContentEncoding);
            if (endpoint == null)
            {
                ErrorRoute.Process(context, 404);
                return;
            }
            if (request.HttpMethod.Equals("PUT"))
            {
                string text;
                using (var reader = new StreamReader(request.InputStream,
                    request.ContentEncoding))
                {
                    text = reader.ReadToEnd();
                }
                router.ServerStorage.Add(new ServerData(endpoint, JsonConvert.DeserializeObject<ServerInfo>(text)));
                response.StatusCode = 200;
            }
            else if (request.HttpMethod.Equals("GET"))
            {
                var rsp = router.ServerStorage.GetOne(endpoint);
                if (rsp.Equals(""))
                {
                    ErrorRoute.Process(context, 404);
                    return;
                }
                var buffer = request.ContentEncoding.GetBytes(rsp);
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                response.StatusCode = 200;
            }
            else
            {
                ErrorRoute.Process(context, 404);
                return;
            }
            response.Close();
        }
    }
}