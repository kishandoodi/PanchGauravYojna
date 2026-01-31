using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace PanchGauravYojna.Service
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Download-Options", "noopen");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            //context.Response.Headers.Add("X-MS-InvokeApp", "1; RequireReadOnly");
            //context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "master-only");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            //context.Response.Headers["Referrer-Policy"] = "same-origin";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            // Remove the Server header
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("Server");


            //var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            //context.Items["CSPNonce"] = nonce;
            //context.Response.Headers.Add("Content-Security-Policy",
            // $"default-src 'self'; script-src 'self' 'nonce-{nonce}'; style-src 'self'; object-src 'none'; font-src 'self'; img-src 'self'; connect-src 'self'; frame-ancestors 'none';");
            //context.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'nonce-{nonce}'; object-src 'none'; default-src 'self'");
            //Content-Security-Policy with frame-ancestors
            //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; object-src 'none'; script-src 'self'; style-src 'self'; frame-ancestors 'none'");
            //context.Response.Headers.Add("Content-Security-Policy", "default-src*;'self'; 'unsafe-inline' 'unsafe-eval';");

            //var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            //context.Items["ScriptNonce"] = nonce;
            //context.Response.Headers.Add("Content-Security-Policy",
            //$"default-src 'self'; script-src 'self' 'nonce-{nonce}'; style-src 'self' 'nonce-{nonce}'; font-src 'self' 'nonce-{nonce}'; img-src 'self' data:; connect-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self'; form-action 'self';");

            var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            context.Items["CSPNonce"] = nonce;

            context.Response.Headers.Add("Content-Security-Policy", $@"
                default-src 'self';
                script-src 'self' 'nonce-{nonce}' 'strict-dynamic';
                style-src 'self' 'nonce-{nonce}';  
                img-src 'self' data:;
                font-src 'self' data:;
                connect-src 'self';
                form-action 'self' https://ssotest.rajasthan.gov.in;                
                object-src 'none';
                base-uri 'self'; 
               frame-src 'self' data:;
               frame-ancestors 'self';   
            ".Replace(Environment.NewLine, ""));

            //context.Response.Headers["Content-Security-Policy"] = cspHeader;

            //context.Response.Headers.Add("Content-Security-Policy", $@"
            //    default-src 'self';
            //    script-src 'self' 'nonce-{nonce}' 'strict-dynamic';
            //    style-src 'self' 'nonce-{nonce}';
            //    connect-src 'self';
            //    form-action 'self';
            //    img-src 'self' data:;
            //    font-src 'self';
            //".Replace(Environment.NewLine, ""));

            await _next(context);          
        }
    }
}
