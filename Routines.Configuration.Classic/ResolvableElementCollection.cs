using System.Configuration;

namespace DashboardCode.Routines.Configuration.Classic
{
    [ConfigurationCollection(typeof(ResolvableElement), AddItemName = ResolvableElementName, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ResolvableElementCollection : ConfigurationElementCollection
    {
        public const string ResolvableElementName = "resolvable";
        
        #region Overrides
        protected override string ElementName
        {
            get
            {
                return ResolvableElementName;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ResolvableElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ICollectionMemberElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        #endregion

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Add(ResolvableElement value)
        {
            base.BaseAdd(value);
        }
    }
}
