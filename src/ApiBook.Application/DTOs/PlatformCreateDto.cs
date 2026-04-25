using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBook.Application.DTOs
{
    public sealed record PlatformCreateDto(string Name, string Publisher);
}
