using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Constants
    {
        public const string Issuer = Audience;
        public const string Audience = "https://localhost:5001/";

        // Store in a vault
        public const string Secret = "just_a_little_secret_to_test_functionality_with";
    }
}
