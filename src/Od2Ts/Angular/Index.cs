﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Od2Ts.Angular
{
    public class Index : AngularRenderable
    {
        public Angular.Package Package {get; private set;}
        public Index(Angular.Package package)
        {
            this.Package = package;
        }
        public override string Name => this.Package.EndpointName;
        public override string NameSpace => "";
        public override string FileName => "index";
        public override string Directory => this.NameSpace;
        public override IEnumerable<string> ImportTypes 
        {
            get { 
                var ns = new List<String>();
                ns.AddRange(Package.Enums.SelectMany(e => e.ImportTypes));
                ns.AddRange(Package.Models.SelectMany(m => m.ImportTypes));
                ns.AddRange(Package.Services.Select(s => s.EdmEntitySet.EntityType));
                ns.AddRange(Package.Module.ImportTypes);
                ns.AddRange(Package.Config.ImportTypes);
                return ns;
            }
        }
        public override IEnumerable<string> ExportTypes => new string[] {};
        public override string Render()
        {
            var exports = this.GetImportRecords().Select(record => $"export * from './{record.From}';");

            return $@"{String.Join("\n", exports)}
export * from './{this.Package.EndpointName.ToLower()}.config';
export * from './{this.Package.EndpointName.ToLower()}.module';";
        }
    }
}
