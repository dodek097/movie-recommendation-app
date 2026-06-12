using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.Validation;

public sealed class CroatianPhoneAttribute : RegularExpressionAttribute
{
    public const string RegexPattern = @"^\+385\s(?:1\s\d{3}\s\d{4}|[2-5]\d\s\d{3}\s\d{3}|9\d\s\d{3}\s\d{4})$";

    public CroatianPhoneAttribute() : base(RegexPattern)
    {
        ErrorMessage = "Koristi hrvatski format, npr. +385 91 123 4567.";
    }
}
