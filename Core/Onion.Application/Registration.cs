using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Bases;
using Onion.Application.Beheviors;
using Onion.Application.Exceptions;
using System.Globalization;
using System.Reflection;

namespace Onion.Application
{
    public static class Registration
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddTransient<ExceptionMiddleware>();
            services.AddMediatR(conf => conf.RegisterServicesFromAssembly(assembly));


            services.AddRulesFromAssemblyContaining(assembly, typeof(BaseRules));
            // bu metod ile BaseRules'dan türetilen tüm class'lar için IoC'de nesne oluşturulacak
            // BaseRules'dan türeyen class'lar business işlerini yürüten class'lardır.


            services.AddValidatorsFromAssembly(assembly);
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("tr");
            // fluent validation'daki hataları türkçe olarak almak için

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehevior<,>));


        }

        private static IServiceCollection AddRulesFromAssemblyContaining(this IServiceCollection services, Assembly assembly, Type type)
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();

            // BaseRules'dan türeyen class'ları bulacak ve listeleyecek
            // type != t burada listelenecek class'ların BaseRules'dan türeyen class'lar olacak demek BaseRules'u dahil etmemesi demek

            foreach (var item in types)
            {
                services.AddTransient(item);
            }

            return services;
        }
    }
}
