using FluentValidation;
using MediatR;

namespace Onion.Application.Beheviors
{
    public class FluentValidationBehevior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest: IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validator;

        public FluentValidationBehevior(IEnumerable<IValidator<TRequest>> validator)
        {
            _validator = validator;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            // burada request nesnesine gelen data'ları fluent validation ile doğrulama işlemi yaptık

            var failtures = _validator.Select(v => v.Validate(context))
                                      .SelectMany(result => result.Errors)  // hata alınan birden çok prop olabilir o yüzden selectmany ile seçildi
                                      .GroupBy(x => x.ErrorMessage)  // aynı hata mesajından birden fazla olabilir o yüzden gruplama yaptık
                                      .Select(x => x.First()) 
                                      .Where(f => f != null) // hata alınmayan prop'ları listeden çıkarmak için null olmayanları aldık çünkü hata yoksa error alanı null olur
                                      .ToList();

            if(failtures.Any())
            {
                throw new ValidationException(failtures);
            }

            return next();
        }
    }
}

/*Burada yapılan işlemler MediatR kütüphanesinin özelliklerini kullanarak yapıldı
 * Doğrulama işlemlerini yapmak için FluentValidation kullanılacak
 * CQRS kullandığımız için request ve response nesnelerimiz var bu nesneler ile işlemler yapılacak mesela ürün ekleme, güncelleme vs.
 * 
 * MediatR kütüphanesinin IPipelineBehavior interface'i sayesinde request nesnesi geldiğinde, handle işlemi yapılmadan önce burada yazılan kodlar çalışacak
 * Yani bir nevi interceptor mantığı var (araya girme) bu sayede fluentvalidation'ın doğrulamalarını burada yapması sağladık
 * Eğer fluentvalidation'da doğrulama hatası olursa hata fırlatılacak eğer doğrulama başarılıysa işlem devam edecek.
 * 
 * 
 * Ek olarak burada doğrulama başarılı değilse hata fırlatılıyor 
 * global error handling mekanizmasına fluent validation hatası alındığında ne olacağını eklememiz lazım
 * 
 * Ek olarak birde DI olarak eklenmelidir
 *  services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehevior<,>));
 *  
 *  burada işlemin yapılabilmesi için IoC'ye bu şekilde eklenmelidir.
 *  
 *  
 *  
 *  Eğer request sonucunda response nesnesi döndürülmüyorsa buradaki araya girme işlemi hata verir
 *  bu hatayı çözmek için
 *  1) request nesnesinde public class CreateProductCommandRequest: IRequest<Unit> yazmamız lazım
 *      yani unit döndürülmesi lazım
 *  2) handle sınıfında bütün işlemler bittikten sonra "return Unit.Value;" yazılmalıdır.
 *  
 *  Bu hatanın sebebi MediatR kütüphanesi ile ilgili 
 *  eski sürümlerinde response nesnesi döndürmek zorunluydu hiçbirşey döndürülmeyecekse unit.value yazılıyordu.
 *  yeni sürümlerinde response nesnesi döndürmek zorunda değiliz ama buradaki behevior işleminin çalışabilmesi için eski sürümlerinde olduğu gibi yapmalıyız
 *  
 * 
 * 
 */
