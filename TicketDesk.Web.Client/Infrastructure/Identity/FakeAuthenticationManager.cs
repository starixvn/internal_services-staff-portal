namespace TicketDesk.Web.Client
{
    using Microsoft.Owin.Security;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class FakeAuthenticationManager : IAuthenticationManager
    {
        // see this: https://simpleinjector.codeplex.com/discussions/564822
        public AuthenticationResponseChallenge AuthenticationResponseChallenge { get; set; }

        public AuthenticationResponseGrant AuthenticationResponseGrant { get; set; }

        public AuthenticationResponseRevoke AuthenticationResponseRevoke { get; set; }

        public ClaimsPrincipal User { get; set; }

        public Task<IEnumerable<AuthenticateResult>> AuthenticateAsync(string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync(string authenticationType)
        {
            throw new NotImplementedException();
        }

        public void Challenge(params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public void Challenge(AuthenticationProperties properties, params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes(
            Func<AuthenticationDescription, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes()
        {
            throw new NotImplementedException();
        }

        public void SignIn(params ClaimsIdentity[] identities)
        {
            // Method intentionally left empty.
        }

        public void SignIn(AuthenticationProperties properties, params ClaimsIdentity[] identities)
        {
            // Method intentionally left empty.
        }

        public void SignOut(params string[] authenticationTypes)
        {
            // Method intentionally left empty.
        }

        public void SignOut(AuthenticationProperties properties, params string[] authenticationTypes)
        {
            // Method intentionally left empty.
        }
    }
}