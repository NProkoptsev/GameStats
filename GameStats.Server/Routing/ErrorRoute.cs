using System;
using System.Net;

namespace GameStats.Server.Routing
{
    internal class ErrorRoute
    {
        public static void Process(HttpListenerContext context, int code)
        {
            var response = context.Response;
            var request = context.Request;
            Console.Error.WriteLine("Error " + code + " while perfoming request:  " + request.HttpMethod + "  " +
                                    request.RawUrl + " \r\n");
            response.StatusCode = code;
            response.Close();
        }
    }
}