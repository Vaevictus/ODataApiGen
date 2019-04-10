using System;
using System.Collections.Generic;
using System.Linq;
using Od2Ts.Abstracts;
using Od2Ts.Interfaces;

namespace Od2Ts.Angular {
    public class Model : Renderable, IHasImports {
        public StructuredType EdmStructuredType {get; private set;}

        public Model Base {get; set;}
        public bool UseInterfaces {get; set;} = false;

        public Model(StructuredType type, bool inter) {
            EdmStructuredType = type;
            UseInterfaces = inter;
        }

        public override string Render() {
            var properties = EdmStructuredType.Properties.Select(prop =>
                $"{prop.Name}" + (prop.IsRequired ? ":" : "?:") + $" {this.GetTypescriptType(prop.Type)};");

            var navigations = EdmStructuredType.NavigationProperties.Select(nav =>
                $"{nav.Name}" + 
                (nav.IsRequired ? ":" : "?:") + 
                $" {this.GetTypescriptType(nav.Type)}" + (nav.IsCollection ? "[];" : ";"));

            var imports = this.RenderImports(this);
            return $@"{String.Join("\n", imports)}
export {this.GetModelSignature()} {{
  /* Navigation properties */
  {String.Join("\n  ", navigations)}
  /* Properties */
  {String.Join("\n  ", properties)}
}}"; 
        }

        public string GetModelSignature() {
            var signature = $"{GetModelType()} {this.EdmStructuredType.Name}";
            if (this.Base != null)
                signature = $"{signature} extends {this.Base.EdmStructuredType.Name}";
            return signature;
        }

        public string GetModelType() {
            return this.UseInterfaces ? "interface" : "class";
        }

        public IEnumerable<Import> Imports
        {
            get
            {
                var namespaces = this.EdmStructuredType.NavigationProperties
                    .Select(a => a.Type)
                    .Where(a => a != this.EdmStructuredType.Type)
                    .ToList();
                /*For Not-EDM types (e.g. enums with namespaces, complex types*/
                namespaces.AddRange(this.EdmStructuredType.Properties
                    .Where(a => !a.IsEdmType)
                    .Select(a => a.Type));
                if (this.Base != null)
                    namespaces.Add(this.Base.EdmStructuredType.Type);
                return namespaces.Distinct().Select(a => new Import(this.BuildUri(a)));
            }
        }

        private Uri _uri;
        public Uri Uri => _uri ?? (_uri = new Uri("r://" + this.EdmStructuredType.NameSpace.Replace(".", "/") + "/" + this.EdmStructuredType.Name, UriKind.Absolute));

        public override string Name => this.EdmStructuredType.Name;

        public override string NameSpace => this.EdmStructuredType.NameSpace;
    }
}