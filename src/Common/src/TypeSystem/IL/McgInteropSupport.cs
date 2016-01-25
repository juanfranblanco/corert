// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

using Internal.TypeSystem;
using Debug = System.Diagnostics.Debug;

namespace Internal.IL
{
    /// <summary>
    /// Provides compilation hooks for interop code generated by tool
    /// </summary>
    public static class McgInteropSupport
    {
        /// <summary>
        /// Assembly name suffix for pregenerated interop code.
        /// </summary>
        private const string AssemblyNameSuffix = ".McgInterop";

        /// <summary>
        /// Returns true if <paramref name="method"/> is pregenerated interop code
        /// </summary>
        public static bool IsPregeneratedInterop(MethodDesc method)
        {
            var metadataType = (MetadataType)method.OwningType;
            var module = metadataType.Module;

            var assemblyName = ((IAssemblyDesc)module).GetName();
            var simpleName = assemblyName.Name;

            return simpleName.EndsWith(AssemblyNameSuffix);
        }

        /// <summary>
        /// Returns pregenerated interop code for given PInvoke method if one exist
        /// </summary>
        public static MethodDesc TryGetPregeneratedPInvoke(MethodDesc method)
        {
            Debug.Assert(method.IsPInvoke);

            var metadataType = (MetadataType)method.OwningType;
            var module = metadataType.Module;

            var assemblyName = ((IAssemblyDesc)module).GetName();

            var interopAssemblyName = new AssemblyName();

            interopAssemblyName.Name = assemblyName.Name + AssemblyNameSuffix;
            interopAssemblyName.Version = assemblyName.Version;
            interopAssemblyName.SetPublicKeyToken(interopAssemblyName.GetPublicKeyToken());
            interopAssemblyName.CultureName = assemblyName.CultureName;
            interopAssemblyName.ContentType = assemblyName.ContentType;

            var interopModule = module.Context.ResolveAssembly(interopAssemblyName, false);
            if (interopModule == null)
                return null;

            var pregeneratedMethod = GetMatchingMethod(interopModule, method);
            if (pregeneratedMethod == null)
            {
                // TODO: Better error message
                throw new MissingMemberException("Missing method in " + interopAssemblyName.Name + ":" + method.ToString());
            }
            return pregeneratedMethod;
        }

        // Returns null if no matching method is found
        private static MethodDesc GetMatchingMethod(ModuleDesc module, MethodDesc method)
        {
            var matchingType = GetMatchingType(module, method.OwningType);
            if (matchingType == null)
                return null;
            return matchingType.GetMethod(method.Name, method.Signature);
        }

        // Returns null if no matching type is found
        private static TypeDesc GetMatchingType(ModuleDesc module, TypeDesc type)
        {
            var metadataType = (MetadataType)type;
            var containingType = metadataType.ContainingType;
            if (containingType != null)
            {
                var matchingContainingType = (MetadataType)GetMatchingType(module, containingType);
                if (matchingContainingType == null)
                    return null;
                return matchingContainingType.GetNestedType(metadataType.Name);
            }
            else
            {
                return module.GetType(metadataType.Namespace, metadataType.Name, false);
            }
        }
    }
}