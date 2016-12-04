using System.Configuration;

namespace Vse.Routines.Configuration
{
    [ConfigurationCollection(typeof(ResolvableElement), AddItemName = "resolvable", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ResolvableElementCollection : ConfigurationElementCollection
    {
        private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        
        #region Overrides
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "resolvable";
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

        public ResolvableElement this[int index]
        {
            get
            {
                return (ResolvableElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ResolvableElement this[string name]
        {
            get
            {
                return (ResolvableElement)BaseGet(name);
            }
        }
    }
}
