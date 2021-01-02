// using System;
// using System.Reflection;
// using Microsoft.AspNetCore.Mvc.Controllers;

// namespace JCTools.GenericCrud.Settings.DependencyInjection
// {
//     public class CrudControllerFeatureProvider : ControllerFeatureProvider
//     {

//         protected override bool IsController(TypeInfo typeInfo)
//         {
//             if (TryFindGenericController(typeInfo, out Type generic))
//                 return false;

//             return base.IsController(typeInfo);
//         }
//         /// <summary>
//         /// Try find the <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> definition
//         /// used into the specified type
//         /// </summary>
//         /// <param name="controllerType">The controller type to review</param>
//         /// <param name="resultType">The found <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> definition</param>
//         /// <returns>True if the <see cref="Controllers.GenericController{TContext, TModel, TKey}"/> definition is found, else false</returns>
//         private bool TryFindGenericController(Type controllerType, out Type resultType)
//         {
//             if (controllerType.Name.Equals(Configurator.GenericControllerType.Name))
//                 resultType = controllerType;
//             else if (controllerType.BaseType == null)
//                 resultType = null;
//             else
//                 return TryFindGenericController(controllerType.BaseType, out resultType);

//             return resultType != null;
//         }
//     }
// }