using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions
{
    public static class GenericTypeExtensions
    {
        public static string GetGenericTypeName(this Type type)
        {
            //Representa la cadena vacía. Este campo es de solo lectura.
            var typeName = string.Empty;

            //Obtiene un valor ya se el tipo actual es un tipo genérico.
            // Devuelve: True, si el tipo es genérico, de otra manera, false.
            if (type.IsGenericType)
            {
                //Concatena los elementos de una matriz especificada o los miembros de una colección, 
                //utilizando el separador especificado entre cada elemento o miembro.
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }
    }
}
