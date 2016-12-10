using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LGSA_Server.Authentication
{
    public class ErrorResult : IHttpActionResult
    {
        private string _value;
        private HttpRequestMessage _request;

        public ErrorResult(string value, HttpRequestMessage request)
        {
            _value = value;
            _request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(_value),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}