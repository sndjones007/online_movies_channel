using System.Net;
using System.Net.Security;
using System.Security.Principal;

namespace SeleniumTest
{
    class HttpRequestConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Request method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Get or set the credential for accessing the website
        /// </summary>
        public ICredentials Credrntials { get; set; }

        /// <summary>
        /// Get or set the authentication level
        /// </summary>
        public AuthenticationLevel AuthLevel { get; set; }

        /// <summary>
        /// Get or set the impersonation level
        /// </summary>
        public TokenImpersonationLevel ImpersonationLevel { get; set; }

        /// <summary>
        /// Get or set a value that indicates whether to make a persistent 
        /// connection to the Internet resource.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Get or set the value for user agent http headers
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Get or set the security protocol used by serivce point object
        /// </summary>
        public SecurityProtocolType SecurityProtocol { get; set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HttpRequestConfiguration()
        {
            Method = "GET";
            Credrntials = CredentialCache.DefaultNetworkCredentials;
            AuthLevel = AuthenticationLevel.None;
            ImpersonationLevel = TokenImpersonationLevel.None;
            KeepAlive = false;
            UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
            SecurityProtocol = SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        #endregion Constructor
    }
}
