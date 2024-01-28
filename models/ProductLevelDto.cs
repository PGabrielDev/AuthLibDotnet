namespace CheckPermissionsLib.models
{
    public class ProductLevelDto
    {
        public string Name { get; set; }
        public ICollection<string> Permissions { get; set; }
    }
}
