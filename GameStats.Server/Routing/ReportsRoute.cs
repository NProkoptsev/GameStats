using System.Linq;
using System.Net;

namespace GameStats.Server.Routing
{
    internal class ReportsRoute : IRoute
    {
        public void Process(Router router, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var segments = context.Request.RawUrl.Split('/').Skip(1).ToArray();

            if (request.HttpMethod.Equals("GET"))
            {
                var rsp = segments.Length == 2
                    ? router.ReportsManger.Get(segments[1], null)
                    : router.ReportsManger.Get(segments[1], int.Parse(segments[2]));
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