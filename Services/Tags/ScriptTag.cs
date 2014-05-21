﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DotLiquid;
using Orchard;
using Orchard.UI.Resources;

namespace Lombiq.LiquidMarkup.Services.Tags
{
    public class ScriptTag : Tag
    {
        private string _url;
        private ResourceLocation _location = ResourceLocation.Foot;


        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            base.Initialize(tagName, markup, tokens);

            var parametersCommaSeparated = markup.Trim();
            var parameters = parametersCommaSeparated
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.Trim().Trim('"', '\''));

            if (!parameters.Any()) return;

            _url = parameters.First();

            if (parameters.Count() == 2 && parameters.Last() == "head")
            {
                _location = ResourceLocation.Head;
            }
        }

        public override void Render(Context context, TextWriter result)
        {
            if (HttpContext.Current == null || string.IsNullOrEmpty(_url)) return;

            var wc = HttpContext.Current.Request.RequestContext.GetWorkContext();

            if (wc == null) return;

            var script = wc.Resolve<IResourceManager>().Include("script", _url, _url);
            script.Location = _location;
        }
    }
}