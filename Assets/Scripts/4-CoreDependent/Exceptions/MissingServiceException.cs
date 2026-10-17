using System;
using System.Collections.Generic;
using System.Linq;

namespace PKLStudios
{
    public class MissingServiceException : Exception
    {
        private static readonly IReadOnlyDictionary<Type, IBaseService> reference = GameServices.Services;

        public MissingServiceException(Type serviceType)
            : base(ExceptionMessage(serviceType, reference)) {}

        static string ExceptionMessage(Type service, IReadOnlyDictionary<Type, IBaseService> references)
        {
            Type[] implementedInterfaces = service.GetInterfaces();

            string baseMessage = $"Service {service.Name} is not found in GameService.\n";
            
            string interfaceList = implementedInterfaces.Length > 0 ? 
                                string.Join("\n", implementedInterfaces.Select(type => type.Name)) : 
                                "No Implementations";

            string registeredServices = references.Count > 0 ? 
                                        string.Join("\n", references.Keys.Select(type => type.Name)) :
                                        "No Services Registered";

            return
            $"{baseMessage}\n" + 
            $"Service Name: {service.Name}\n" + 
            $"Implemented Interfaces:\n{interfaceList}\n\n" + 
            $"Registered Services:\n{registeredServices}";
        }
    }
}
