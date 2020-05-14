using System.Collections.Generic;
using System.Linq;
using System;
using ODataApiGen.Models;
using Newtonsoft.Json;
using System.IO;

namespace ODataApiGen.Angular
{
    public class ApiConfig: AngularRenderable, DotLiquid.ILiquidizable 
    {
        public Models.EntityContainer EdmEntityContainer {get; private set;}
        public ApiConfig(EntityContainer container) {
            this.EdmEntityContainer = container;
        }
        public override string FileName => this.EdmEntityContainer.Name.ToLower() + ".api.config";
        public override string Name => this.EdmEntityContainer.Name + "ApiConfig";

        public string Annotations {
            get {
                return JsonConvert.SerializeObject(this.EdmEntityContainer.Annotations.Select(annot => annot.ToDictionary()), Formatting.Indented);
            }
        }
        public string ApiType => this.EdmEntityContainer.FullName;
        public string ApiName => this.EdmEntityContainer.Name;
        // Imports
        public override IEnumerable<string> ImportTypes => new List<string> { this.ApiType };
        public override IEnumerable<string> ExportTypes => new string[] { this.Name };
        public override IEnumerable<Import> Imports => GetImportRecords();
        public override string NameSpace => this.EdmEntityContainer.Namespace;
        public override string Directory => this.NameSpace.Replace('.', Path.DirectorySeparatorChar);
        public override bool Overwrite => true; 
        public object ToLiquid()
        {
            return new {
                Name = this.Name,
                Type = this.Type,
                ApiType = this.ApiType
            };
        }
    }
}