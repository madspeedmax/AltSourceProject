using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BankApi.Models;

namespace BankApi.Aspects
{
    [Serializable]
    public class BankApiAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            try
            {
                var headers = HttpContext.Current.Request.Headers;
                HttpContext.Current.Items["beginRequestTime"] = DateTime.UtcNow;

                var request = HttpContext.Current.Request;
                var method = args.Method.Name;
                request.InputStream.Position = 0;
                var bytes = new byte[request.InputStream.Length];
                request.InputStream.Read(bytes, 0, bytes.Length);
                request.InputStream.Position = 0;
                var contentType = request.Params.Get("CONTENT_TYPE");
                var requestString = Encoding.ASCII.GetString(bytes);
                if (contentType == "application/x-www-form-urlencoded")
                {
                    requestString = HttpUtility.UrlDecode(requestString);
                }

                HttpContext.Current.Items["request"] = requestString;
                 
                string username;
                string password;
                GetCredentialsFromAuthHeader(headers["Authorization"], out username, out password);

                if (!UserList.Users.Any(u => u.Email == username && u.Password == password))
                {
                    throw new Exception("Unauthorized", null);
                }
            }

            catch
            {
                throw new Exception("Unauthorized", null);
            }
        }

        public void GetCredentialsFromAuthHeader(string authHeader, out string username, out string password)
        {
            username = null;
            password = null;
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
            {
                authHeader = authHeader.Substring("Basic ".Length);
            }

            if (!string.IsNullOrEmpty(authHeader))
            {

                authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

                var tokens = authHeader.Split(':');
                if (tokens.Length >= 2)
                {
                    username = tokens[0];
                    password = tokens[1];
                }
            }
        }
    }
}