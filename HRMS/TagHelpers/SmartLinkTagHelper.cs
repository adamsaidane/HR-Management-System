using HRMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HRMS.TagHelpers;

[HtmlTargetElement("smart-link")]
public class SmartLinkTagHelper : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    [ViewContext]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName("entity")]
    public object Entity { get; set; }

    [HtmlAttributeName("class")]
    public string CssClass { get; set; }

    public SmartLinkTagHelper(IUrlHelperFactory urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Entity == null)
        {
            output.SuppressOutput();
            return;
        }

        var (controller, action, routeValues, _) = GetEntityRouteInfo(Entity);

        // Récupère le contenu interne (<strong>...</strong>)
        var childContent = await output.GetChildContentAsync();

        if (string.IsNullOrEmpty(controller))
        {
            output.TagName = "span";
            if (!string.IsNullOrEmpty(CssClass))
                output.Attributes.SetAttribute("class", CssClass);

            output.Content.SetHtmlContent(childContent);
            return;
        }

        var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
        var url = urlHelper.Action(action, controller, routeValues);

        output.TagName = "a";
        output.Attributes.SetAttribute("href", url);

        if (!string.IsNullOrEmpty(CssClass))
            output.Attributes.SetAttribute("class", CssClass);

        // 🔥 clé du problème
        output.Content.SetHtmlContent(childContent);
    }


    private (string Controller, string Action, object RouteValues, string DisplayText) GetEntityRouteInfo(object entity)
    {
        return entity switch
        {
            Employee emp => ("Employees", "Details", new { id = emp.EmployeeId }, $"{emp.FirstName} {emp.LastName}"),
            Department dept => ("Departments", "Details", new { id = dept.DepartmentId }, dept.Name),
            Position pos => ("Departments", "PositionDetails", new { id = pos.PositionId }, pos.Title),
            Candidate cand => ("Recruitment", "CandidateDetails", new { id = cand.CandidateId }, $"{cand.FirstName} {cand.LastName}"),
            _ => (null, null, null, entity.ToString())
        };
    }
}