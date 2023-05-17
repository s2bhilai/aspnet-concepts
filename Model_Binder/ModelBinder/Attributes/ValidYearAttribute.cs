using ModelBinder.Models;
using System.ComponentModel.DataAnnotations;

namespace ModelBinder.Attributes;

public class ValidYearAttribute:ValidationAttribute
{
    private int _allowedYear;

    public ValidYearAttribute(int allowedYear)
	{
		_allowedYear = allowedYear;
	}

	public string GetErrorMessage() =>
		$"Accepted year is not valid, year must be ({_allowedYear})";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var yrObj = (YearObject)validationContext.ObjectInstance;
        var actionYear = ((YearObject)yrObj!).YearAccepted;

        if(actionYear != _allowedYear)
        {
            return new ValidationResult(GetErrorMessage());
        }

        return ValidationResult.Success;
    }

    
}