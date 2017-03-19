using System.Linq;
using System.Net;
using System.Web;

namespace GameStats.Server.Routing
{
    internal class PlayersStatsRoute : IRoute
    {
        public void Process(Router router, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var segments = context.Request.RawUrl.Split('/').Skip(1).ToArray();
            var name = HttpUtility.UrlDecode(segments[1], context.Request.ContentEncoding);
            if (name == null)
            {
                ErrorRoute.Process(context, 404);
                return;
            }
            if (request.HttpMethod.Equals("GET"))
            {
                var rsp = router.PlayerManager.Get(name);
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