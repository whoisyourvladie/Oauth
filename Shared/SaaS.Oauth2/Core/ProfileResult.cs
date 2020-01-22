using Newtonsoft.Json;

namespace SaaS.Oauth2.Core
{
    public class ProfileResult
    {
        public ProfileResult(FacebookProfileResult profile) : this(profile.Id, profile.Email, profile.FirstName, profile.LastName) { }
        public ProfileResult(GoogleProfileResult profile) : this(profile.Id, profile.Email, profile.FirstName, profile.LastName) { }
        public ProfileResult(MicrosoftProfileResult profile) : this(profile.Id, profile.Email, profile.FirstName, profile.LastName) { }

        public ProfileResult(string id, string email, string firstName, string lastName)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public string Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class BaseProfileResult
    {
        public string Id { get; set;}
    }

    public class FacebookProfileResult: BaseProfileResult
    {
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    public class GoogleProfileResult: BaseProfileResult
    {
        public string Email { get; set; }

        [JsonProperty("given_name")]
        public string FirstName { get; set; }

        [JsonProperty("family_name")]
        public string LastName { get; set; }
    }

    public class MicrosoftProfileResult: BaseProfileResult
    {
        [JsonProperty("userPrincipalName")]
        public string Email { get; set; }

        [JsonProperty("givenName")]
        public string FirstName { get; set; }

        [JsonProperty("surname")]
        public string LastName { get; set; }
    }
}
