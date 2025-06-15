namespace WebApiApp.Library
{
    /// <summary>
    /// SEE: https://medium.com/@sametkarademir244/jwtbearerevents-in-asp-net-core-managing-token-events-04cdeb9dc30d
    /// </summary>
    public class ApiClientJwtBearerEvents : JwtBearerEvents
    {
        /// <summary>
        /// Invoked if exceptions are thrown during request processing. 
        /// The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                var ExpiredException = context.Exception as SecurityTokenExpiredException;

                string DT = JsonSerializer.Serialize(ExpiredException.Expires);

                // add a custom header
                context.Response.Headers["X-Token-Expired"] = DT;
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Invoked if Authorization fails and results in a Forbidden response
        /// </summary>
        public override Task Forbidden(ForbiddenContext context)
        {
            return base.Forbidden(context);
        }
        /// <summary>
        /// The <see cref="JwtBearerEvents.MessageReceived"/> event gives the application an opportunity 
        /// to get the token from a different location, adjust, or reject the token.
        /// <para>The application may set the Context.Token in the OnMessageReceived. Otherwise Context.Token is null.</para>
        /// <para>SEE: https://stackoverflow.com/a/54497616/1779320        </para>
        /// </summary>
        public override Task MessageReceived(MessageReceivedContext context)
        {
            return base.MessageReceived(context);
        }
        /// <summary>
        /// The <see cref="JwtBearerEvents.TokenValidated"/> is called 
        /// after the passed in <see cref="TokenValidatedContext.SecurityToken"/> is loaded and validated successfully.
        /// </summary>
        public override Task TokenValidated(TokenValidatedContext context)
        {
            ClaimsPrincipal Principal = context.Principal;

            List<Claim> ClaimList = ClaimHelper.GetClaimList(context.SecurityToken);

            string JtiValue = ClaimHelper.GetClaimValue(ClaimList, JwtRegisteredClaimNames.Jti);

            if (string.IsNullOrWhiteSpace(JtiValue))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Fail("Invalid Token. A valid Access Token is required.");
            }
            else
            {
                string JtiCacheKey = TokenHelper.GetJtiCacheKey(JtiValue);
                if (!Lib.Cache.ContainsKey(JtiCacheKey))
                {
                    throw new SecurityTokenException("Token has expired.");
                }
            }

            return base.TokenValidated(context);
        }
        /// <summary>
        /// Invoked before a challenge is sent back to the caller.
        /// </summary>
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            DataResult Result = new();

            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            // error string is a must
            if (string.IsNullOrWhiteSpace(context.Error))
                context.Error = "Not Authenticated.";

            // error description string is a must
            if (string.IsNullOrWhiteSpace(context.ErrorDescription))
                context.ErrorDescription = "Not Authenticated. Invalid Token or not Token at all. A valid Access Token is required.";

            Result.NotAuthenticated();

            // Add some extra context for expired tokens.
            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
            {
                var ExpiredException = context.AuthenticateFailure as SecurityTokenExpiredException;

                string DT = JsonSerializer.Serialize(ExpiredException.Expires);
                string Description = $"The Access Token is expired on {DT}";

                context.ErrorDescription = Description;

                // add a custom header
                context.Response.Headers["X-Token-Expired"] = DT;

                string ErrorMessage = $"{Description}: {context.AuthenticateFailure.Message}";
            }

            string JsonText = JsonSerializer.Serialize(Result);
            return context.Response.WriteAsync(JsonText);
        }
    }
}
