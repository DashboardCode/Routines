using System.Configuration;


namespace Vse.Routines.Configuration
{
    [ConfigurationCollection(typeof(RoutineElement), AddItemName = RoutineElementName, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class RoutineElementCollection : ConfigurationElementCollection
    {
        public const string RoutineElementName = "routine";

        #region Overrides
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
                return RoutineElementName;
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

        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion

        public void Add(RoutineElement value)
        {
            base.BaseAdd(value);
        }

        public void Remove(RoutineElement value)
        {
            BaseRemove(value);
        }
    }
}
