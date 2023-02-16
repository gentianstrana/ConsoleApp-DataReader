namespace ConsoleApp
{
    //The Schema, ParentName, ParentType, DataType, and IsNullable properties are all converted from fields to properties.
    //The IsNullable property is changed from a string to a boolean type to better represent its values.
    //The NumberOfChildren property is changed from a double to an int type to better represent its values.
    public class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public int NumberOfChildren { get; set; }
    }
}
