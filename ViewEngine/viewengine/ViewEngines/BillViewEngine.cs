using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace viewengine.ViewEngines;

public class BillViewEngine : IViewEngine
{
    private const string _ViewExtension = ".bill";

    private readonly string[] _viewLocationFormats =
    {
        "Views/{1}/{0}" + _ViewExtension,
        "Views/Shared/{0}" + _ViewExtension
    };

    public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
    {
        if (!context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName))
            throw new Exception("Controller route value not found");

        var checkedLocations = new List<string>();

        foreach (var locationFormat in _viewLocationFormats)
        {
            var possibleViewLocation = string.Format(locationFormat, viewName, controllerName);

            if(File.Exists(possibleViewLocation))
            {
                return ViewEngineResult.Found(viewName, new BillView(possibleViewLocation));
            }

            checkedLocations.Add(possibleViewLocation);
        }

        return ViewEngineResult.NotFound(viewName, checkedLocations);
    }

    public ViewEngineResult GetView(string? executingFilePath, string viewPath, bool isMainPage)
    {
        if (string.IsNullOrEmpty(viewPath) || !viewPath.EndsWith(_ViewExtension, StringComparison.OrdinalIgnoreCase))
        {
            return ViewEngineResult.NotFound(viewPath, Enumerable.Empty<string>());
        }

        if (executingFilePath == null) return ViewEngineResult.NotFound(viewPath, new List<string>());
        var appRelativePath = GetAbsolutePath(executingFilePath, viewPath);

        return File.Exists(appRelativePath)
          ? ViewEngineResult.Found(viewPath, new BillView(appRelativePath))
          : ViewEngineResult.NotFound(viewPath, new List<string> { appRelativePath });
    }

    private static string GetAbsolutePath(string executingFilePath, string viewPath)
    {
        if (IsAbsolutePath(viewPath))
        {
            // An absolute path already; no change required.
            return viewPath.Replace("~/", string.Empty);
        }

        if (string.IsNullOrEmpty(executingFilePath))
        {
            return $"/{viewPath}";
        }

        var index = executingFilePath.LastIndexOf('/');
        return executingFilePath[..(index + 1)] + viewPath;
    }

    private static bool IsAbsolutePath(string name) => name.StartsWith("~/") || name.StartsWith("/");
}