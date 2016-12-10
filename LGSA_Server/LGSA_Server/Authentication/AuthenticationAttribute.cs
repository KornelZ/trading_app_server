using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace LGSA_Server.Authentication
{
    public class AuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private AuthenticationService _service;

        public const string Scheme = "Basic";
        public const string EmptyRequest = "Empty Request";
        public const string InvalidRequest = "Invalid Request";
        public const string InvalidAuthentication = "Authentication Failed";

        public AuthenticationAttribute()
        {
            _service = new AuthenticationService(new DbUnitOfWorkFactory());
        }
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext ctx, CancellationToken token)
        {
            var request = ctx.Request;
            var authorization = request.Headers.Authorization;

            if(authorization == null)
            {
                return;
            }
            if(authorization.Scheme != Scheme)
            {
                return;
            }
            if(string.IsNullOrEmpty(authorization.Parameter))
            {
                ctx.ErrorResult = new ErrorResult(EmptyRequest, request);
                return;
            }
            var credentials = ParseCredentials(authorization.Parameter);
            if(credentials == null)
            {
                ctx.ErrorResult = new ErrorResult(InvalidRequest, request);
                return;
            }
            string id = credentials.Item1;
            string password = credentials.Item2;

            IPrincipal principal = await AuthenticateAsync(id, password, token);
            if(principal == null)
            {
                ctx.ErrorResult = new ErrorResult(InvalidAuthentication, request);
                return;
            }

            ctx.Principal = principal;
        }
        private async Task<IPrincipal> AuthenticateAsync(string id, string password, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            int parsedId = 0;
            if(!int.TryParse(id, out parsedId))
            {
                return null;
            }
            var user = (await _service.GetData(u => u.User_id == parsedId 
                        && u.password == password))?.First();

            if(user == null)
            {
                return null;
            }
            var principal = new UserPrincipal(parsedId, password);
            return principal;
        }
        private Tuple<string, string> ParseCredentials(string parameter)
        {
            byte[] encodedCredentials;
            try
            {
                encodedCredentials = Convert.FromBase64String(parameter);
            }
            catch(FormatException)
            {
                return null;
            }
            var encoding = Encoding.ASCII;
            encoding = (Encoding)encoding.Clone();
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;
            try
            {
                decodedCredentials = encoding.GetString(encodedCredentials);
            }
            catch(DecoderFallbackException)
            {
                return null;
            }
            if(string.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }
            int colonIndex = decodedCredentials.IndexOf(':');
            if(colonIndex == -1)
            {
                return null;
            }
            var id = decodedCredentials.Substring(0, colonIndex);
            var password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(id, password);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext ctx, CancellationToken token)
        {
            var challenge = new AuthenticationHeaderValue(Scheme);
            ctx.Result = new ChallengeOnUnAuthorizedResult(challenge, ctx.Result);
            return Task.FromResult(0);
        }
    }
}