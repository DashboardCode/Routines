namespace Vse.AdminkaV1
{
    public static class LengthConstants
    {
        public const int GoodForKey = 4;         // 64 bit can be effective primary key
        public const int GoodForColumnName = 8;  
        public const int GoodForFormLabel = 16; 
        public const int GoodForName = 32;   // real entities names (for single line, non-scrollable textboxes)
        public const int GoodForTitle = 64;    
        public const int AdName = 126; //  21 (domain)  + 1 ("\") +  104 (username)
        public const int GoodForLongTitle = 128; 
        public const int GoodForMultilineNote = 512; 
    }
}
