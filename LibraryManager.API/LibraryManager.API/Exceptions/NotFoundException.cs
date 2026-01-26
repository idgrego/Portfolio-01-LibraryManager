using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;

namespace LibraryManager.API.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string entityName, string key) 
            : base($"{entityName} com ID {key} não foi encontrado.", HttpStatusCode.NotFound)
        { }
    }
}
