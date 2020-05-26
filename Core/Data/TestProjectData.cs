using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TestMyCode.CSharp.API.Attributes;

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
            //I would prefer using AssemblyLoadContext but then xUnit can't find the assembly
            //xUnit only accepts assembly location and not the assembly itself
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

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

                foreach (MethodInfo methodInfo in type.GetMethods())
                {
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

                    if (!(typeAttribute is null))
                    {
                        points.Add(typeAttribute.Name);
                    }

                    if (!(methodAttribute is null))
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
