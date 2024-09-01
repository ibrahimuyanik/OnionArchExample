using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Mapper.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Mapper
{
    public static class Registration
    {
        public static void AddCustomMapper(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, Onion.Mapper.AutoMapper.Mapper>();
        }
    }
}
