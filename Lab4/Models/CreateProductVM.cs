using Microsoft.AspNetCore.Mvc;

public class CreateProductVM
{
    [ModelBinder(BinderType = typeof(RawProductBinder))]
    public string? RawProduct {get;set;}
    

    public string? Name {get;set;}
    public decimal? Price {get;set;}
    public int? Quantity {get;set;}
    public bool? Status {get;set;}
    public IFormFile? ImageFile {get;set;}
}