
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ModelBinder.Models;
using System.Text;

namespace ModelBinder.InputFormatter;

public class NatsRecordListFormatter : TextInputFormatter
{
    private const string RECORDSTART = "<<RECORD";

    public NatsRecordListFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/x-natsrecords"));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedEncodings.Add(Encoding.ASCII);
    }

    protected override bool CanReadType(Type type)
    {
        return type == typeof(NatsRecordList);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, 
        Encoding encoding)
    {
        if(context == null)
        { 
            throw new ArgumentNullException(nameof(context));
        }

        if(encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        //Make HttpContext easier to access
        var httpContext = context.HttpContext;

        //If DI resolver has to resolve services
        //var serviceProvider = httpContext.RequestServices;
        //var logger = serviceProvider.GetRequiredService<ILogger<NatsRecordListFormatter>>();

        //Get an asynchronous reader attached to the body and ready to read
        using var bodyReader = new StreamReader(httpContext.Request.Body, encoding);

        //Set some state variables
        bool startFound = false;
        string? inputLine = string.Empty;

        //Find first "<<RECORDSTART"
        while(inputLine != RECORDSTART)
        {
            inputLine = await bodyReader.ReadLineAsync();
            if(inputLine == null)
            {
                return await InputFormatterResult.NoValueAsync();
            }
        }

        startFound = true;

        //set up main result object
        var resultModel = new NatsRecordList();
        var itemList = new Dictionary<string, object>();

        //main records parsing loop
        inputLine = string.Empty;

        while (inputLine != null)
        {
            inputLine = await bodyReader.ReadLineAsync();

            if (inputLine == RECORDSTART)
            {
                resultModel.Records.Add(itemList);
                itemList = new Dictionary<string, object>();
            }
            else
            {
                if (inputLine == null || !inputLine.Contains("=")) continue;
                var parts = inputLine.Split('=');
                itemList.Add(parts[0].Trim(), parts[1].Trim());
            }
        }

        resultModel.Records.Add(itemList);

        return await InputFormatterResult.SuccessAsync(resultModel);

    }
}