using System.Configuration;


namespace Vse.Routines.Configuration
{
    [ConfigurationCollection(typeof(RoutineElement), AddItemName = "routine", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class RoutineElementCollection : ConfigurationElementCollection
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

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "routine";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RoutineElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ICollectionMemberElement)element).Key;
        }
        #endregion

        public RoutineElement this[int index]
        {
            get
            {
                return (RoutineElement)BaseGet(index);
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

        public new RoutineElement this[string name]
        {
            get
            {
                return (RoutineElement)BaseGet(name);
            }
        }
    }
}
