﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotLiquid;
using Lombiq.LiquidMarkup.Models;
using Lombiq.LiquidMarkup.Services.Filters;
using Lombiq.LiquidMarkup.Services.Tags;
using Orchard.Caching.Services;
using Orchard.DisplayManagement.Shapes;

namespace Lombiq.LiquidMarkup.Services
{
    public class LiquidTemplateService : ILiquidTemplateService
    {
        private static bool _templateIsConfigured;

        private readonly ICacheService _cacheService;


        public LiquidTemplateService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        
    
        public string ExecuteTemplate(string liquidSource, dynamic model)
        {
            EnsureTemplateConfigured();

            var templateModel = new StaticShape(model);

            var liquidTemplate = _cacheService.Get(liquidSource, () => Template.Parse(liquidSource));
            return liquidTemplate.Render(new RenderParameters
            {
                LocalVariables = Hash.FromAnonymousObject(new { Model = templateModel }),
                RethrowErrors = true
            });
        }

        public void VerifySource(string liquidSource)
        {
            EnsureTemplateConfigured();

            Template.Parse(liquidSource);
        }


        // This method potentially runs from multiple threads, also the first time but this is safe to do so.
        private static void EnsureTemplateConfigured()
        {
            if (_templateIsConfigured) return;

            // Currently only global configuration is possible, see: https://github.com/formosatek/dotliquid/issues/93
            Template.NamingConvention = new DotLiquid.NamingConventions.CSharpNamingConvention();
            Template.RegisterSafeType(typeof(ShapeMetadata), new[] { "Type", "DisplayType", "Position", "PlacementSource", "Prefix", "Wrappers", "Alternates", "WasExecuted" });
            Template.RegisterTag<StyleTag>("style");
            Template.RegisterTag<StyleTag>("stylerequire");
            Template.RegisterTag<ScriptTag>("script");
            Template.RegisterTag<ScriptTag>("scriptrequire");
            Template.RegisterTag<DisplayTag>("display");
            Template.RegisterTag<DisplayTag>("Display");
            Template.RegisterFilter(typeof(DisplayFilter));

            _templateIsConfigured = true;
        }
    }
}