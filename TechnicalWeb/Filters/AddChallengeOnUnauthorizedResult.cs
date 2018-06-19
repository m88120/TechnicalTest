using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Http;

namespace Technical.Filters
{
    public class AddChallengeOnUnauthorizedResult : IActionResult
    {
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        public AuthenticationHeaderValue Challenge { get; private set; }

        public IActionResult InnerResult { get; private set; }

        //public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        //{
        //    //HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken);

        //    //if (response.StatusCode == HttpStatusCode.Unauthorized)
        //    //{
        //    //    response.StatusCode = HttpStatusCode.Forbidden;
        //    //    // Only add one challenge per authentication scheme.
        //    //    if (!response.Headers.WwwAuthenticate.Any((h) => h.Scheme == Challenge.Scheme))
        //    //    {
        //    //        response.Headers.WwwAuthenticate.Add(Challenge);
        //    //    }
        //    //}

        //    //return response;
        //    throw new NotImplementedException();
        //}

        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}