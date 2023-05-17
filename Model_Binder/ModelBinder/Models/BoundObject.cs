using Microsoft.AspNetCore.Mvc;
using ModelBinder.ModelBinders;

namespace ModelBinder.Models;


[ModelBinder(BinderType = typeof(BoundObjectBinder))]
public class BoundObject
{
    public int WizardId { get; set; }
    public string CaseType { get; set; } = "";
}
