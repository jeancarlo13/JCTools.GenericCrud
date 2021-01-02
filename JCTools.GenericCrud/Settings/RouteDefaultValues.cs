using Microsoft.AspNetCore.Routing;

namespace JCTools.GenericCrud.Settings
{
    internal class RouteDefaultValues
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ModelType { get; set; }
        internal ICrudType ICrudType { get; set; }

        public bool IsEquivalent(RouteValueDictionary obj)
        {
            var isSameController = obj.ContainsKey(nameof(Controller))
                && obj[nameof(Controller)].Equals(Controller);
            var isSameAction = obj.ContainsKey(nameof(Action))
                && obj[nameof(Action)].Equals(Action);
            var isSameModelType = obj.ContainsKey(nameof(ModelType))
                && obj[nameof(ModelType)].Equals(ModelType);
            var isSameICrudType = obj.ContainsKey(nameof(RouteDefaultValues.ICrudType))
                && obj[nameof(RouteDefaultValues.ICrudType)].Equals(this.ICrudType);

            return isSameController && isSameAction && isSameModelType && isSameICrudType;
        }
    }
}