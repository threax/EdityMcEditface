using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edity.Mvc.Auth
{
    /// <summary>
    /// This interface provides the user info to the rest of the system.
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// The unique user name for this account. Must be unique across
        /// all users of the system.
        /// </summary>
        String UniqueUserName { get; }

        /// <summary>
        /// The pretty, displayable name for the user.
        /// </summary>
        String PrettyUserName { get; }

        /// <summary>
        /// The email for the user.
        /// </summary>
        String Email { get; }
    }
}
