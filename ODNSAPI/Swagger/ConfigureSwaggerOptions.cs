﻿namespace ODNSAPI.Swagger
{
    using Asp.Versioning.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration config) 
        { 
            this.provider = provider; 
            this.configuration = config;
        }

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var text = new StringBuilder(this.configuration.GetValue<string>("Settings:DocsSwaggerOptions:HeaderDescription"));
            var info = new OpenApiInfo()
            {
                Title = this.configuration.GetValue<string>("Settings:DocsSwaggerOptions:Title"),
                Version = description.ApiVersion.ToString(),
                Contact = new OpenApiContact() { 
                    Name = this.configuration.GetValue<string>("Settings:DocsSwaggerOptions:ContactName"), 
                    Email = this.configuration.GetValue<string>("Settings:DocsSwaggerOptions:ContactEmail")
                }//,
                //License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };

            if (description.IsDeprecated)
            {
                text.Append("<br/> This API version has been deprecated.");
            }

            if (description.SunsetPolicy is { } policy)
            {
                if (policy.Date is { } when)
                {
                    text.Append(" The API will be sunset on ")
                        .Append(when.Date.ToShortDateString())
                        .Append('.');
                }

                if (policy.HasLinks)
                {
                    text.AppendLine();

                    var rendered = false;

                    for (var i = 0; i < policy.Links.Count; i++)
                    {
                        var link = policy.Links[i];

                        if (link.Type == "text/html")
                        {
                            if (!rendered)
                            {
                                text.Append("<h4>Links</h4><ul>");
                                rendered = true;
                            }

                            text.Append("<li><a href=\"");
                            text.Append(link.LinkTarget.OriginalString);
                            text.Append("\">");
                            text.Append(
                                StringSegment.IsNullOrEmpty(link.Title)
                                ? link.LinkTarget.OriginalString
                                : link.Title.ToString());
                            text.Append("</a></li>");
                        }
                    }

                    if (rendered)
                    {
                        text.Append("</ul>");
                    }
                }
            }

            text.Append("<h4>Additional Information</h4>");
            info.Description = text.ToString();

            return info;
        }
    }
}
