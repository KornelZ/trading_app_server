using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LGSA_Server.Authentication
{
    public class ChallengeOnUnAuthorizedResult : IHttpActionResult
    {
        public AuthenticationHeaderValue Challenge { get; set; }
        public IHttpActionResult Result { get; set; }
        public ChallengeOnUnAuthorizedResult(AuthenticationHeaderValue challenge,
                                             IHttpActionResult result)
        {
            Challenge = challenge;
            Result = result;
        }
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await Result.ExecuteAsync(cancellationToken);

            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if(!response.Headers.WwwAuthenticate.Any(h => h.Scheme == Challenge.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(Challenge);
                }
            }
            return response;
        }
    }
}