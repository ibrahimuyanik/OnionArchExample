using FluentValidation;

namespace Onion.Application.Features.Products.Command.UpdateProduct
{
    public class UpdateProductCommandValidator: AbstractValidator<UpdateProductCommandRequest>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithName("Başlık");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithName("Açıklama");

            RuleFor(x => x.BrandId)
                .GreaterThan(0)
                .WithName("Marka");

            RuleFor(x => x.Price)
                .GreaterThan(0) // 0'dan büyük olmalıdır
                .WithName("Fiyat");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0) // 0'dan büyük veya eşit olmalıdır
                .WithName("İndirim Oranı");

            RuleFor(x => x.CategoryIds)
                .NotEmpty()
                .Must(categories => categories.Any()) // herhangi bir şey olmalı demek
                .WithName("Kategoriler");
        }
    }
}
