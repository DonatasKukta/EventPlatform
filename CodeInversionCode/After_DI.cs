namespace Realizacija.Priklausomybes_inversija_paketas
{
    public class ProfileController
    {
        IAuthentication auth;
        LoginContext ctx;
        
        // Constructor injection
        public ProfileController(IAuthentication auth)
        {
            this.auth = auth;
        }

        // Property injection
        public void setAuth(IAuthentication auth)
        {
            if (auth != null)
                this.auth = auth;
            else
                throw new System.ArgumentNullException(" Tried to set auth object to null.");
        }
        // Usage of interface object
        public void useAuthentication()
        {
            // ...
            bool isAuthenticated = false;
            if (this.auth != null)
                throw System.ArgumentNullException(" Tried to perfom authentication while auth object is null.");
            else
                isAuthenticated = auth.authenticate(new LoginContext());
            // ...
        }
    }

    public interface IAuthentication
    {
        bool authenticate(LoginContext ctx);
    }

    public class LocalAuthentication : IAuthentication
    {
        public bool authenticate(LoginContext ctx)
        {
            return false;
        }
    }

    public class FacebookAdapter : IAuthentication
    {
        FacebookAuthentification adaptee;

        public bool authenticate(LoginContext ctx)
        {
            return adaptee.authenticate(ctx);
        }
    }

    public class FacebookAuthentication
    {
        public bool authenticate(LoginContext ctx)
        {
            return false;
        }
    }

    public class Office365Adapter : IAuthentication
    {
        Office365Authentification adaptee;

        public bool authenticate(LoginContext ctx)
        {
            return adaptee.authenticate(ctx);
        }
    }

    public class Office365Authentication
    {
        public bool authenticate(LoginContext ctx)
        {
            return false;
        }
    }

    public class LoginContext
    {
        public String email;
        public String password;
        public String cookies;
        public String username;
        public String IP;
        date timeStamp;
    }

}