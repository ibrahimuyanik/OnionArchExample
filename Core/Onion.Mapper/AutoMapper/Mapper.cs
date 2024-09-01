using AutoMapper;
using AutoMapper.Internal;

namespace Onion.Mapper.AutoMapper
{
    public class Mapper : Application.Interfaces.AutoMapper.IMapper
    {
        public static List<TypePair> typePairs = new();
        // TypePair AutoMapper'dan geldi.
        private IMapper MapperContainer;
        // buradaki IMapper'da AutoMapper'dan geldi bizim oluşturduğumuz değil


        public TDestination Map<TDestination, TSource>(TSource source, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);

            return MapperContainer.Map<TSource, TDestination>(source);
        }

        public IList<TDestination> Map<TDestination, TSource>(IList<TSource> source, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);

            return MapperContainer.Map<IList<TSource>, IList<TDestination>>(source);
        }

        public TDestination Map<TDestination>(object source, string? ignore = null)
        {
            Config<TDestination, object>(5, ignore);

            return MapperContainer.Map<TDestination>(source);
        }

        public IList<TDestination> Map<TDestination>(IList<object> source, string? ignore = null)
        {
            Config<TDestination, IList<object>>(5, ignore);

            return MapperContainer.Map<IList<TDestination>>(source);
        }

        protected void Config<TDestination, TSource>(int depth = 5, string? ignore = null)
        {
            // parametredeki depth değeri iç içe dto nesneleri olursa onlarıda map'lemek için 
            // 5 tane iç içe dto varsa onları map'ler 5'den fazla olursa map'leyemez.
            // Örnek productDto'nun içinde categoryDto nesnesi var(property olarak) onun içinde de başka dto'lar var gibi

            var typePair = new TypePair(typeof(TSource), typeof(TDestination));

            if (typePairs.Any(a => a.DestinationType == typePair.DestinationType && a.SourceType == typePair.SourceType) && ignore is null)
                return;

            typePairs.Add(typePair);

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var item in typePairs)
                {
                    if (ignore is not null)
                        cfg.CreateMap(item.SourceType, item.DestinationType).MaxDepth(depth).ForMember(ignore, x => x.Ignore()).ReverseMap();
                    else
                        cfg.CreateMap(item.SourceType, item.DestinationType).MaxDepth(depth).ReverseMap();


                }
            });

            MapperContainer = config.CreateMapper();

        }
    }
}
