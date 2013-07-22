/*
   Copyright 2012 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/ 
//-CRE-


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Glass.Mapper.Profilers;

namespace Glass.Mapper.Pipelines.ObjectConstruction.Tasks.CreateConcrete
{
    /// <summary>
    /// Class CreateConcreteTask
    /// </summary>
    public class CreateConcreteTask : IObjectConstructionTask
    {
        private const string ConstructorErrorMessage = "No constructor for class {0} with parameters {1}";

        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public void Execute(ObjectConstructionArgs args)
        {
            if (args.Result != null)
                return;

            var type = args.Configuration.Type;

            if (type.IsInterface || args.AbstractTypeCreationContext.IsLazy)
            {
                return;
            }

            //here we create a concrete version of the class
            args.Result = CreateObject(args);
            args.AbortPipeline();
        }

      

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>System.Object.</returns>
        protected virtual object CreateObject(ObjectConstructionArgs args)
        {

            var constructorParameters = args.AbstractTypeCreationContext.ConstructorParameters;

            var parameters =
                constructorParameters == null || !constructorParameters.Any()
                    ? Type.EmptyTypes
                    : constructorParameters.Select(x => x.GetType()).ToArray();

            var constructorInfo = args.Configuration.Type.GetConstructor(parameters);

            Delegate conMethod = args.Configuration.ConstructorMethods[constructorInfo];

            var obj = conMethod.DynamicInvoke(constructorParameters);

            args.Configuration.MapPropertiesToObject(obj, args.Service, args.AbstractTypeCreationContext);

            return obj;
        }
    }
}




