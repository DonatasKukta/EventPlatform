namespace Realizacija.Priklausomybes_inversija_paketas.Pries_inversija_paketas_redaguotas
{
    public class ProfileController
    {
        LoginContext ctx;
        FacebookAuthentification fckAuth;
        Office365Authentification off365Auth;
        LocalAuthentification lclAuth;
    }

    public class LocalAuthentication
    {
        public bool authenticate(LoginContext ctx)
        {
            return false;
        }
    }

    public class FacebookAuthentication
    {
        public bool authenticate(LoginContext ctx)
        {
            return false;
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