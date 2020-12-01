using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using TestMyCode.CSharp.API.Attributes;
using Xunit;

namespace TestMyCode.CSharp.Core.Data
{
    public class TestProjectData
    {
        private readonly List<Assembly> _Assemblies;

        private readonly Dictionary<string, HashSet<string>> _Points;

        public TestProjectData()
        {
            this._Assemblies = new List<Assembly>();

            this._Points = new Dictionary<string, HashSet<string>>();
        }

        public void LoadProject(string assemblyPath)
        {
            AssemblyLoadContext alc = new AssemblyLoadContext("TestProjectLoader", isCollectible: true);

            alc.Resolving += (sender, args) =>
            {
                string assemblyName = $"{args.Name}.dll";
                string dir = Path.GetDirectoryName(assemblyPath)!;

                string assemblyFile = Path.Combine(dir, assemblyName);
                if (File.Exists(assemblyFile))
                {
                    return sender.LoadFromAssemblyPath(assemblyFile);
                }

                return null;
            };

            Assembly assembly = alc.LoadFromAssemblyPath(assemblyPath);

            this.LoadAssembly(assembly);
        }

        public void LoadAssembly(Assembly assembly)
        {
            this.FindPoints(assembly);

            this._Assemblies.Add(assembly);
        }

        private void FindPoints(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                PointsAttribute? typeAttribute = type.GetCustomAttribute<PointsAttribute>();

                foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance
                                                                  | BindingFlags.Static //xUnit allows static methods
                                                                  | BindingFlags.Public
                                                                  | BindingFlags.NonPublic)) //xUnit allows private methods
                {
                    //TheoryAttribute inherits from FactAttribute
                    FactAttribute? factAttribute = methodInfo.GetCustomAttribute<FactAttribute>();
                    if (factAttribute is null)
                    {
                        continue;
                    }

                    PointsAttribute? methodAttribute = methodInfo.GetCustomAttribute<PointsAttribute>();
                    if (typeAttribute is null && methodAttribute is null)
                    {
                        continue;
                    }

                    string fullName = $"{type.FullName}.{methodInfo.Name}";

                    if (!this._Points.TryGetValue(fullName, out HashSet<string>? points))
                    {
                        points = this._Points[fullName] = new HashSet<string>(0);
                    }

                    if (typeAttribute is not null)
                    {
                        points.Add(typeAttribute.Name);
                    }

                    if (methodAttribute is not null)
                    {
                        points.Add(methodAttribute.Name);
                    }
                }
            }
        }

        public IReadOnlyList<Assembly> Assemblies => this._Assemblies;
        public IReadOnlyDictionary<string, HashSet<string>> Points => this._Points;
    }
}
