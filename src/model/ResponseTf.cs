using AutoRest.Core.Model;
using System.Diagnostics;
using System.Net;

namespace AutoRest.Terraform
{
    internal class ResponseTf
    {
        public ResponseTf(HttpStatusCode status, Response response)
        {
            Debug.Assert(response != null);
            HttpStatus = status;
            Response = response;
        }

        public HttpStatusCode HttpStatus { get; }

        public IModelType BodyType => Response.Body;

        private Response Response { get; }
    }
}
