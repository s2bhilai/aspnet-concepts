using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelBinder.Models;
using System.Text;

namespace ModelBinder.ModelBinders;

public class BoundObjectBinder : IModelBinder
{
    public BoundObjectBinder()
    {

    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        //Make sure valid binding context
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        //Get the payload from body of request
        //This will consume all data sent, once you have copy of payload you cannot get the payload
        //again, so make this class global to your binding class if you need to reuse it
        string bodyPayload;
        using(var reader = 
            new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body,Encoding.UTF8))
        {
            bodyPayload = await reader.ReadToEndAsync();
        }

        //Fetch the model name, in the case of class this will set to "",but if your binding a single property 
        //in an object, this should be the property name
        var modelName = bindingContext.ModelName;

        //Body text should always be a number of single line items, seperated by cr/lf with the
        //first item being equal to "<<RECORD"

        //split the payload up into lines
        //For cross platform portability, we split on all 4 carriage return formats(windows,macOS,linux)
        var bodyLines = bodyPayload
            .Split(new string[] { "\n", "\r", "\r\n", "\n\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        if(bodyLines.First() != "<<RECORD")
        {
            //Not a valid data, so force a null object to be handed to an action
            bindingContext.ModelState.TryAddModelError(modelName, "Posted data is not a VALID nats record");
            bindingContext.ModelState.SetModelValue(modelName, null, bodyPayload);//Return null to action method object
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        //Turn the remaining lines into a lookup map
        bodyLines.Remove("<<RECORD");
        var lineMap =
            (from bodyLine in bodyLines
             where bodyLine.Contains('=')
             select bodyLine.Split('=')).ToDictionary(tmp => tmp[0].Trim(), tmp => tmp[1].Trim());

        //Check map for entries we expect to find, and if we find them add them to Result
        var result = new BoundObject();

        //In this example we are looking only for 2 properties, and a specific record type
        bool foundId = false;
        bool foundType = false;

        string wizId = "orwizardid";
        string caseType = "orcasetype";

        if(lineMap.ContainsKey(wizId))
        {
            if (int.TryParse(lineMap[wizId],out int wizardid))
            {
                result.WizardId = wizardid;
                foundId = true;
            }
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(modelName, $"Could not find an {wizId} token in posted data");
        }

        if(lineMap.ContainsKey(caseType))
        {
            result.CaseType = lineMap[caseType];
            foundType = true;
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(modelName, $"Could not find an {caseType} token in posted data");
        }

        bindingContext.ModelState.SetModelValue(modelName, result, bodyPayload);
        bindingContext.Result = ModelBindingResult.Success(result);
    }
}