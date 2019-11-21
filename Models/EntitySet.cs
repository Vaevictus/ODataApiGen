﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ODataApiGen.Models
{
    public class EntitySet
    {
        public EntitySet(XElement xElement)
        {
            Name = xElement.Attribute("Name")?.Value;
            EntityType = xElement.Attribute("EntityType")?.Value;

            NavigationPropertyBindings = xElement.Descendants().Where(a => a.Name.LocalName == "NavigationPropertyBinding")
                .Select(navPropBind => new NavigationPropertyBinding()
                {
                    Path = navPropBind.Attribute("Path").Value,
                    Target = navPropBind.Attribute("Target").Value,
                }).ToList();
                
            NameSpace =
                xElement.Ancestors().FirstOrDefault(a => a.Attribute("Namespace") != null)?.Attribute("Namespace").Value;
        }

        public void AddActions(IEnumerable<ActionImport> actionImports, IEnumerable<Action> actions) {
            Actions = actionImports
                .Where(a => a.EntitySet == Name)
                .Select(ai => ai.Action)
                .Union(actions.Where(a => a.IsBound && a.BindingParameter == EntityType));
        }
        public void AddFunctions(IEnumerable<FunctionImport> functionImports, IEnumerable<Function> functions) {
            Functions = functionImports
                .Where(a => a.EntitySet == Name)
                .Select(fi => fi.Function)
                .Union(functions.Where(f => f.IsBound && f.BindingParameter == EntityType));
        }
        public string Name { get; private set; }
        public string NameSpace { get; private set; }
        public string FullName => $"{this.NameSpace}.{this.Name}";
        public string EntityType { get; private set; }
        public IEnumerable<Action> Actions { get; set; }
        public IEnumerable<Function> Functions { get; set; }
        public IEnumerable<NavigationPropertyBinding> NavigationPropertyBindings { get; set; }
    }
}
