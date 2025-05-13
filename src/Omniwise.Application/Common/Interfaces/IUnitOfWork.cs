using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniwise.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task ExecuteTransactionalAsync(Func<Task> action);
}
