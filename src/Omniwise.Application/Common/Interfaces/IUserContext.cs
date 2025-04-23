using Omniwise.Application.Common.Types;

namespace Omniwise.Application.Common.Interfaces;

public interface IUserContext
{
    CurrentUser GetCurrentUser();
}
