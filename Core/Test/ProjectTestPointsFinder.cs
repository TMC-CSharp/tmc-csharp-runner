using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using TestMyCode.Csharp.API.Attributes;

namespace TestMyCode.Csharp.Core.Test
{
    public class ProjectTestPointsFinder
    {
        private AssemblyLoadContext AssemblyLoadContext { get; }

        private Dictionary<string, HashSet<string>> _Points { get; }

        public ProjectTestPointsFinder()
        {
            this.AssemblyLoadContext = new AssemblyLoadContext("TMC");

            this._Points = new Dictionary<string, HashSet<string>>();
        }

        public void FindPoints(string assemblyPath)
        {
            Assembly assembly = this.AssemblyLoadContext.LoadFromAssemblyPath(assemblyPath);

            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    PointsAttribute pointsAttribute = methodInfo.GetCustomAttribute<PointsAttribute>();
                    if (pointsAttribute is null)
                    {
                        continue;
                    }

                    if (!this._Points.TryGetValue(type.FullName, out HashSet<string> points))
                    {
                        points = this._Points[type.FullName] = new HashSet<string>();
                    }

                    points.Add(pointsAttribute.Name);
                }
            }
        }

        public IReadOnlyDictionary<string, HashSet<string>> Points => this._Points;
    }
}
