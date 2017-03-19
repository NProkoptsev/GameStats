using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using GameStats.Server.Entities;
using GameStats.Server.Storage;
using Newtonsoft.Json;

namespace GameStats.Server.Routing
{
    internal class ServersEndpointMatchesRoute : IRoute
    {
        public void Process(Router router, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var segments = context.Request.RawUrl.Split('/').Skip(1).ToArray();
            var endpoint = HttpUtility.UrlDecode(segments[1], context.Request.ContentEncoding);
            var dateTimestring = HttpUtility.UrlDecode(segments[3], context.Request.ContentEncoding);

            if (endpoint == null || dateTimestring == null)
            {
                ErrorRoute.Process(context, 404);
                return;
            }
            DateTime dateTime;
            if (DateTime.TryParseExact(segments[3], "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime) ==
                false)
            {
                ErrorRoute.Process(context, 404);
                return;
            }
            dateTime = dateTime.ToUniversalTime();
            if (request.HttpMethod.Equals("PUT"))
            {
                var rsp = router.ServerStorage.GetOne(endpoint);
                if (rsp.Equals(""))
                {
                    ErrorRoute.Process(context, 400);
                    return;
                }
                string text;
                using (var reader = new StreamReader(request.InputStream,
                    request.ContentEncoding))
                {
                    text = reader.ReadToEnd();
                }
                var matchData = new MatchData(endpoint, dateTime, JsonConvert.DeserializeObject<MatchInfo>(text));
                router.MatchStorage.Add(matchData);
                router.ServerManager.Add(matchData);
                router.PlayerManager.Add(matchData);
                response.StatusCode = 200;
            }
            else if (request.HttpMethod.Equals("GET"))
            {
                var rsp = router.MatchStorage.GetOne(endpoint, dateTime);
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